using UnityEngine;

namespace Game.Runtime.Camera
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class CameraController : MonoBehaviour
    {
        public static CameraController Singleton { get; private set; }

        private void Awake()
        {
            Singleton = this;
        }
    }
}