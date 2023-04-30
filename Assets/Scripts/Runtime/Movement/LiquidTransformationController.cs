using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Runtime.Input;
using qASIC;
using UnityEngine.Events;

namespace Game.Runtime.Movement
{
    public class LiquidTransformationController : MonoBehaviour
    {
        public enum State
        {
            Human,
            Liquid,
            Possessing,
        }

        public State TransformationState { get; set; }

        [Label("Animation")]
        [SerializeField] Animator anim;
        [SerializeField] string animMeltTrigger;
        [SerializeField] string animUnmeltTrigger;

        [Label("Input")]
        public List<MonoBehaviour> humanInputables;
        public List<MonoBehaviour> liquidInputables;
        [SerializeField] PlayerInputController playerInput;

        [Label("Rotation")]
        [SerializeField] Transform xAxis;
        [SerializeField] Transform yAxis;

        [Label("Possessing")]
        [SerializeField] CharacterController characterController;
        [SerializeField] Vector3 possessionCenter;
        [SerializeField] float possessionRadius = 1f;
        public UnityEvent OnPossess;
        public UnityEvent OnLeavePossession;

        private void Awake() =>
            ChangeTransformation(State.Human);

        private void Update()
        {
            var input = playerInput.GetPlayerInput();

            if (input.meltThisFrame)
            {
                ChangeTransformation(TransformationState switch
                {
                    State.Human => State.Liquid,
                    State.Liquid => State.Human,
                    State.Possessing => State.Human,
                    _ => State.Human,
                });
            }

            qDebug.DisplayValue(nameof(TransformationState), TransformationState);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + possessionCenter, possessionRadius);
        }

        public void ChangeTransformation(State state)
        {
            State previousState = TransformationState;
            TransformationState = state;

            switch (previousState, state)
            {
                //Liquid => Human
                case (State.Liquid, State.Human):
                    if (_target == null)
                    {
                        var hits = Physics.SphereCastAll(transform.position + possessionCenter, possessionRadius, Vector3.up, 0f);
                        foreach (var hit in hits)
                        {
                            var possessable = hit.transform.GetComponent<PossessableObject>();
                            if (possessable == null) continue;
                            Possess(possessable);
                            return;
                        }
                    }

                    anim.SetTrigger(animUnmeltTrigger);
                    break;
                //Human => Liquid
                case (State.Human, State.Liquid):
                    anim.SetTrigger(animMeltTrigger);
                    break;
                //Possessing => Human
                case (State.Possessing, State.Human):
                    LeavePossession();
                    break;
            }

            playerInput.inputablesList = TransformationState switch
            {
                State.Human => humanInputables,
                State.Liquid => liquidInputables,
                State.Possessing => new List<MonoBehaviour>(),
                _ => humanInputables,
            };
        }

        PossessableObject _target;
        public void Possess(PossessableObject possessable)
        {
            if (_target != null ||
                possessable == null) return;

            TransformationState = State.Possessing;
            playerInput.inputablesList = possessable.inputables;

            possessable.cameraXTransform.eulerAngles = new Vector3(possessable.cameraXTransform.eulerAngles.x,
                xAxis.eulerAngles.y,
                possessable.cameraXTransform.eulerAngles.z);

            possessable.cameraYTransform.eulerAngles = new Vector3(yAxis.eulerAngles.x,
                possessable.cameraYTransform.eulerAngles.y,
                possessable.cameraYTransform.eulerAngles.z);

            _target = possessable;
            _target.OnPossess.Invoke();
            OnPossess.Invoke();
        }

        public void LeavePossession()
        {
            if (_target == null) return;
            anim.SetTrigger(animUnmeltTrigger);

            characterController.enabled = false;
            transform.position = _target.transform.position;
            characterController.enabled = true;

            xAxis.eulerAngles = new Vector3(xAxis.eulerAngles.x,
                _target.cameraXTransform.eulerAngles.y,
                xAxis.eulerAngles.z);

            yAxis.eulerAngles = new Vector3(_target.cameraYTransform.eulerAngles.x,
                yAxis.eulerAngles.y,
                yAxis.eulerAngles.z);

            _target.OnLeave.Invoke();
            _target = null;
            OnLeavePossession.Invoke();
        }
    }
}
