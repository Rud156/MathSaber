using UnityEngine;

namespace UnityTemplateProjects.Scenes.EndScene
{
    public class EndSceneController : MonoBehaviour
    {
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