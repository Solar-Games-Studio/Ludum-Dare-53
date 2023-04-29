using UnityEngine.Events;
using UnityEngine;

namespace Game.Runtime.Combat
{
    public class TargetBox : MonoBehaviour
    {
        public enum Team
        {
            Friendly,
            Enemy,
        }

        public Vector3 center;
        public Team team = Team.Enemy;

        [Label("Events")]
        public UnityEvent OnStartTargeting;
        public UnityEvent OnStopTargeting;

        private void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Target Box");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + center, 0.1f);
        }
    }
}