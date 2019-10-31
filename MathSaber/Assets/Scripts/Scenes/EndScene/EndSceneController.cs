using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UnityTemplateProjects.Scenes.EndScene
{
    public class EndSceneController : MonoBehaviour
    {
        public float sceneSwitchTime = 3;

        private float _currentSceneTime;
        private bool _sceneSwitchActive;

        private GameObject _player;

        #region Unity Functions

        private void Start() => _player = GameObject.FindGameObjectWithTag(TagManager.Player);

        private void Update()
        {
            if (!_sceneSwitchActive)
            {
                return;
            }

            _currentSceneTime -= Time.deltaTime;
            if (_currentSceneTime <= 0)
            {
                DestroyImmediate(_player);
                SceneManager.LoadScene(0);
            }
        }

        #endregion

        #region External Functions

        public void ActiveSceneSwitchCountDown()
        {
            _sceneSwitchActive = true;
            _currentSceneTime = sceneSwitchTime;
        }

        #endregion

        #region Singleton

        private static EndSceneController _instance;

        public static EndSceneController Instance => _instance;

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