using System;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class LightFlasherManager : MonoBehaviour
    {
        public List<LightFlasher> lightFlashers;

        #region Unity Functions

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                FlashAllLights();
            }
        }

        #endregion

        #region External Functions

        public void FlashAllLights()
        {
            foreach (LightFlasher lightFlasher in lightFlashers)
            {
                lightFlasher.ActiveFlashEmission();
            }
        }

        #endregion
    }
}