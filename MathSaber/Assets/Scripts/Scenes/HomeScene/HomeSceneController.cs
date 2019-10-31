using System;
using Equations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.HomeScene
{
    public class HomeSceneController : MonoBehaviour
    {
        public float sceneSwitchTime = 3;

        private float _currentSceneSwitchCountDown;
        private bool _sceneSwitchActive;

        #region Unity Functions

        private void Update()
        {
            if (!_sceneSwitchActive)
            {
                return;
            }

            _currentSceneSwitchCountDown -= Time.deltaTime;
            if (_currentSceneSwitchCountDown <= 0)
            {
                EquationsAnalyticsManager.Instance.ClearAnalyticsData();
                SceneManager.LoadScene(1);
            }
        }

        #endregion

        #region External Functions

        public void ActivateSceneSwitchCountDown()
        {
            if (_sceneSwitchActive)
            {
                return;
            }

            _sceneSwitchActive = true;
            _currentSceneSwitchCountDown = sceneSwitchTime;
        }

        #endregion

        #region Singleton

        private static HomeSceneController _instance;

        public static HomeSceneController Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}