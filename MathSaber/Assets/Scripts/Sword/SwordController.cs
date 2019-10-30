using Equations;
using EzySlice;
using General;
using Scenes.HomeScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Valve.VR;
using Random = UnityEngine.Random;

namespace Sword
{
    public class SwordController : MonoBehaviour
    {
        [Header("Slicing Effect")] public float forceAmount = 5;
        public float minTorqueAmount = 5;
        public float maxTorqueAmount = 20;
        public Material borderMaterial;
        public GameObject sparkEffectPrefab;

        [Header("Audio Effect")] public AudioClip correctHitClip;
        public AudioClip wrongHitClip;
        public AudioSource audioSource;

        [Header("Velocity Controller")] public float controllerVelocityThreshold = 0.12f;

        private Transform _objectHolder;
        private EquationSpawner _equationSpawner;

        private Vector3 _contactStartPosition;
        private Vector3 _contactEndPoint;

        #region Unity Functions

        private void Start()
        {
            _objectHolder = GameObject.FindGameObjectWithTag(TagManager.BlockHolder).transform;

            GameObject equationSpawnerGameObject = GameObject.FindGameObjectWithTag(TagManager.EquationSpawner);
            _equationSpawner = equationSpawnerGameObject.GetComponent<EquationSpawner>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 velocity = SteamVR_Actions._default.Pose.velocity;
            if (velocity.sqrMagnitude < controllerVelocityThreshold)
            {
                return;
            }

            GameObject other = collision.gameObject;

            // Probably do this somewhere else. Not really sure...
            if (other.CompareTag(TagManager.StartBlock))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else
            {
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
                    PlayAudioClip(correctHitClip);

                    float startTime = cubeController.StartTime;
                    float timeDifference = Time.time - startTime;
                    EquationsAnalyticsManager.Instance.AddEquationToList(cubeController.Equation, cubeController.Answer, cubeController.IsCorrect, timeDifference);

                    cubeController.DestroyAllChildrenImmediate();

                    Debug.Log("Correct Answer Hit");
                }
                else if (other.CompareTag(TagManager.InCorrectAnswer))
                {
                    PlayAudioClip(wrongHitClip);

                    float startTime = cubeController.StartTime;
                    float timeDifference = Time.time - startTime;
                    EquationsAnalyticsManager.Instance.AddEquationToList(cubeController.Equation, cubeController.Answer, cubeController.IsCorrect, timeDifference);

                    Debug.Log("InCorrect Answer Hit");
                }

                cubeController.NotifyParentCollision();

                // Do this at the end as it resets the parent's in the Spawner
                _equationSpawner.SpawnNextEquation();
                _contactStartPosition = collision.contacts[0].point;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            GameObject other = collision.gameObject;

            if (other.CompareTag(TagManager.StartBlock))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.CorrectAnswer) || other.CompareTag(TagManager.InCorrectAnswer))
            {
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
                if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
                {
                    return;
                }

                if (!cubeController.HasSwordCollided() || !cubeController.HasParentDetectedCollisions())
                {
                    return;
                }

                _contactEndPoint = collision.contacts[0].point;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Vector3 velocity = SteamVR_Actions._default.Pose.velocity;
            if (velocity.sqrMagnitude < controllerVelocityThreshold)
            {
                return;
            }

            GameObject other = collision.gameObject;

            if (other.CompareTag(TagManager.StartBlock))
            {
                SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                HomeSceneController.Instance.ActivateSceneSwitchCountDown();
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();

                bool isAnswerValid = _equationSpawner.ReduceBonusValueCheckAndActivateEnd(int.Parse(cubeController.Answer));
                if (isAnswerValid)
                {
                    PlayAudioClip(correctHitClip);
                    SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                }
                else
                {
                    cubeController.FallFlashBlock();
                }
            }
            else
            {
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
                if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
                {
                    return;
                }

                if (!cubeController.HasSwordCollided() || !cubeController.HasParentDetectedCollisions())
                {
                    return;
                }

                if (other.CompareTag(TagManager.CorrectAnswer))
                {
                    SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                }
                else
                {
                    cubeController.FallFlashBlock();
                }
            }
        }

        #endregion

        #region Utility Functions

        private void SliceCollidingGameObject(GameObject objectToSlice, Vector3 startPoint, Vector3 endPoint)
        {
            Instantiate(sparkEffectPrefab, endPoint, Quaternion.identity);

            Vector3 direction = endPoint - startPoint;
            Quaternion lookDirection = Quaternion.LookRotation(direction);

            GameObject[] slicedGameObjects = objectToSlice.SliceInstantiate(startPoint, lookDirection * Vector3.up);
            if (slicedGameObjects == null)
            {
                Debug.Log("Invalid Cutting of GameObjects");
                return;
            }

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

        private void PlayAudioClip(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        #endregion
    }
}