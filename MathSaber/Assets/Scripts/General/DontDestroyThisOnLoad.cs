using System;
using UnityEngine;

namespace General
{
    public class DontDestroyThisOnLoad : MonoBehaviour
    {
        #region Unity Functions

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #endregion
    }
}