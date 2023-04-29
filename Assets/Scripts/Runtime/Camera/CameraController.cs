using UnityEngine;

namespace Game.Runtime.Camera
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Singleton { get; private set; }

        private void Awake()
        {
            Singleton = this;
        }
    }
}