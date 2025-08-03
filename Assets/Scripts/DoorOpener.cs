using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Animator doorAnimator;
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        CustomerSpawner.instance.onCustomerSpawned.AddListener(OpenDoor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenDoor(int customerIndex)
    {
        Debug.Log("Opening door");
        doorAnimator.SetBool("IsPaused", false);
    }

    void PauseDoor()
    {
        doorAnimator.SetBool("IsPaused", true);
    }
}