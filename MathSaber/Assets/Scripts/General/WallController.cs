using Blocks;
using Equations;
using UnityEngine;
using Utils;

namespace General
{
    public class WallController : MonoBehaviour
    {
        public EquationSpawnerBase equationSpawnerBase;

        #region Unity Functions

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.CorrectAnswer) || other.CompareTag(TagManager.InCorrectAnswer))
            {
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
                if (!cubeController.HasParentDetectedCollisions())
                {
                    cubeController.NotifyParentCollision();
                    equationSpawnerBase.DecrementSpeed();

                    string equation = cubeController.Equation;
                    string answer = cubeController.Answer;
                    float startTime = cubeController.StartTime;

                    EquationsAnalyticsManager.Instance.AddEquationToList(equation, answer, false, Time.time - startTime);
                }

                Destroy(cubeController.transform.parent.gameObject);
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                Destroy(other.gameObject);
            }
        }

        #endregion
    }
}