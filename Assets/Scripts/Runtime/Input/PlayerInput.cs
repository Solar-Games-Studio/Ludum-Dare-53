using UnityEngine;

namespace Game.Runtime.Input
{
    public struct PlayerInput
    {
        public Vector2 movement;
        public bool sprint;
        public bool jump;
        public bool jumpThisFrame;
        public bool melt;
        public bool meltThisFrame;

        public bool shoot;
        public bool shootThisFrame;
        public bool target;
        public bool itemNext;
        public bool itemPrevious;

        public float zoom;
        public Vector2 look;
    }
}