using UnityEngine;

namespace Game.Runtime.Camera
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            if (CameraController.Singleton != null)
                transform.eulerAngles = CameraController.Singleton.transform.eulerAngles;
        }
    }
}