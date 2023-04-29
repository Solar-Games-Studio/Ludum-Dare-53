namespace Game.Runtime.Camera
{
    public class CursorLock
    {
        public CursorLock()
        {
            CursorManager.AddLock(this);
        }

        public CursorLock(bool locked)
        {
            Locked = locked;
            CursorManager.AddLock(this);
        }

        public bool Locked { get; private set; }

        public void ChangeState(bool locked)
        {
            Locked = locked;
            CursorManager.Refresh();
        }
    }
}