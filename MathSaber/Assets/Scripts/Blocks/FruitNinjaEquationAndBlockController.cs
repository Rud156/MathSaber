using General;
using UnityEngine;

namespace Blocks
{
    public class FruitNinjaEquationAndBlockController : EquationBlockController
    {
        [Header("Launch Data")] public float launchAngleRange;
        public float launchForce;

        private Rigidbody _rigidbody;

        #region Overridden Parent

        protected override void Start()
        {
            base.Start();

            _rigidbody = GetComponent<Rigidbody>();
            LaunchBlock();
        }

        protected override void UpdateBlockMovement()
        {
            // Don't dop anything here...
        }

        #endregion

        #region Utility Functions

        private void LaunchBlock()
        {
            float randomAngle = Random.Range(-launchAngleRange, launchAngleRange);
            Vector3 launchForceAmount = Vector3.up * launchForce + Vector3.right * randomAngle;

            _rigidbody.AddForce(launchForceAmount, ForceMode.Impulse);
        }

        #endregion
    }
}