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
        private float _gravityAngle = 0f;
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
            InputSystem.onDeviceChange += this.onDeviceChange;
            if (stageCamera == null)
                stageCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        }
        private void Start()
        {
            synchroPlayerAndDevice();
            changeGloadPlayer(stagePlayers[UnityEngine.Random.Range(0, stagePlayers.Count - 1)]);
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
            var devices_ = from dev in InputSystem.devices where dev is Gamepad || dev is Keyboard select dev;
            var devices = devices_.ToList();
            stagePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
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
        }

        IEnumerator _rotateGravityDuration(float angle, float duration)
        {
            float _ang = 0;
            float dangle, t = 0f, x;
            while (t < duration)
            {
                t += Time.deltaTime;
                x = (float)t / duration;
                dangle = Mathf.Pow(x, 3) * 4 * Time.deltaTime * angle;
                dangle = Mathf.Min(dangle, angle - _ang);
                rotate_Gravity(dangle);
                _ang += dangle;
                //Debug.Log($"{t} {x} {dangle} {_ang}");
                yield return null;
            }
            yield return null;
        }
        public void rotateGravityDuration(float angle, float duration = 1f)
        {
            StartCoroutine(_rotateGravityDuration(angle, duration));
        }
        #endregion
        private void OnDisable()
        {

        }

    }
}