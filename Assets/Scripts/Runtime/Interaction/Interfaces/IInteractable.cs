namespace Game.Runtime.Interaction
{
    public interface IInteractable
    {
        public bool IsSelected { get; set; }

        public void Interact(PlayerInteract playerInteract);
    }
}