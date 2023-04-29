using UnityEngine;
using qASIC.Input;
using System.Collections.Generic;

using UInput = UnityEngine.Input;

namespace Game.Runtime.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [BeginGroup("List")]
        [EndGroup]
        public List<MonoBehaviour> inputablesList;

        [BeginGroup("Movement")]
        [Label("Input")]
        [SerializeField] InputMapItemReference i_movement;
        [SerializeField] InputMapItemReference i_sprint;
        [SerializeField] InputMapItemReference i_jump;
        [SerializeField] InputMapItemReference i_melt;

        [Label("Combat")]
        [SerializeField] InputMapItemReference i_shoot;
        [SerializeField] InputMapItemReference i_target;
        [SerializeField] InputMapItemReference i_itemNext;
        [SerializeField] InputMapItemReference i_itemPrevious;

        [Label("Camera")]
        [SerializeField] InputMapItemReference i_zoom;
        [EndGroup]
        [SerializeField] InputMapItemReference i_look;

        PlayerInput _playerInput;
        public PlayerInput GetPlayerInput() =>
            _playerInput;

        private void Update()
        {
            _playerInput = new PlayerInput()
            {
                movement = i_movement.GetInputValue<Vector2>(),
                sprint = i_sprint.GetInput(),
                jump = i_jump.GetInput(),
                jumpThisFrame = i_jump.GetInputDown(),
                melt = i_melt.GetInput(),
                meltThisFrame = i_melt.GetInputDown(),

                shoot = i_shoot.GetInput() || UInput.GetMouseButton(0),
                shootThisFrame = i_shoot.GetInputDown() || UInput.GetMouseButtonDown(0),
                target = i_target.GetInput() || UInput.GetMouseButton(1),
                itemNext = i_itemNext.GetInput() || UInput.mouseScrollDelta.y > 0f,
                itemPrevious = i_itemPrevious.GetInput() || UInput.mouseScrollDelta.y < 0f,

                zoom = i_zoom.GetInputValue<float>(),
                look = new Vector2(UInput.GetAxis("Mouse X"), -UInput.GetAxis("Mouse Y")) -
                    i_look.GetInputValue<Vector2>(),
            };

            foreach (var item in inputablesList)
                if (item is IInputable inputable)
                    inputable.HandleInput(_playerInput);
        }
    }
}
