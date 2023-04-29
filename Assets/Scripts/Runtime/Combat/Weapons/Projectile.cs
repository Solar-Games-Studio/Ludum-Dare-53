using UnityEngine;

namespace Game.Runtime.Combat.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float sphereRadius;
        [SerializeField] float destroyTime = 10f;
        public LayerMask layerMask;
        public float speed;

        float _shootTime;

        private void Awake()
        {
            _shootTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _shootTime >= destroyTime)
            {
                Destroy(gameObject);
                return;
            }

            HandleMovement();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }

        void HandleMovement()
        {
            var didHit = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed * Time.fixedDeltaTime, layerMask);

            transform.position += transform.forward * speed * Time.deltaTime;

            if (didHit)
                Destroy(gameObject);
        }
    }
}
