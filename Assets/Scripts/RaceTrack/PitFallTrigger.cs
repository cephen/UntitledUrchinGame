using UnityEngine;
using Unity.Logging;

namespace UrchinGame
{
    public class PitFallTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.GetComponent<PlayerTag>()) {
                Log.Debug($"{other.gameObject.name} fell into the pit!");
            }
        }
    }
}
