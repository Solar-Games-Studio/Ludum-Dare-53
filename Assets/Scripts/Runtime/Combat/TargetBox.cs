using UnityEngine.Events;
using UnityEngine;

namespace Game.Runtime.Combat
{
    public class TargetBox : MonoBehaviour
    {
        public Vector3 center;
        public UnityEvent OnStartTargeting;
        public UnityEvent OnStopTargeting;

        private void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Target Box");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + center, 0.3f);
        }
    }
}