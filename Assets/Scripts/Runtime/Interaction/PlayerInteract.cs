using Game.Runtime.Input;
using Game.Runtime.Camera;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Runtime.Interaction
{
    public class PlayerInteract : MonoBehaviour, IInputable
    {
        [Label("Raycast")]
        [SerializeField] float maxDistance;
        [SerializeField] LayerMask layerMask;

        [Label("Events")]
        public UnityEvent OnSelectInteractable;
        public UnityEvent OnDeselectInteractable;

        IInteractable _target = null;

        private void FixedUpdate()
        {
            var trans = CameraController.Singleton.transform;

            bool didHit = Physics.Raycast(transform.position, trans.forward, out RaycastHit hit, maxDistance, layerMask);

            if (!didHit) return;
            var interactable = hit.transform.GetComponent<IInteractable>();

            if (interactable == _target) return;
            if (interactable == null)
            {
                DeselectTarget();
                return;
            }

            SelectTarget(interactable);
        }

        void SelectTarget(IInteractable newTarget)
        {
            _target = newTarget;
            _target.IsSelected = true;
            OnSelectInteractable.Invoke();
        }

        void DeselectTarget()
        {
            if (_target == null) return;
            _target.IsSelected = false;
            _target = null;
            OnDeselectInteractable.Invoke();
        }

        public void HandleInput(PlayerInput input)
        {
            if (input.interactThisFrame && _target != null)
                Interact();
        }

        void Interact()
        {
            _target.Interact(this);
        }
    }
}
