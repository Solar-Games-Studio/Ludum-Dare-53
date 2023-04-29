using Game.Runtime.Combat.Weapons;
using UnityEngine;

namespace Game.Runtime.Combat.Enemies
{
    public class EnemyShoot : MonoBehaviour
    {
        [Label("Aiming")]
        [SerializeField] Enemy enemyBrain;
        [SerializeField] Transform playerRotation;

        [Label("Shooting")]
        [BeginGroup("Fire")]
        [SerializeField] Transform firePoint;
        [EndGroup]
        [SerializeField] float fireRate;

        [BeginGroup("Projectile")]
        [SerializeField] Projectile projectilePrefab;
        [EndGroup]
        [SerializeField] float projectileSpeed;

        private void Update()
        {
            if (enemyBrain.Target == null)
                return;

            HandleRotation();
            HandleShooting();
        }

        void HandleRotation()
        {
            var euler = playerRotation.eulerAngles;

            euler.y = Quaternion.LookRotation(enemyBrain.Target.transform.position + enemyBrain.Target.center - firePoint.position)
                .eulerAngles.y;

            playerRotation.eulerAngles = euler;
        }

        float _lastFireTime;
        void HandleShooting()
        {
            if (Time.time - _lastFireTime < fireRate)
                return;

            _lastFireTime = Time.time;
            var projectile = Instantiate(projectilePrefab);

            projectile.transform.SetPositionAndRotation(firePoint.transform.position, firePoint.transform.rotation);
            projectile.speed = projectileSpeed;
        }
    }
}