using UnityEngine;
using System.Collections.Generic;
using Game.Runtime.Combat.Weapons.Items;
using qASIC.Input;
using qASIC;

namespace Game.Runtime.Combat.Weapons
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [Label("Inventory")]
        public List<InventoryWeapon> items = new List<InventoryWeapon>();

        [Label("Shooting")]
        [SerializeField] Transform shootPointPosition;
        [SerializeField] Transform shootPointRotation;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_shoot;
        [SerializeField] InputMapItemReference i_itemNext;
        [SerializeField] InputMapItemReference i_itemPrevious;

        private void Update()
        {
            if (i_itemNext.GetInputDown() || Input.mouseScrollDelta.y > 0f)
                ChangeWeapon(1);

            if (i_itemPrevious.GetInputDown() || Input.mouseScrollDelta.y < 0f)
                ChangeWeapon(-1);

            HandleWeapon();

            qDebug.DisplayValue("_selectedItemIndex", _selectedItemIndex);
        }

        float _equipTime;

        void ChangeWeapon(int amount)
        {
            _selectedItemIndex = (_selectedItemIndex + amount) % items.Count;
            if (_selectedItemIndex < 0)
                _selectedItemIndex += items.Count;

            _equipTime = Time.time;
        }

        int _selectedItemIndex;
        float _lastFireTime;
        /// <summary>Wait for the player to stop pressing the shoot button</summary>
        bool _waitForFireStop;

        void HandleWeapon()
        {
            bool shootInput = i_shoot.GetInput() || Input.GetMouseButton(0);
            InventoryWeapon currentWeapon = items.IndexInRange(_selectedItemIndex) ?
                items[_selectedItemIndex] :
                null;

            if (!shootInput)
            {
                _waitForFireStop = false;
                return;
            }

            //Ignore if the player didn't stop shooting
            if (_waitForFireStop)
                return;

            if (currentWeapon?.weapon == null)
                return;

            if (Time.time - _equipTime < currentWeapon.weapon.equipTime)
                return;

            if (Time.time - _lastFireTime < currentWeapon.weapon.fireRate)
                return;

            _lastFireTime = Time.time;
            if (!currentWeapon.weapon.automatic)
                _waitForFireStop = true;

            var context = new Weapon.ShootContext()
            {
                shootPointPosition = shootPointPosition,
                shootPointRotation = shootPointRotation,
            };

            currentWeapon.weapon.Shoot(context);
        }

        [System.Serializable]
        public class InventoryWeapon
        {
            public Weapon weapon;
            public int ammo;
        }
    }
}
