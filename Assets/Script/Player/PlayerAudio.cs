using System;
using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        public AudioClip walkClip;
        public AudioClip jumpClip;
        public AudioClip landClip;
        public AudioClip healClip;
        public AudioClip hurtClip;
        private PlayerController playerController;
        private PlayerAttribute playerAttribute;
        private AudioSource audioSource;
        private void Start()
        {
            playerAttribute = GetComponent<PlayerAttribute>();
            playerController = playerAttribute.playerController;
            playerAttribute.playerHealth.healAction += (blood) =>
            {
                this.Play(healClip);
            };
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            playerAttribute.playerHealth.damageAction += (val) =>
            {
                Play(hurtClip);
            };
        }
        void Play(AudioClip clip)
        {
            if (clip == null)
                return;
            if (clip == audioSource.clip)
            {
                if (audioSource.isPlaying)
                    return;
                audioSource.Play();
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
        }
    }
}