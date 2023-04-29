using UnityEngine;

namespace Game.Runtime.Camera
{
    public class ChangeCursorState : MonoBehaviour
    {
        public bool locked = true;

        CursorLock cursorLock;

        private void Awake()
        {
            cursorLock = new CursorLock(locked);
        }

        public void ChangeLockedState(bool locked)
        {
            this.locked = locked;
            cursorLock.ChangeState(locked);
        }

        public void ChangeUnlockedState(bool unlocked)
        {
            locked = !unlocked;
            cursorLock.ChangeState(!unlocked);
        }
    }
}