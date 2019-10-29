using UnityEngine;
using Utils;

namespace Scenes.HomeScene
{
    public class Scene : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Sword))
            {
                // TODO: Do something here...
            }
        }
    }
}