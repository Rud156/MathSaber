using Blocks;
using Effects;
using Equations;
using EzySlice;
using Scenes.HomeScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplateProjects.Scenes.EndScene;
using Utils;
using Valve.VR;
using Valve.VR.InteractionSystem;
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

        [Header("Haptic")] public float hapticDuration;
        public float hapticAmplitude;
        public float hapticFrequency;

        [Header("Audio Effect")] public AudioClip correctHitClip;
        public AudioClip wrongHitClip;
        public AudioSource audioSource;

        [Header("Velocity Controller")] public float controllerVelocityThreshold = 0.12f;

        [Header("Sword Faking Controls")] public float deactivateSwordTime;

        private Hand _handController;

        private Transform _objectHolder;
        private EquationSpawnerBase _equationSpawnerBase;
        private LightFlasherManager _lightFlasherManager;

        private Vector3 _contactStartPosition;
        private Vector3 _contactEndPoint;

        private float _currentSwordDeactivationTime;

        #region Unity Functions

        private void Start()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex == 1 || buildIndex == 2 || buildIndex == 3)
            {
                InitializeDefaults();
            }

            _handController = GetComponentInParent<Hand>();
        }

        private void OnEnable() => SceneManager.sceneLoaded += HandleSceneLoaded;

        private void OnDestroy() => SceneManager.sceneLoaded -= HandleSceneLoaded;

        private void Update()
        {
            if (_currentSwordDeactivationTime > 0)
            {
                _currentSwordDeactivationTime -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 velocity = SteamVR_Actions._default.Pose.velocity;
            if (velocity.sqrMagnitude < controllerVelocityThreshold)
            {
                return;
            }

            _handController.TriggerHapticPulse(hapticDuration, hapticFrequency, hapticAmplitude);

            GameObject other = collision.gameObject;

            // Probably do this somewhere else. Not really sure...
            if (other.CompareTag(TagManager.StartBlock))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.NinjaStartBlock))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.MasterStart))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.RestartBlock))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                _contactStartPosition = collision.contacts[0].point;
            }
            else
            {
                if (_currentSwordDeactivationTime > 0)
                {
                    return;
                }

                // Don't do anything in start as it might be the case where the player
                // Enters very fast but they stop in the middle and 
                // Thus the ending would be broken as assets are destroyed
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
                if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
                {
                    return;
                }

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
            else if (other.CompareTag(TagManager.NinjaStartBlock))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.MasterStart))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.RestartBlock))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                _contactEndPoint = collision.contacts[0].point;
            }
            else if (other.CompareTag(TagManager.CorrectAnswer) || other.CompareTag(TagManager.InCorrectAnswer))
            {
                if (_currentSwordDeactivationTime > 0)
                {
                    return;
                }

                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();
                if (!cubeController || (!other.CompareTag(TagManager.CorrectAnswer) && !other.CompareTag(TagManager.InCorrectAnswer)))
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

            _handController.TriggerHapticPulse(hapticDuration, hapticFrequency, hapticAmplitude);

            GameObject other = collision.gameObject;

            if (other.CompareTag(TagManager.StartBlock))
            {
                PlayAudioClip(correctHitClip);

                SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                HomeSceneController.Instance.ActivateSceneSwitchCountDown(1);
            }
            else if (other.CompareTag(TagManager.NinjaStartBlock))
            {
                PlayAudioClip(correctHitClip);

                SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                HomeSceneController.Instance.ActivateSceneSwitchCountDown(2);
            }
            else if (other.CompareTag(TagManager.MasterStart))
            {
                PlayAudioClip(correctHitClip);

                SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                HomeSceneController.Instance.ActivateSceneSwitchCountDown(3);
            }
            else if (other.CompareTag(TagManager.RestartBlock))
            {
                PlayAudioClip(correctHitClip);
                SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                EndSceneController.Instance.ActiveSceneSwitchCountDown();
            }
            else if (other.CompareTag(TagManager.BonusAnswer))
            {
                EquationBlockController cubeController = other.GetComponent<EquationBlockController>();

                bool isAnswerValid = _equationSpawnerBase.ReduceBonusValueCheckAndActivateEnd(int.Parse(cubeController.Answer));
                if (isAnswerValid)
                {
                    PlayAudioClip(correctHitClip);
                    _lightFlasherManager.FlashAllLights();
                    SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                }
                else
                {
                    PlayAudioClip(wrongHitClip);
                    cubeController.FallFlashBlock();
                }
            }
            else
            {
                if (_currentSwordDeactivationTime > 0)
                {
                    return;
                }

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
                    _equationSpawnerBase.IncrementSpeed();

                    float startTime = cubeController.StartTime;
                    float timeDifference = Time.time - startTime;
                    EquationsAnalyticsManager.Instance.AddEquationToList(
                        cubeController.QuestionIndex, cubeController.Equation, cubeController.Answer, cubeController.CubeAnswer,
                        cubeController.IsCorrect, timeDifference);

                    cubeController.DestroyAllChildren();

                    Debug.Log("Correct Answer Hit");

                    _lightFlasherManager.FlashAllLights();
                    SliceCollidingGameObject(other, _contactStartPosition, _contactEndPoint);
                }
                else if (other.CompareTag(TagManager.InCorrectAnswer))
                {
                    PlayAudioClip(wrongHitClip);
                    _equationSpawnerBase.DecrementSpeed();

                    float startTime = cubeController.StartTime;
                    float timeDifference = Time.time - startTime;
                    EquationsAnalyticsManager.Instance.AddEquationToList(cubeController.QuestionIndex, cubeController.Equation, cubeController.Answer,
                        cubeController.CubeAnswer,
                        cubeController.IsCorrect, timeDifference);

                    Debug.Log("InCorrect Answer Hit");

                    cubeController.FallFlashBlock();
                }

                cubeController.NotifyParentCollision();
                cubeController.MakeOthersFall();
                _equationSpawnerBase.SpawnNextEquation();

                _currentSwordDeactivationTime = deactivateSwordTime;
            }
        }

        #endregion

        #region Utility Functions

        private void SliceCollidingGameObject(GameObject objectToSlice, Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 normalizedStart = new Vector3(startPoint.x, startPoint.y, startPoint.z);
            Vector3 normalizedEnd = new Vector3(endPoint.x, endPoint.y, startPoint.z);
            Vector3 middlePosition = (normalizedStart + normalizedEnd) / 2.0f;

            Instantiate(sparkEffectPrefab, middlePosition, Quaternion.identity);

            float angle = Mathf.Atan2(normalizedEnd.y - normalizedStart.y, normalizedEnd.x - normalizedStart.x) * Mathf.Rad2Deg;
            Vector3 center = (normalizedStart + normalizedEnd) / 2.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;

            GameObject[] slicedGameObjects = objectToSlice.SliceInstantiate(center, direction);
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
                Material[] materials = slicedGameObject.GetComponent<MeshRenderer>().materials;
                materials[totalMaterials - 1] = new Material(borderMaterial);
                slicedGameObject.GetComponent<MeshRenderer>().materials = materials;

                slicedGameObject.transform.SetParent(_objectHolder);
            }

            Destroy(objectToSlice);
        }

        private void PlayAudioClip(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            // There probably is a better way to do this.
            // Not sure at this point
            if (scene.buildIndex == 1 || scene.buildIndex == 2 || scene.buildIndex == 3)
            {
                InitializeDefaults();
            }
        }

        private void InitializeDefaults()
        {
            _objectHolder = GameObject.FindGameObjectWithTag(TagManager.BlockHolder)?.transform;

            GameObject equationSpawnerGameObject = GameObject.FindGameObjectWithTag(TagManager.EquationSpawner);
            _equationSpawnerBase = equationSpawnerGameObject.GetComponent<EquationSpawnerBase>();

            GameObject lightFlasherGameObject = GameObject.FindGameObjectWithTag(TagManager.LightFlasher);
            _lightFlasherManager = lightFlasherGameObject.GetComponent<LightFlasherManager>();
        }

        #endregion
    }
}