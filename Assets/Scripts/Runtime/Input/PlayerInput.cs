using UnityEngine;

namespace Game.Runtime.Input
{
    public class PlayerInput
    {
        public Vector2 movement;
        public bool sprint;
        public bool jump;
        public bool jumpThisFrame;
        public bool melt;
        public bool metlThidFrame;

        public bool shoot;
        public bool shootThisFrame;
        public bool target;
        public bool itemNext;
        public bool itemPrevious;

        public float zoom;
        public Vector2 look;
    }
}