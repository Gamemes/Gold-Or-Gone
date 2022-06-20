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
        // Start is called before the first frame update
        public Vector2 gravity
        {
            get
            {
                return Physics2D.gravity;
            }
            set
            {
                gravityDirection = value.normalized;
                Physics2D.gravity = value;
            }
        }
        private float _gravitySize = 0;
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
                Debug.Log($"{value}");
                _gravitySize = MathF.Min(50, value);
                Vector2 griv = new Vector2(0, -_gravitySize);
                gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
                Debug.Log($"change grivate {_gravitySize}");
            }
        }
        private float _gravityAngle = 0f;
        private float initalGrivateSize;
        public GameObject playerPrefab;
        public float gravityAngle
        {
            get
            {
                return _gravityAngle;
            }
            private set
            {
                //Debug.Log(value);
                _gravityAngle = value;

            }
        }
        public Vector2 gravityDirection { get; private set; }
        public bool isdebug = true;
        public Action<float> onGravityRotated;
        /// <summary>
        /// 这个场景里所有的Player
        /// </summary>
        public List<GameObject> stagePlayers { get; private set; }
        /// <summary>
        /// 当前的上帝玩家
        /// </summary>
        public GameObject glodPlayer { get; private set; }
        /// <summary>
        /// 当上帝玩家切换, 传入切换后的玩家(GameObject)
        /// </summary>
        public Action<GameObject> onGlodPlayerChange;
        public Cinemachine.CinemachineVirtualCamera stageCamera = null;
        void Awake()
        {
            MyGameManager.instance.setStageManager(this);
            gravityDirection = gravity.normalized;
            initalGrivateSize = gravity.magnitude;
            _gravitySize = initalGrivateSize;
            Debug.Log($"{gravity} initalGrivateSize {initalGrivateSize}");
            InputSystem.onDeviceChange += this.onDeviceChange;
            if (stageCamera == null)
                stageCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        }
        private void Start()
        {
            stagePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
            if (!isdebug)
            {
                synchroPlayerAndDevice();
                changeGloadPlayer(stagePlayers[UnityEngine.Random.Range(0, stagePlayers.Count)]);
            }
        }
        void Update()
        {

        }
        /// <summary>
        /// 改变当前的上帝玩家
        /// </summary>
        /// <param name="player">目标玩家</param>
        public void changeGloadPlayer(GameObject player)
        {
            if (player == this.glodPlayer)
                return;
            Debug.Log($"change god player to {player}");
            this.glodPlayer = player;
            onGlodPlayerChange?.Invoke(player);
        }
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
        #region 设备控制
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
        public void rotate_Gravity(float angle)
        {
            gravity = Quaternion.Euler(0, 0, angle) * gravity;
            onGravityRotated?.Invoke(angle);
            gravityAngle += angle;
            stageCamera.transform.Rotate(new Vector3(0, 0, angle));
        }

        IEnumerator _rotateGravityDuration(float angle, float duration)
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
            yield return null;
        }
        public void rotateGravityDuration(float angle, float duration = 1f)
        {
            Debug.Log($"rotate {angle}");
            StartCoroutine(_rotateGravityDuration(angle, duration));
        }
        public void reSetGrivateSize()
        {
            Vector2 griv = new Vector2(0, -initalGrivateSize);
            gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
        }
        public void scaleGrivate(float val = 1.0f, float duration = -1f)
        {
            Vector2 griv = new Vector2(0, -initalGrivateSize * val);
            gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
        }
        public void addGrivate(float val)
        {
            gravitySize += val;
        }
        #endregion
        private void OnDisable()
        {

        }

    }
}