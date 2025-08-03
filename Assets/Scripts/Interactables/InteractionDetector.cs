using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    public class InteractionDetector : MonoBehaviour
    {
        private Interactable _currentInteractable; //closest interactable

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _currentInteractable?.Interact();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Interactable interactable) && interactable.IsInteractable)
            {
                if(_currentInteractable)
                    _currentInteractable.Untint();
                _currentInteractable = interactable;
                _currentInteractable.Tint();
                //currentInteractable.onInteractStart();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Interactable interactable) && interactable == _currentInteractable)
            {
                //currentInteractable.onInteractEnd();
                _currentInteractable.Untint();
                _currentInteractable = null;
            }
        }
    }
}