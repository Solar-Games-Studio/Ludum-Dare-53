using UnityEngine;
using UnityEngine.Events;

namespace Game.Runtime.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnInteract;

        public bool IsSelected { get; set; }

        public void Interact(PlayerInteract playerInteract)
        {
            OnInteract.Invoke();
        }
    }
}