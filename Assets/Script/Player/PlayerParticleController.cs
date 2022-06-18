using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        // Start is called before the first frame update
        public ParticleSystem walkParticle;
        private ParticleSystem.EmissionModule walkEmission;
        public ParticleSystem lanParticle;
        private ParticleSystem.EmissionModule landEmission;
        private PlayerAttribute playerAttribute;
        private PlayerController playerController;
        void Start()
        {
            playerAttribute = GetComponent<PlayerAttribute>();
            Debug.Assert(playerAttribute != null);
            playerController = playerAttribute.playerController;
            walkEmission = walkParticle.emission;
            landEmission = lanParticle.emission;
        }

        // Update is called once per frame
        void Update()
        {
            if (playerController.colDow)
                walkEmission.enabled = true;
            else
                walkEmission.enabled = false;
            if (playerController.LandingThisFrame)
                lanParticle.Play();
        }
    }
}