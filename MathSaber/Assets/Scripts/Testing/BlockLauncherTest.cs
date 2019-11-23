﻿using System;
using UnityEngine;

namespace Testing
{
    public class BlockLauncherTest : MonoBehaviour
    {
        public GameObject fruitNinjaBlockPrefab;
        public Transform launchPoint;

        #region Unity Functions

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Instantiate(fruitNinjaBlockPrefab, launchPoint.position, Quaternion.identity);
            }
        }

        #endregion
    }
}