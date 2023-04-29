using qASIC;
using qASIC.Input;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Game.Runtime.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private static bool _noclip = false;
        public static bool Noclip
        {
            get => _noclip;
            set
            {
                _noclip = value;
                OnChangeNoclip?.Invoke();
            }
        }

        public static float SpeedMultiplier { get; set; } = 1f;

        public static bool IsGround { get; private set; }
        public static bool IsGroundPrevious { get; private set; }
        public float GravityVelovity { get; private set; }

        public static event Action OnChangeNoclip;

        [Label("Assign")]
        [SerializeField] Transform followAxis;
        [SerializeField] Transform modelTransform;
        [SerializeField] CharacterController characterController;

        [Label("Rotation")]
        [SerializeField] bool rotateSmoothly = true;
        [HideIf(nameof(rotateSmoothly), false)] [SerializeField] float rotationSpeed = 10f;
        [HideIf(nameof(rotateSmoothly), false)] [SerializeField] float maxDifference = 60f;

        [Label("Movement")]
        [SerializeField] float speed = 6f;
        [SerializeField] float sprintSpeed = 15f;
        [SerializeField] float airSpeedMultiplier = 0.3f;
        [SerializeField] float noclipSpeed = 16f;

        [Label("Gravity")]
        [SerializeField] float gravity = 30f;
        [SerializeField] float groundVelocity = 2f;
        [SerializeField] float jumpHeight = 2f;
        [SerializeField] Vector3 groundPointOffset = new Vector3(0f, -0.6f, 0f);
        [SerializeField] float groundPointRadius = 0.5f;
        [SerializeField] Vector3 topPointOffset = new Vector3(0f, 0.6f, 0f);
        [SerializeField] float topPointRadius = 0.5f;
        [SerializeField] LayerMask layer;
        [SerializeField] float coyoteTime = 0.15f;
        [SerializeField] float jumpQueue = 0.2f;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_movement;
        [SerializeField] InputMapItemReference i_sprint;
        [SerializeField] InputMapItemReference i_jump;
        [SerializeField] InputMapItemReference i_melt;

        [Label("Events")]
        public UnityEvent OnLand;
        public UnityEvent OnJump;

        private void Awake()
        {
            OnChangeNoclip += RefreshNoclip;
        }

        private void Update()
        {
            var input = new PlayerInput()
            {
                movement = i_movement.GetInputValue<Vector2>(),
                sprint = i_sprint.GetInput(),
                jumpThisFrame = i_jump.GetInputDown(),
                jump = i_jump.GetInput(),
                meltThisFrame = i_melt.GetInputDown(),
                melt = i_melt.GetInput(),
            };

            HandleRotation(input);
            Move(input);

            qDebug.DisplayValue("Noclip", Noclip);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + groundPointOffset, groundPointRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + topPointOffset, topPointRadius);
        }

        private void RefreshNoclip()
        {
            characterController.enabled = !Noclip;
        }

        void Move(PlayerInput input)
        {
            var path = new Vector3();

            switch (Noclip)
            {
                case true:
                    path = GetNoclipPath(input);
                    break;
                case false:
                    path = GetNormalPath(input);
                    break;
            }

            switch (Noclip)
            {
                case true:
                    characterController.transform.position += path * Time.deltaTime;
                    break;
                case false:
                    characterController.Move(path * Time.deltaTime);
                    break;
            }


            _lastPath = path;
        }

        void HandleRotation(PlayerInput input)
        {
            if (input.movement.magnitude != 0f)
            {
                float additionalAngle = Vector3.SignedAngle(input.movement, Vector3.up, Vector3.forward);
                float desiredAngle = followAxis.eulerAngles.y + additionalAngle;

                var euler = modelTransform.eulerAngles;
                var difference = desiredAngle - euler.y;

                //I hate this with a passion
                if (difference >= 180f)
                    difference -= 360f;
                if (difference <= -180f)
                    difference += 360f;


                euler.y += rotateSmoothly ?
                    Mathf.Clamp(difference * rotationSpeed * Time.deltaTime, -maxDifference, maxDifference) :
                    difference;

                modelTransform.eulerAngles = euler;
            }
        }

        Vector3 _lastPath;

        Vector3 GetNormalPath(PlayerInput input)
        {
            Vector3 path = new Vector3();
            Vector3 inputPath = (followAxis.right * input.movement.x + followAxis.forward * input.movement.y).normalized;

            float gravityPath = GetGravityPath(input);

            switch (IsGround)
            {
                case true:
                    path = inputPath;
                    path *= input.sprint ? sprintSpeed : speed;
                    path *= SpeedMultiplier;
                    break;
                case false:
                    var currentSpeed = input.sprint ? sprintSpeed : speed;
                    path = Vector3.Lerp(_lastPath, inputPath * currentSpeed * SpeedMultiplier, Time.deltaTime * airSpeedMultiplier);
                    path.y = 0f;
                    //path = Vector3.ClampMagnitude(path, currentSpeed * SpeedMultiplier);
                    break;
            }

            path.y += gravityPath;

            return path;
        }

        Vector3 GetNoclipPath(PlayerInput input)
        {
            return (followAxis.right * input.movement.x + followAxis.forward * input.movement.y + //WASD movement
                ((input.jump ? 1f : 0f) - (input.melt ? 1f : 0f)) * Vector3.up) //Up and down
                .normalized * noclipSpeed * SpeedMultiplier; //Speed
        }

        float _lastGroundTime;
        bool _acceptCoyoteTime;
        float _lastJumpQueueTime = float.MinValue;

        float GetGravityPath(PlayerInput input)
        {
            IsGroundPrevious = IsGround;
            IsGround = CheckForGround();

            bool forceJump = false;
            if (IsGround && !IsGroundPrevious)
            {
                if (Time.time - _lastJumpQueueTime <= jumpQueue)
                    forceJump = true;

                _acceptCoyoteTime = true;
                _lastJumpQueueTime = 0f;
                OnLand.Invoke();
            }

            switch (IsGround)
            {
                case true:
                    _lastGroundTime = Time.time;
                    GravityVelovity = -groundVelocity;
                    break;
                case false:
                    if (input.jumpThisFrame)
                        _lastJumpQueueTime = Time.time;

                    GravityVelovity -= gravity * Time.deltaTime;

                    //Check if head touches a celling
                    if (GravityVelovity > 0 && Physics.CheckSphere(transform.position + topPointOffset, topPointRadius, layer))
                        GravityVelovity = 0f;
                    break;
            }

            if ((forceJump || //force jump
                (IsGround || CoyoteInRange() && _acceptCoyoteTime) && //coyote time
                input.jumpThisFrame))
            {
                OnJump?.Invoke();
                GravityVelovity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                _acceptCoyoteTime = false;
            }

            return GravityVelovity;
        }

        bool CoyoteInRange() =>
            Time.time - _lastGroundTime <= coyoteTime;

        public bool CheckForGround() =>
            Physics.CheckSphere(transform.position + groundPointOffset, groundPointRadius, layer);

        public struct PlayerInput
        {
            public Vector2 movement;
            public bool sprint;
            public bool jumpThisFrame;
            public bool jump;
            public bool meltThisFrame;
            public bool melt;
        }
    }
}