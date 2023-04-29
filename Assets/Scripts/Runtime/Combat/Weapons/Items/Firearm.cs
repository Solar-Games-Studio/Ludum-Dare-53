using UnityEngine;

namespace Game.Runtime.Combat.Weapons.Items
{
    [CreateAssetMenu(fileName = "New Firearm", menuName = "Scriptable Objects/Weapons/Firearm")]
    public class Firearm : Weapon
    {
        [Label("Bullet")]
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] float projectileSpeed = 20f;
        [SerializeField] bool overrideProjectileLayer = true;
        [SerializeField] [DisableIf(nameof(overrideProjectileLayer), false)] LayerMask projectileLayerMask;

        public override void Shoot(ShootContext context)
        {
            var bullet = Instantiate(projectilePrefab);
            bullet.transform.SetPositionAndRotation(context.shootPointPosition.position, context.shootPointRotation.rotation);

            bullet.speed = projectileSpeed;
            if (overrideProjectileLayer)
                bullet.layerMask = projectileLayerMask;
        }
    }
}