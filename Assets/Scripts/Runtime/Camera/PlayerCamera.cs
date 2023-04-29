using UnityEngine;
using qASIC.Input;
using qASIC.SettingsSystem;
using Cinemachine;

namespace Game.Runtime.Camera
{
    public class PlayerCamera : MonoBehaviour
    {
        [Label("Rotation")]
        [SerializeField] Transform xAxis;
        [SerializeField] Transform yAxis;
        [SerializeField][MinMaxSlider(-90f, 90f)] Vector2 yRotationLimit = new Vector2(-90f, 90f);

        [Label("Zoom")]
        [SerializeField] CinemachineVirtualCamera cam;
        [SerializeField][MinMaxSlider(0f, 10f)] Vector2 zoomRegion = new Vector2(1f, 5f);
        [SerializeField] float zoomSpeed = 0.1f;
        [SerializeField] float gamepadZoomSpeed = 0.1f;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_zoom;
        [SerializeField] InputMapItemReference i_look;

        float _yRotation;
        CinemachineFramingTransposer _camTransposer;

        public static float Sensitivity { get; set; } = 1f;
        public static float ZoomSpeed { get; set; } = 1f;

        private void Awake()
        {
            _camTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        void Update()
        {
            HandleRotation();
            HandleZoom();
        }

        [OptionsSetting("camera_sensitivity", 1f)]
        void SetSensitivity(float value)
        {
            Sensitivity = value;
        }

        [OptionsSetting("camera_zoom_speed", 1f)]
        void SetZoomSpeed(float value)
        {
            ZoomSpeed = value;
        }

        void HandleRotation()
        {
            Vector2 cameraInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) -
                i_look.GetInputValue<Vector2>();

            _yRotation += cameraInput.y * Sensitivity;
            _yRotation = Mathf.Clamp(_yRotation, yRotationLimit.x, yRotationLimit.y);
            float xMove = cameraInput.x * Sensitivity;

            yAxis.eulerAngles = new Vector3(_yRotation, yAxis.eulerAngles.y, yAxis.eulerAngles.z);
            xAxis.eulerAngles += Vector3.up * xMove;
        }

        void HandleZoom()
        {
            float zoom = _camTransposer.m_CameraDistance;
            zoom -= (Input.mouseScrollDelta.y * zoomSpeed +
                i_zoom.GetInputValue<float>() * gamepadZoomSpeed * Time.deltaTime) * ZoomSpeed;
            zoom = Mathf.Clamp(zoom, zoomRegion.x, zoomRegion.y);
            _camTransposer.m_CameraDistance = zoom;
        }
    }
}