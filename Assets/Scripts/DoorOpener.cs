using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Animator doorAnimator;
    
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        if(CustomerSpawner.instance == null)
            Debug.Log("DoorOpener: CustomerSpawner.instance is null");
        // Try to subscribe immediately, but also set up a coroutine to retry if needed
        TrySubscribeToCustomerSpawner();
    }

    void Update()
    {
        // Keep trying to subscribe if the CustomerSpawner isn't ready yet
        if (!IsSubscribed())
        {
            TrySubscribeToCustomerSpawner();
        }
    }

    private bool IsSubscribed()
    {
        // Check if we're already subscribed by trying to get the event count
        // This is a simple way to check if we're listening
        return CustomerSpawner.instance != null && 
               CustomerSpawner.instance.onCustomerSpawned != null;
    }

    private void TrySubscribeToCustomerSpawner()
    {
        if (CustomerSpawner.instance != null)
        {
            Debug.Log("DoorOpener: Subscribing to CustomerSpawner.onCustomerSpawned");
            CustomerSpawner.instance.onCustomerSpawned.AddListener(OpenDoor);
        }
        else
        {
            Debug.LogWarning("DoorOpener: CustomerSpawner.instance is null, will retry in Update");
        }
    }

    void OpenDoor()
    {
        Debug.Log($"DoorOpener: Opening door for customer");
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsPaused", false);
        }
        else
        {
            Debug.LogError("DoorOpener: doorAnimator is null!");
        }
    }

    void PauseDoor()
    {
        Debug.Log("DoorOpener: Pausing door");
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsPaused", true);
        }
    }

    void OnDestroy()
    {
        // Clean up the listener when this object is destroyed
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.onCustomerSpawned.RemoveListener(OpenDoor);
        }
    }
}