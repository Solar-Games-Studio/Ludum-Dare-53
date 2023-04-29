using UnityEngine;

namespace Game.Runtime.Combat.Weapons.Items
{
    public abstract class Weapon : ScriptableObject
    {
        public string weaponName;
        [TextArea] public string weaponDescription;

        [Label("Time")]
        public float equipTime = 0.2f;
        
        [Space]
        [Tooltip("If the weapon isn't automatic the player will have to re-press the fire button")] public bool automatic;
        public float fireRate;

        public virtual void Shoot(ShootContext context) { }

        public struct ShootContext
        {
            public Transform shootPointPosition;
            public Transform shootPointRotation;
        }
    }
}