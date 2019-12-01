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

        private int _sceneIndex;

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
                SceneManager.LoadScene(_sceneIndex);
            }
        }

        #endregion

        #region External Functions

        public void ActivateSceneSwitchCountDown(int scene)
        {
            if (_sceneSwitchActive)
            {
                return;
            }

            _sceneIndex = scene;
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