using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable _currentInteractable; //closest interactable

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
        if (other.TryGetComponent(out IInteractable interactable) && interactable.IsInteractable)
        {
            _currentInteractable = interactable;
            Debug.Log("Started interaction with " + _currentInteractable);
            //currentInteractable.onInteractStart();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == _currentInteractable)
        {
            //currentInteractable.onInteractEnd();
            Debug.Log("Ended interaction with " + _currentInteractable);
            _currentInteractable = null;
        }
    }
}