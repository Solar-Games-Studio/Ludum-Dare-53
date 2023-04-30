using UnityEngine;
using Game.Runtime.Input;
using qASIC.SettingsSystem;
using Cinemachine;

namespace Game.Runtime.Camera
{
    public class PlayerCamera : MonoBehaviour, IInputable
    {
        [Label("Rotation")]
        [SerializeField] Transform xAxis;
        [SerializeField] Transform yAxis;
        [SerializeField][MinMaxSlider(-90f, 90f)] Vector2 yRotationLimit = new Vector2(-90f, 90f);

        [Label("Zoom")]
        [SerializeField] CinemachineVirtualCamera cam;
        [SerializeField][MinMaxSlider(0f, 10f)] Vector2 zoomRegion = new Vector2(1f, 5f);
        [SerializeField] float zoomSpeed = 0.1f;

        float _yRotation;
        CinemachineFramingTransposer _camTransposer;
        PlayerInput _input;

        public static float Sensitivity { get; set; } = 1f;
        public static float ZoomSpeed { get; set; } = 1f;
        public Vector2? OverrideRotation { get; set; } = null;

        private void Awake()
        {
            _camTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void OnEnable()
        {
            _yRotation = yAxis.eulerAngles.x > 180f ?
                yAxis.eulerAngles.x - 360f :
                yAxis.eulerAngles.x;
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

        public void HandleInput(PlayerInput input) =>
            _input = input;

        void HandleRotation()
        {
            float xMove = _input.look.x * Sensitivity;
            float xRotation = xAxis.eulerAngles.y + xMove;
            _yRotation += _input.look.y * Sensitivity;

            if (OverrideRotation.HasValue)
            {
                xRotation = OverrideRotation.Value.x;
                _yRotation = OverrideRotation.Value.y;
            }

            _yRotation = Mathf.Clamp(_yRotation, yRotationLimit.x, yRotationLimit.y);

            xAxis.eulerAngles = Vector3.up * xRotation;
            yAxis.eulerAngles = new Vector3(_yRotation, yAxis.eulerAngles.y, yAxis.eulerAngles.z);
        }

        void HandleZoom()
        {
            float zoom = _camTransposer.m_CameraDistance;
            zoom -= _input.zoom * zoomSpeed * Time.deltaTime * ZoomSpeed;
            zoom = Mathf.Clamp(zoom, zoomRegion.x, zoomRegion.y);
            _camTransposer.m_CameraDistance = zoom;
        }
    }
}