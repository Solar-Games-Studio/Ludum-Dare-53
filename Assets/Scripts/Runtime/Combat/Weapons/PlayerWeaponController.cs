using UnityEngine;
using System.Collections.Generic;
using Game.Runtime.Combat.Weapons.Items;
using Game.Runtime.Camera;
using Game.Runtime.Input;
using qASIC;

namespace Game.Runtime.Combat.Weapons
{
    public class PlayerWeaponController : MonoBehaviour, IInputable
    {
        [Label("Inventory")]
        public List<InventoryWeapon> items = new List<InventoryWeapon>();

        [Label("Shooting")]
        [SerializeField] Transform shootPointPosition;
        [SerializeField] Transform shootPointRotation;

        [Label("Targeting")]
        [SerializeField] PlayerCamera playerCamera;
        [SerializeField] LayerMask targetRayLayerMask;
        [SerializeField] [Layer] int targetBoxLayerMask;
        [SerializeField] float maxTargetDistance = 20f;

        PlayerInput _input;

        private void Update()
        {
            if (_input.itemNext)
                ChangeWeapon(1);

            if (_input.itemPrevious)
                ChangeWeapon(-1);

            HandleWeapon();
            HandleTargeting();

            qDebug.DisplayValue("_selectedItemIndex", _selectedItemIndex);
            qDebug.DisplayValue("_currentTarget", _currentTarget?.name);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.6f, 0f, 1f);
            if (CameraController.Singleton != null)
            {
                var startPosition = CameraController.Singleton.transform;
                Gizmos.DrawLine(startPosition.position, startPosition.forward * maxTargetDistance);
            }
        }

        public void HandleInput(PlayerInput input) =>
            _input = input;

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
            bool shootInput = _input.shoot;
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

        TargetBox _currentTarget;
        /// <summary>Wait for the player to stop pressing the target button</summary>
        bool _waitForTargetStop;
        void HandleTargeting()
        {
            bool targetInput = _input.target;

            //Ignore if the player didn't stop targeting
            if (_waitForTargetStop)
            {
                if (!targetInput)
                    _waitForTargetStop = false;

                return;
            }

            switch (_currentTarget == null, targetInput)
            {
                //Start targeting
                case (true, true):
                    var target = DetectTarget();

                    StartTargeting(target);
                    break;
                //Targeting
                case (false, true):
                    var startPoint = CameraController.Singleton.transform;
                    var rotation = Quaternion.LookRotation(_currentTarget.transform.position + _currentTarget.center - startPoint.position).eulerAngles;

                    playerCamera.OverrideRotation = new Vector2(rotation.y,
                        rotation.x > 180f ? rotation.x - 360f : rotation.x);

                    if (DetectTarget() != _currentTarget)
                    {
                        StopTargeting();
                        _waitForTargetStop = true;
                    }
                    break;
                //Stop targeting
                case (false, false):
                    StopTargeting();
                    break;
            }
        }

        void StartTargeting(TargetBox target)
        {
            if (target == null)
                return;

            _currentTarget = target;
            _currentTarget.OnStartTargeting.Invoke();
        }

        void StopTargeting()
        {
            if (_currentTarget != null)
                _currentTarget.OnStopTargeting.Invoke();

            _currentTarget = null;
            playerCamera.OverrideRotation = null;
        }

        TargetBox DetectTarget()
        {
            var startPoint = CameraController.Singleton.transform;

            bool didHit = Physics.Raycast(startPoint.position, startPoint.forward, out RaycastHit hit, maxTargetDistance, targetRayLayerMask);
            if (!didHit ||
                hit.transform.gameObject.layer != targetBoxLayerMask)
                return null;

            return hit.transform.GetComponent<TargetBox>();
        }

        [System.Serializable]
        public class InventoryWeapon
        {
            public Weapon weapon;
            public int ammo;
        }
    }
}
