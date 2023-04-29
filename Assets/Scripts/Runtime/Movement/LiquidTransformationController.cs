using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Runtime.Input;
using qASIC;

namespace Game.Runtime.Movement
{
    public class LiquidTransformationController : MonoBehaviour, IInputable
    {
        public bool IsLiquid { get; set; }

        public List<MonoBehaviour> humanInputables;
        public List<MonoBehaviour> liquidInputables;
        [SerializeField] PlayerInputController playerInput;

        PlayerInput _input;

        public void HandleInput(PlayerInput input) =>
            _input = input;

        private void Awake() =>
            ChangeTransformation(false);

        private void Update()
        {
            if (_input.meltThisFrame)
                ChangeTransformation(!IsLiquid);

            qDebug.DisplayValue(nameof(IsLiquid), IsLiquid);
        }

        public void ChangeTransformation(bool isLiquid)
        {
            IsLiquid = isLiquid;
            playerInput.inputablesList = (IsLiquid ? liquidInputables : humanInputables)
                .Concat(new MonoBehaviour[] { this })
                .ToList();
        }
    }
}
