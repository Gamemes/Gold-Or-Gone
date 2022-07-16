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
                //Debug.Log($"set gravity size to {value}");
                _gravitySize = MathF.Min(50, value);
                Vector2 griv = new Vector2(0, -_gravitySize);
                gravity = Quaternion.Euler(0, 0, gravityAngle) * griv;
                Debug.Log($"change grivate size {_gravitySize}");
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
        public Dictionary<GameObject, Player.PlayerAttribute> stagePlayerAttributes { get; private set; }
        /// <summary>
        /// 每添加了一个玩家就广播一次
        /// </summary>
        public Action<GameObject> onAddPlayer;
        /// <summary>
        /// 当玩家退出
        /// </summary>
        public Action<GameObject> onRemovePlayer;
        /// <summary>
        /// todo: 这是个临时方案, 后期需要取消他现在的事件绑定
        /// 游戏开始事件, 在游戏开始的时候会触发此事件.
        /// 游戏开始是建立在 
        /// 1.玩家数量大于2 
        /// 2.玩家全部准备就绪
        /// </summary>
        public Action onGameStart;
        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public Action onReGame;
        /// <summary>
        /// 当前的上帝玩家, 调用<see cref="ChangeGloadPlayer"/>,而不是直接调用setter除非你知道这样做会出现的问题. 
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
        private GameObject _godPlayer = null;
        /// <summary>
        /// 当上帝玩家切换, 传入切换后的玩家(GameObject)
        /// </summary>
        public Action<GameObject> onGlodPlayerChange;
        public Cinemachine.CinemachineVirtualCamera stageCamera = null;
        public NetworkStageManager networkStage = null;
        public bool isOnline = false;
        public bool isdebug = true;
        //边界范围
        private PolygonCollider2D polygonCollider;
        //场景信息UI
        public UIStageInfo stageInfo;
        void Awake()
        {
            //初始化设置
            MyGameManager.instance.setStageManager(this);
            stagePlayers = new List<GameObject>();
            stagePlayerAttributes = new Dictionary<GameObject, Player.PlayerAttribute>();

            gravityDirection = gravity.normalized;
            initalGrivateSize = gravity.magnitude;
            _gravitySize = initalGrivateSize;
            Debug.Log($"{gravity} initalGrivateSize {initalGrivateSize}");

            //检测是否是线上模式
            networkStage = GetComponent<NetworkStageManager>();
            if (stageCamera == null)
                stageCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

            //是否是线上模式, 如果存在 NetworkStageManager 组件就说明是线上模式.
            isOnline = (networkStage != null);
            //每次增加玩家就加入stagePlayers里.
            onAddPlayer += (GameObject player) =>
            {
                this.stagePlayers.Add(player);
                this.stagePlayerAttributes.Add(player, player.GetComponent<Player.PlayerAttribute>());
                if (stagePlayers.Count >= 2)
                {
                    StartGame();
                }
            };
            onRemovePlayer += (player) =>
            {
                this.stagePlayers.Remove(player);
                this.stagePlayerAttributes.Remove(player);
            };
            //如果不是线上模式就需要接入多设备输入
            if (!isOnline)
                InputSystem.onDeviceChange += this.onDeviceChange;
            //获取多边形碰撞体(边界)
            this.polygonCollider = GetComponent<PolygonCollider2D>();
            //获取info UI
            this.stageInfo = GetComponentInChildren<UIStageInfo>();
        }
        private void Start()
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                onAddPlayer?.Invoke(player);
            }
            //如果不是线上模式, 需要同步输入设备到玩家.
            if (!isOnline && !isdebug)
            {
                synchroPlayerAndDevice();
            }
        }

        /// <summary>
        /// 改变当前的上帝玩家
        /// </summary>
        /// <param name="player">目标玩家</param>
        public void ChangeGloadPlayer(GameObject player)
        {
            if (isOnline)
            {
                networkStage.CmdChangeGodPlayer(player);
            }
            else
            {
                GodPlayer = player;
                stageInfo.ShowInfo($"Change God to {player}");
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
            onAddPlayer?.Invoke(newPlayer);
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
        private void CheckPlayerInBorder()
        {
            foreach (var player in this.stagePlayers)
            {
                if (!this.polygonCollider.OverlapPoint(player.transform.position))
                {
                    player.transform.position = new Vector3(0, 0, 0);
                    player.GetComponent<Player.PlayerAttribute>().playerHealth.CauseDamage(1);
                    Debug.Log($"{player} out");
                }
            }
        }
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            CheckPlayerInBorder();
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            ChangeGloadPlayer(stagePlayers[UnityEngine.Random.Range(0, stagePlayers.Count)]);
            onGameStart?.Invoke();
        }
        /// <summary>
        /// 重新游戏
        /// </summary>
        public void ReGame()
        {
            if (isOnline)
            {
                networkStage.CmdReGame();
            }
            else
            {
                onReGame?.Invoke();
            }
            StartCoroutine(Utils.Utils.DelayInvoke(() =>
            {
                ChangeGloadPlayer(stagePlayers[UnityEngine.Random.Range(0, stagePlayers.Count)]);
            }, 4f));
        }

    }
}