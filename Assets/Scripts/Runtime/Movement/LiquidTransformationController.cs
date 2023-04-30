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

        [Label("Possessing")]
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
                    anim.SetTrigger(animUnmeltTrigger);
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

            _target = possessable;
            _target.OnPossess.Invoke();
            OnPossess.Invoke();
        }

        public void LeavePossession()
        {
            if (_target == null) return;
            _target.OnLeave.Invoke();
            _target = null;
            OnLeavePossession.Invoke();
        }
    }
}
