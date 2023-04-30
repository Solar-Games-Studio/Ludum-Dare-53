using UnityEngine;

namespace Game.Runtime.Camera
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] bool useX = true;
        [SerializeField] bool useY = true;
        [SerializeField] bool useZ = true;

        private void Update()
        {
            if (CameraController.Singleton == null) return;
            var target = CameraController.Singleton.transform.eulerAngles;
            transform.eulerAngles = new Vector3(useX ? target.x : transform.eulerAngles.x,
                useY ? target.y : transform.eulerAngles.y,
                useZ ? target.z : transform.eulerAngles.z);
        }
    }
}