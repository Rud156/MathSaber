using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class LightFlasherManager : MonoBehaviour
    {
        public float timeBetweenFlash;
        public List<LightFlasher> leftLightFlashers;
        public List<LightFlasher> rightLightFlashers;

        #region External Functions

        public void FlashAllLights()
        {
            int totalLeftCount = leftLightFlashers.Count;
            for (int i = 0; i < leftLightFlashers.Count; i++)
            {
                FlashLight(leftLightFlashers[i], (totalLeftCount - i) * timeBetweenFlash);
            }

            int totalRightCount = rightLightFlashers.Count;
            for (int i = 0; i < rightLightFlashers.Count; i++)
            {
                FlashLight(rightLightFlashers[i], (totalRightCount - i) * timeBetweenFlash);
            }
        }

        #endregion

        #region Utility Functions

        private void FlashLight(LightFlasher lightFlasher, float waitTime)
        {
//            yield return new WaitForSeconds(waitTime);
            lightFlasher.ActiveFlashEmission();
        }

        #endregion
    }
}