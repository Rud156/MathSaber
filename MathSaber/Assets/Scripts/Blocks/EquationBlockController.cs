using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blocks
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

        private int _questionIndex;
        private string _equation;
        private string _answer;
        private string _cubeAnswer;
        private bool _isCorrect;
        private float _startTime;

        private bool _isFlashOn;
        private int _currentFlashCount;
        private float _currentTimeBetweenFlash;
        private Material _flashMaterial;
        private static readonly int EmissionColorParam = Shader.PropertyToID("_EmissionColor");

        public delegate void BlockDestroyed(EquationBlockController equationBlockController);

        public BlockDestroyed OnBlockDestroyed;

        private enum BlockStatus
        {
            MovementMode,
            BlockFallMode,
            BlockFlashMode,
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

                case BlockStatus.BlockFallMode:
                    break;

                case BlockStatus.BlockFlashMode:
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

        public void MakeOthersFall() => _parentBlockController.MakeAllBlocksFall();

        public void MakeOthersFlashFall() => _parentBlockController.MakeAllBlocksFlashFall();

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

        #region Question Data

        public void SetEquationStatus(int questionIndex, string equation, string answer, string cubeAnswer, bool isCorrect)
        {
            _questionIndex = questionIndex;
            _equation = equation;
            _answer = answer;
            _cubeAnswer = cubeAnswer;
            _isCorrect = isCorrect;
        }

        public int QuestionIndex => _questionIndex;

        public string Equation => _equation;

        public string Answer => _answer;

        public string CubeAnswer => _cubeAnswer;

        public bool IsCorrect => _isCorrect;

        public float StartTime => _startTime;

        #endregion

        public void FallBlock()
        {
            if (_blockStatus == BlockStatus.BlockFlashMode || _blockStatus == BlockStatus.BlockFallMode)
            {
                return;
            }

            Debug.Log("Falling Blocks");

            Rigidbody rigidBodyController = gameObject.GetComponent<Rigidbody>();
            if (rigidBodyController == null)
            {
                rigidBodyController = gameObject.AddComponent<Rigidbody>();
            }

            rigidBodyController.useGravity = true;
            rigidBodyController.constraints = RigidbodyConstraints.None;
            rigidBodyController.velocity = Random.onUnitSphere * forceAmount;

            float randomTorque = Random.Range(minTorqueAmount, maxTorqueAmount);
            rigidBodyController.AddTorque(randomTorque * Vector3.one, ForceMode.Impulse);

            SetBlockStatus(BlockStatus.BlockFallMode);
        }

        public void FallFlashBlock()
        {
            if (_blockStatus == BlockStatus.BlockFlashMode || _blockStatus == BlockStatus.BlockFallMode)
            {
                return;
            }

            FallBlock();

            Debug.Log("Flash Block");


            _isFlashOn = false;
            _currentTimeBetweenFlash = timeBetweenFlash;
            _currentFlashCount = totalFlashCount;
            _flashMaterial.SetColor(EmissionColorParam, normalColor);

            SetBlockStatus(BlockStatus.BlockFlashMode);
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