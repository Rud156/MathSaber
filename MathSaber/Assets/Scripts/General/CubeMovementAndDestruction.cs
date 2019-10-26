using Equations;
using UnityEngine;
using Utils;

namespace General
{
    public class CubeMovementAndDestruction : MonoBehaviour
    {
        public float movementSpeed = 7;

        private bool _hasParentDetectCollision;
        private EquationSpawner _equationSpawner;

        #region Unity Functions

        private void Update()
        {
            Vector3 pos = transform.position;
            pos.z += movementSpeed * Time.deltaTime;
            transform.position = pos;
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

        #endregion
    }
}