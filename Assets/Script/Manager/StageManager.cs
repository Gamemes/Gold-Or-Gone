using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    /// <summary>
    /// 场景管理, 包括这个场景的一切, 比如玩家, 重力
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        public Vector2 gravity
        {
            get
            {
                return Physics2D.gravity;
            }
            private set
            {
                gravityDirection = value.normalized;
                Physics2D.gravity = value;
            }
        }
        private float _gravitySize = 0;
        /// <summary>
        /// 重力大小,用<see cref="addGrivate"/>改变重力大小, 不要直接调用setter函数除非你知道原因
        /// </summary>
        /// <value></value>
        public float gravitySize
        {
            get
            {
                if (_gravitySize * _gravitySize != gravity.sqrMagnitude)
                {
                    _gravitySize = gravity.magnitude;
                }
                return _gravitySize;
            }
            set
            {
                Debug.Log($"set gravity size to {value}");
                _gravitySize = MathF.Min(50, value);
                Vector2 griv = new Vector2(0, -_gravitySize);
                gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
                Debug.Log($"change grivate {_gravitySize}");
            }
        }
        private float initalGrivateSize;
        public GameObject playerPrefab;
        /// <summary>
        /// 重力的方向
        /// </summary>
        public float gravityAngle { get; private set; }
        /// <summary>
        /// 重力的normalized
        /// </summary>
        /// <value></value>
        public Vector2 gravityDirection { get; private set; }
        public Action<float> onGravityRotated;
        public Action<float> onGravityRotateCompleted;
        /// <summary>
        /// 这个场景里所有的Player
        /// </summary>
        public List<GameObject> stagePlayers { get; private set; }
        /// <summary>
        /// 每添加了一个玩家就广播一次
        /// </summary>
        public Action<GameObject> onAddPlayer;
        /// <summary>
        /// 当前的上帝玩家, 调用<see cref="changeGloadPlayer"/>,而不是直接调用setter除非你知道这样做会出现的问题. 
        /// </summary>
        public GameObject GodPlayer
        {
            get
            {
                return _godPlayer;
            }
            set
            {
                if (value == _godPlayer)
                    return;
                Debug.Log($"change god player to {value}");
                _godPlayer = value;
                onGlodPlayerChange?.Invoke(value);
            }
        }
        private GameObject _godPlayer;
        /// <summary>
        /// 当上帝玩家切换, 传入切换后的玩家(GameObject)
        /// </summary>
        public Action<GameObject> onGlodPlayerChange;
        public Cinemachine.CinemachineVirtualCamera stageCamera = null;
        public NetworkStageManager networkStage = null;
        public bool isOnline = false;
        public bool isdebug = true;
        void Awake()
        {
            MyGameManager.instance.setStageManager(this);
            gravityDirection = gravity.normalized;
            initalGrivateSize = gravity.magnitude;
            _gravitySize = initalGrivateSize;
            Debug.Log($"{gravity} initalGrivateSize {initalGrivateSize}");
            networkStage = GetComponent<NetworkStageManager>();
            if (stageCamera == null)
                stageCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

            //是否是线上模式
            isOnline = (networkStage != null);
            onAddPlayer += (player) => { this.stagePlayers.Add(player); };
            //如果不是线上模式就需要接入多设备输入
            if (!isOnline)
                InputSystem.onDeviceChange += this.onDeviceChange;
        }
        private void Start()
        {
            //如果不是线上模式, 需要同步输入设备到玩家.
            if (!isOnline)
            {
                stagePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
                if (!isdebug)
                {
                    synchroPlayerAndDevice();
                    changeGloadPlayer(stagePlayers[UnityEngine.Random.Range(0, stagePlayers.Count)]);
                }
            }
        }
        /// <summary>
        /// 改变当前的上帝玩家
        /// </summary>
        /// <param name="player">目标玩家</param>
        public void changeGloadPlayer(GameObject player)
        {
            if (isOnline)
            {
                networkStage.CmdChangeGodPlayer(player);
            }
            else
            {
                GodPlayer = player;
            }
        }
        #region 设备控制
        void addPlayer(InputDevice inputDevice)
        {
            Debug.Log($"ADD PLAYER {inputDevice}");
            //建立新的Player
            GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var controller = newPlayer.GetComponent<Player.FrameInput>();
            controller.setDevice(inputDevice);
            //添加到stagePlayers
            stagePlayers.Add(newPlayer);
        }
        /// <summary>
        /// 同步输入设备和玩家
        /// </summary>
        void synchroPlayerAndDevice()
        {
            // 获取已经连接的设备, 根据设备数量创建对应数量的玩家.
            var devices_ = from dev in InputSystem.devices where (dev is Gamepad || dev is Keyboard) select dev;
            var devices = devices_.ToList();
            if (stagePlayers.Count > devices.Count)
            {
                throw new UnityException($"players is {stagePlayers.Count} > devices:{devices.Count}");
            }
            for (var i = 0; i < stagePlayers.Count; i++)
            {
                stagePlayers[i].GetComponent<Player.FrameInput>().setDevice(devices[i]);
            }
            for (var i = stagePlayers.Count; i < devices.Count; i++)
            {
                addPlayer(devices[i]);
            }
        }
        void onDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            switch (inputDeviceChange)
            {
                case InputDeviceChange.Added:
                    addPlayer(inputDevice);
                    break;
            }
        }
        #endregion
        #region 重力控制
        /// <summary>
        /// 旋转重力, 直接旋转, 没有过程
        /// </summary>
        /// <param name="angle">度数</param>
        private void rotate_Gravity(float angle)
        {
            gravity = Quaternion.Euler(0, 0, angle) * gravity;
            onGravityRotated?.Invoke(angle);
            gravityAngle += angle;
            stageCamera.transform.Rotate(new Vector3(0, 0, angle));
        }

        public IEnumerator _rotateGravityDuration(float angle, float duration)
        {
            float _ang = 0;
            float dangle, t = 0f, x, _x;
            float dir = angle < 0 ? -1 : 1;
            angle = Mathf.Abs(angle);
            while (t < duration)
            {
                _x = (float)t / duration;
                t += Time.deltaTime;
                x = (float)t / duration;
                dangle = (Mathf.Pow(x, 4) - Mathf.Pow(_x, 4)) * angle;
                dangle = Mathf.Min(dangle, angle - _ang);
                rotate_Gravity(dir * dangle);
                _ang += dangle;
                yield return null;
            }
            onGravityRotateCompleted?.Invoke(angle * dir);
            yield return null;
        }
        public void rotateGravityDuration(float angle, float duration = 1f)
        {
            if (isOnline)
            {
                networkStage.CmdRotateGravityDuration(angle, duration);
            }
            else
            {
                StartCoroutine(_rotateGravityDuration(angle, duration));
            }
        }
        public void reSetGrivateSize()
        {
            if (isOnline)
            {
                Debug.Log($"在线模式未适配");
            }
            else
            {
                Vector2 griv = new Vector2(0, -initalGrivateSize);
                gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
            }
        }
        public void scaleGrivate(float val = 1.0f, float duration = -1f)
        {
            if (isOnline)
            {
                Debug.Log($"在线模式未适配");
            }
            else
            {
                Vector2 griv = new Vector2(0, -initalGrivateSize * val);
                gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
            }
        }
        public void addGrivate(float val)
        {
            if (isOnline)
            {
                networkStage.CmdAddGrivate(val);
            }
            else
            {
                gravitySize += val;
            }
        }
        #endregion
        private void OnDisable()
        {

        }

    }
}