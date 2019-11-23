using UnityEngine;

namespace Blocks
{
    public class FruitNinjaEquationAndBlockController : EquationBlockController
    {
        [Header("Launch Data")] public float launchAngleRange;
        public float launchForce;

        private Rigidbody _rigidbody;
        private bool _initialized;

        #region Overridden Parent

        protected override void Start()
        {
            base.Start();
            Initialize();
        }

        protected override void UpdateBlockMovement()
        {
            // Don't do anything here...
        }

        #endregion

        #region External Functions

        public void LaunchBlock()
        {
            Initialize();

            float randomAngle = Random.Range(-launchAngleRange, launchAngleRange);
            Vector3 launchForceAmount = Vector3.up * launchForce + Vector3.right * randomAngle;

            _rigidbody.AddForce(launchForceAmount, ForceMode.Impulse);
        }

        #endregion

        #region Utility Functions

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _rigidbody = GetComponent<Rigidbody>();
        }

        #endregion
    }
}