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
        public StageManager currentStage { get; private set; } = null;
        public static MyGameManager instance = null;
        public Action onSceneChanged;
        public static StageManager CurrentStageManager()
        {
            //Debug.Assert(MyGameManager.instance.currentStage != null);
            if (MyGameManager.instance.currentStage == null)
                Debug.LogWarning("尝试获得空值场景管理");
            return MyGameManager.instance.currentStage;
        }
        public static void ShowInfoInCurrentStage(string info)
        {
            Debug.Assert(MyGameManager.instance.currentStage != null);
            MyGameManager.instance.currentStage.stageInfo.ShowInfo(info);
        }
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
            this.currentStage = null;
            onSceneChanged?.Invoke();
        }
        public void setStageManager(StageManager stage)
        {
            this.currentStage = stage;
        }
        void Update()
        {

        }
    }
}