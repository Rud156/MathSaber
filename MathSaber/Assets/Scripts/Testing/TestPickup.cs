using UnityEngine;
using Valve.VR;

namespace Testing
{
    public class TestPickup : MonoBehaviour
    {
        public string targetTag;
        public Transform handTransform;
        public bool isLeftHand;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        private bool _isTargetInside;
        private GameObject _targetObject;

        #region Unity Functions

        private void Update()
        {
            if (!_isTargetInside)
            {
                return;
            }

            if (SteamVR_Actions.default_GrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand) && isLeftHand)
            {
                PickupObject();
            }

            else if (SteamVR_Actions.default_GrabGrip.GetStateDown(SteamVR_Input_Sources.RightHand) && !isLeftHand)
            {
                PickupObject();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                _isTargetInside = true;
                _targetObject = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                _isTargetInside = false;
                _targetObject = null;
            }
        }

        #endregion

        #region Utility Functions

        private void PickupObject()
        {
            _targetObject.transform.SetParent(handTransform);
            _targetObject.transform.localPosition = positionOffset;
            _targetObject.transform.localRotation = Quaternion.Euler(rotationOffset);

            _isTargetInside = false;
            _targetObject = null;
        }

        #endregion
    }
}