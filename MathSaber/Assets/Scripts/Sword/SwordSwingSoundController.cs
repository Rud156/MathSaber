using System;
using UnityEngine;
using Utils;
using Valve.VR;

namespace Sword
{
    public class SwordSwingSoundController : MonoBehaviour
    {
        [Header("Audio Source")] public AudioSource swordSwingSource;

        [Header("Audio Pitch")] public float minPitch = 0;
        public float maxPitch = 2.5f;

        [Header("Swing Velocity Data")] public float minVelocityThreshold;
        public float maxVelocityThreshold;

        #region Update

        private void Update()
        {
            Vector3 swingVelocity = SteamVR_Actions._default.Pose.velocity;
            float mappedPitch = ExtensionFunctions.Map(swingVelocity.sqrMagnitude,
                minVelocityThreshold, maxVelocityThreshold,
                minPitch, maxPitch);

            swordSwingSource.pitch = mappedPitch;
        }

        #endregion
    }
}