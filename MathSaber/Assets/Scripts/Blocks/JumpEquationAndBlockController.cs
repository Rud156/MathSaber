using UnityEngine;

namespace Blocks
{
    public class JumpEquationAndBlockController : EquationBlockController
    {
        [Header("Jumping Block Control")] public AnimationCurve movementLerpCurve;
        public float maxYToTravel;
        public float lerpMovementSpeed;

        private Vector3 _startPosition;
        private Vector3 _currentTargetPosition;
        private float _currentMovementLerpAmount;

        #region Overriden Parent

        protected override void Start()
        {
            base.Start();

            _startPosition = transform.position;
            _currentTargetPosition = transform.position + Vector3.up * maxYToTravel;
            _currentMovementLerpAmount = 0;
        }

        protected override void UpdateBlockMovement()
        {
            _currentMovementLerpAmount += lerpMovementSpeed * Time.deltaTime;

            transform.position = Vector3.LerpUnclamped(_startPosition, _currentTargetPosition, movementLerpCurve.Evaluate(_currentMovementLerpAmount));
        }

        #endregion
    }
}