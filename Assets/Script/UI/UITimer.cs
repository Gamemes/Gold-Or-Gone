using System;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class UITimer : MonoBehaviour
    {
        public bool timing = false;
        public AudioClip timingClip;
        public float countSeconds;
        public Action timeUpAction = null;
        [SerializeField] private Text textUI;
        [SerializeField] private AudioSource audioSource;
        private void Start()
        {
            Debug.Assert(textUI != null);
            Debug.Assert(audioSource != null);
        }
        public bool startTiming(float seconds, Action onTimeUp = null)
        {
            if (timing)
                return false;
            countSeconds = seconds;
            if (seconds > 30)
                textUI.color = Color.white;
            timeUpAction = onTimeUp;
            timing = true;
            audioSource.loop = true;
            audioSource.clip = timingClip;
            audioSource.Play();
            return true;
        }
        public bool stopTiming()
        {
            if (!timing)
                return false;
            timing = false;
            timeUpAction?.Invoke();
            audioSource.Stop();
            return true;
        }
        private void Update()
        {
            if (timing)
            {
                countSeconds -= Time.deltaTime;
                show();
                if (countSeconds <= 0)
                {
                    stopTiming();
                }
            }
        }
        private string timeToStr(float seconds)
        {
            int sec = (int)seconds;
            return $"{sec / 60}:{sec % 60}";
        }
        private void show()
        {
            if (countSeconds <= 30)
                textUI.color = Color.red;
            textUI.text = timeToStr(countSeconds);
        }

    }
}
