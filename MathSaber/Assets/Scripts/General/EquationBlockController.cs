using System;
using Equations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General
{
    public class EquationBlockController : MonoBehaviour
    {
        [Header("Flashing")] public Color flashColor;
        public Color normalColor;
        public float emissionAmount;
        public float timeBetweenFlash = 0.2f;
        public int totalFlashCount = 3;
        public int materialIndex;

        [Header("Collision Effect")] public float forceAmount = 5;
        public float minTorqueAmount = 5;
        public float maxTorqueAmount = 20;

        private float _movementSpeed;

        private bool _hasParentDetectCollision;
        private bool _swordCollided;
        private ParentBlockController _parentBlockController;

        private string _equation;
        private string _answer;
        private bool _isCorrect;
        private float _startTime;

        private bool _isFlashOn;
        private int _currentFlashCount;
        private float _currentTimeBetweenFlash;
        private Material _flashMaterial;
        private static readonly int EmissionColorParam = Shader.PropertyToID("_EmissionColor");

        public  delegate  void BlockDestroyed(EquationBlockController equationBlockController);
        public BlockDestroyed OnBlockDestroyed;

        private enum BlockStatus
        {
            MovementMode,
            FlashMode,
            Dead
        }

        private BlockStatus _blockStatus;

        #region Unity Functions

        protected virtual void Start()
        {
            _startTime = Time.time;
            _flashMaterial = GetComponent<MeshRenderer>().materials[materialIndex];

            SetBlockStatus(BlockStatus.MovementMode);
        }

        private void Update()
        {
            switch (_blockStatus)
            {
                case BlockStatus.MovementMode:
                    UpdateBlockMovement();
                    break;

                case BlockStatus.FlashMode:
                    UpdateBlockFlashing();
                    break;

                case BlockStatus.Dead:
                    UpdateDeadState();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy() => OnBlockDestroyed?.Invoke(this);

        #endregion

        #region External Functions

        public void SetMovementSpeed(float movementSpeed)
        {
            _movementSpeed = movementSpeed;
        }

        public void NotifyParentCollision()
        {
            _parentBlockController.NotifyParentCollision();
            _swordCollided = true;
        }

        public void MakeOthersFlashFall() => _parentBlockController.MakeAllBlocksFall();

        public void SetParent(ParentBlockController parentBlockController) => _parentBlockController = parentBlockController;

        public void SetParentCollided() => _hasParentDetectCollision = true;

        public bool HasParentDetectedCollisions() => _hasParentDetectCollision;

        public bool HasSwordCollided() => _swordCollided;

        public void DestroyAllChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void SetEquationStatus(string equation, string answer, bool isCorrect)
        {
            _equation = equation;
            _answer = answer;
            _isCorrect = isCorrect;
        }

        public string Equation => _equation;

        public string Answer => _answer;

        public bool IsCorrect => _isCorrect;

        public float StartTime => _startTime;

        public void FallFlashBlock()
        {
            if (_blockStatus == BlockStatus.FlashMode)
            {
                return;
            }

            Debug.Log("Flash Block");

            Rigidbody rigidBodyController = gameObject.AddComponent<Rigidbody>();

            rigidBodyController.useGravity = true;
            rigidBodyController.constraints = RigidbodyConstraints.None;
            rigidBodyController.velocity = Random.onUnitSphere * forceAmount;

            float randomTorque = Random.Range(minTorqueAmount, maxTorqueAmount);
            rigidBodyController.AddTorque(randomTorque * Vector3.one, ForceMode.Impulse);

            _isFlashOn = false;
            _currentTimeBetweenFlash = timeBetweenFlash;
            _currentFlashCount = totalFlashCount;
            _flashMaterial.SetColor(EmissionColorParam, normalColor);

            SetBlockStatus(BlockStatus.FlashMode);
        }

        #endregion

        #region Utility Functions

        protected virtual void UpdateBlockMovement() => transform.Translate(Time.deltaTime * _movementSpeed * Vector3.forward);

        private void UpdateBlockFlashing()
        {
            _currentTimeBetweenFlash -= Time.deltaTime;
            if (_currentTimeBetweenFlash <= 0)
            {
                if (_isFlashOn)
                {
                    _currentFlashCount -= 1;
                    _currentTimeBetweenFlash = timeBetweenFlash;

                    _isFlashOn = false;
                    _flashMaterial.SetColor(EmissionColorParam, normalColor);
                }
                else
                {
                    _currentTimeBetweenFlash = timeBetweenFlash;

                    _isFlashOn = true;
                    _flashMaterial.SetColor(EmissionColorParam, flashColor * emissionAmount);
                }
            }

            if (_currentFlashCount <= 0)
            {
                SetBlockStatus(BlockStatus.Dead);
            }
        }

        private void UpdateDeadState()
        {
            // This is not really required but just an idle kind of state
        }

        private void SetBlockStatus(BlockStatus blockStatus) => _blockStatus = blockStatus;

        #endregion
    }
}