using General;
using TMPro;
using UnityEngine;
using Utils;

namespace Sword
{
    public class SwordController : MonoBehaviour
    {
        private TextMeshPro _debugText;

        #region Unity Functions

        private void Start() => _debugText = GameObject.FindGameObjectWithTag(TagManager.DisplayText).GetComponent<TextMeshPro>();

        private void OnTriggerEnter(Collider other)
        {
            CubeMovementAndDestruction cubeMovement = other.GetComponent<CubeMovementAndDestruction>();
            if (!cubeMovement)
            {
                return;
            }

            if (cubeMovement.HasParentDetectedCollisions())
            {
                return;
            }

            if (other.CompareTag(TagManager.CorrectAnswer))
            {
                _debugText.text = "Correct Answer";
                cubeMovement.NotifyParentCollision();
                Destroy(other.gameObject);

                Debug.Log("Correct Answer Hit");
            }

            if (other.CompareTag(TagManager.InCorrectAnswer))
            {
                _debugText.text = "Wrong Answer";
                cubeMovement.NotifyParentCollision();
                Destroy(other.gameObject);

                Debug.Log("Wrong Answer Hit");
            }
        }

        #endregion
    }
}