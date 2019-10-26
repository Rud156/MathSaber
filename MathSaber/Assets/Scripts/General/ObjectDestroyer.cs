using UnityEngine;

namespace General
{
    public class ObjectDestroyer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}