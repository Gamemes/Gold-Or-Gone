using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class MyGameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public StageManager stageManager { get; private set; } = null;
        public static MyGameManager myGameManager = null;
        public Action onSceneChanged;
        private void Awake()
        {
            if (myGameManager == null)
            {
                myGameManager = this;
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
        void setStageManager(StageManager stageManager)
        {
            this.stageManager = stageManager;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}