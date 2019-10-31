using System;
using UnityEngine;

namespace General
{
    public class PlayerDontDestroyOnLoad : MonoBehaviour
    {
        #region Singleton

        private static PlayerDontDestroyOnLoad _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(_instance.gameObject);
                _instance = this;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion
    }
}