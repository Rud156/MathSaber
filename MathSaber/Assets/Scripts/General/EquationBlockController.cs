using Equations;
using UnityEngine;
using Utils;

namespace General
{
    public class EquationBlockController : MonoBehaviour
    {
        public float movementSpeed = 7;
        public float stopForSeconds = 3;

        private bool _hasParentDetectCollision;
        private EquationSpawner _equationSpawner;

        private bool _movementActive;

        private bool _stopPositionTargetCompleted;
        private Vector3 _stopPointPosition;
        private float _currentStopTimerLeft;

        #region Unity Functions

        private void Start()
        {
            _stopPointPosition = GameObject.FindGameObjectWithTag(TagManager.StopPointZ).transform.position;

            _currentStopTimerLeft = stopForSeconds;
            _movementActive = true;
        }

        private void Update()
        {
            if (_movementActive)
            {
                Vector3 pos = transform.position;
                pos.z += movementSpeed * Time.deltaTime;
                transform.position = pos;
            }

            if (transform.position.z >= _stopPointPosition.z && !_stopPositionTargetCompleted)
            {
                _movementActive = false;
                _currentStopTimerLeft -= Time.deltaTime;

                if (_currentStopTimerLeft <= 0)
                {
                    _stopPositionTargetCompleted = true;
                    _movementActive = true;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Wall))
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region External Functions

        public void NotifyParentCollision() => _equationSpawner.NotifyParentCollision();

        public void SetParent(EquationSpawner equationSpawner) => _equationSpawner = equationSpawner;

        public void SetParentCollided() => _hasParentDetectCollision = true;

        public bool HasParentDetectedCollisions() => _hasParentDetectCollision;

        public void DestroyAllChildrenImmediate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        #endregion
    }
}