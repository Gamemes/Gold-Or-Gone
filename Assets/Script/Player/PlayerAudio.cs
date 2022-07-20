using System;
using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        public AudioClip walkClip;
        public AudioClip jumpClip;
        public AudioClip landClip;
        private PlayerController playerController;
        private AudioSource audioSource;
        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            Debug.Assert(playerController != null);
        }
        void Play(AudioClip clip)
        {
            if (clip == null)
                return;
            if (clip == audioSource.clip)
            {
                if (audioSource.isPlaying)
                    return;
                audioSource.PlayOneShot(clip);
                return;
            }
            audioSource.clip = clip;
            audioSource.PlayOneShot(clip);
        }
        private void Update()
        {
            if (playerController.LandingThisFrame)
            {
                Play(landClip);
            }
            else if (playerController.JumpingThisFrame)
            {
                Play(jumpClip);
            }
            else if (playerController.colDow && playerController.Velocity.x != 0)
            {
                audioSource.Stop();
            }
        }
    }
}