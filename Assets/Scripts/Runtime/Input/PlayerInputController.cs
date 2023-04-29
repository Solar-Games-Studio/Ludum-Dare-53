using UnityEngine;
using qASIC.Input;
using System.Collections.Generic;

namespace Game.Runtime.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [BeginGroup("List")]
        [EndGroup]
        [SerializeField] List<MonoBehaviour> playerInputablesList;

        [BeginGroup("Input")]
        [Label("Movement")]
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

        private void Update()
        {
            var playerInput = new PlayerInput()
            {
                movement = i_movement.GetInputValue<Vector2>(),
                sprint = i_sprint.GetInput(),
                jump = i_jump.GetInput(),
                jumpThisFrame = i_jump.GetInputDown(),
                melt = i_melt.GetInput(),
                metlThidFrame = i_melt.GetInputDown(),

                shoot = i_shoot.GetInput(),
                shootThisFrame = i_shoot.GetInputDown(),
                target = i_target.GetInput(),
                itemNext = i_itemNext.GetInput(),
                itemPrevious = i_itemPrevious.GetInput(),

                zoom = i_zoom.GetInputValue<float>(),
                look = i_look.GetInputValue<Vector2>(),
            };

            foreach (var item in playerInputablesList)
                if (item is IInputable inputable)
                    inputable.HandleInput(playerInput);
        }
    }
}
