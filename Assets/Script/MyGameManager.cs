using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class MyGameManager : MonoBehaviour
    {
        /// <summary>
        /// 当前场景的stagemanager, 控制场景的属性, 包括重力方向,大小.
        /// </summary>
        public StageManager stageManager { get; private set; } = null;
        public static MyGameManager instance = null;
        public Action onSceneChanged;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        void Start()
        {
            SceneManager.activeSceneChanged += this.onSceneChange;
        }
        void onSceneChange(Scene pre, Scene now)
        {
            this.stageManager = null;
            onSceneChanged?.Invoke();
        }
        public void setStageManager(StageManager stageManager)
        {
            this.stageManager = stageManager;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}