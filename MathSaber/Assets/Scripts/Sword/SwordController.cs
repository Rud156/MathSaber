using EzySlice;
using General;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;

namespace Sword
{
    public class SwordController : MonoBehaviour
    {
        public float forceAmount = 5;
        public float minTorqueAmount = 5;
        public float maxTorqueAmount = 20;
        public Material borderMaterial;

        private TextMeshPro _debugText;
        private Transform _objectHolder;

        private Vector3 _contactStartPosition;
        private Vector3 _contactEndPoint;

        #region Unity Functions

        private void Start()
        {
            _debugText = GameObject.FindGameObjectWithTag(TagManager.DisplayText).GetComponent<TextMeshPro>();
            _objectHolder = GameObject.FindGameObjectWithTag(TagManager.BlockHolder).transform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject other = collision.gameObject;
            if (other.tag == "Start")
            {
                SceneManager.LoadScene("Rud156");
            }
<<<<<<< Updated upstream
=======
            if (other.CompareTag(TagManager.RestartBlock))
            {   
                SceneManager.LoadScene(1);
                return;
            }

>>>>>>> Stashed changes
            EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
            if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
            {
                return;
            }

            if (cubeController.HasParentDetectedCollisions())
            {
                return;
            }

            if (other.CompareTag(TagManager.CorrectAnswer))
            {
                _debugText.text = "Correct Answer";
                Debug.Log("Correct Answer Hit");
            }
            else if (other.CompareTag(TagManager.InCorrectAnswer))
            {
                _debugText.text = "Wrong Answer";
                Debug.Log("InCorrect Answer Hit");
            }
            
            cubeController.NotifyParentCollision();
            cubeController.DestroyAllChildrenImmediate();

            _contactStartPosition = collision.contacts[0].point;
        }

        private void OnCollisionStay(Collision collision)
        {
            GameObject other = collision.gameObject;

            EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
            if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
            {
                return;
            }

            if (!cubeController.HasParentDetectedCollisions())
            {
                return;
            }

            _contactEndPoint = collision.contacts[0].point;
        }

        private void OnCollisionExit(Collision collision)
        {
            GameObject other = collision.gameObject;

            EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
            if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
            {
                return;
            }

            if (!cubeController.HasParentDetectedCollisions())
            {
                return;
            }

            SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
        }

        #endregion

        #region Utility Functions

        private void SliceCollidingGameObject(GameObject objectToSlice, Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 direction = endPoint - startPoint;
            Quaternion lookDirection = Quaternion.LookRotation(direction);

            GameObject[] slicedGameObjects = objectToSlice.SliceInstantiate(startPoint, lookDirection * Vector3.up);
            Debug.Log($"Final GameObjects Count: {slicedGameObjects.Length}");

            foreach (GameObject slicedGameObject in slicedGameObjects)
            {
                MeshCollider meshCollider = slicedGameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;

                Rigidbody slicedObjectRigidBody = slicedGameObject.AddComponent<Rigidbody>();
                slicedObjectRigidBody.velocity = Random.onUnitSphere * forceAmount;
                float randomTorque = Random.Range(minTorqueAmount, maxTorqueAmount);
                slicedObjectRigidBody.AddTorque(randomTorque * Vector3.one, ForceMode.Impulse);

                int totalMaterials = slicedGameObject.GetComponent<MeshRenderer>().materials.Length;
                slicedGameObject.GetComponent<MeshRenderer>().materials[totalMaterials - 1] = borderMaterial;

                slicedGameObject.transform.SetParent(_objectHolder);
            }

            Destroy(objectToSlice);
        }

        #endregion
    }
}