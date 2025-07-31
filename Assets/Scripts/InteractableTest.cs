using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
  

    // Start is called before the first frame update
    void Start()
    {
        IsInteractable = true;
        Debug.Log("Interactable Spawned");
    }

    public void Interact()
    {
        if (IsInteractable) 
            Debug.Log("Tolya pidor");
        //IsInteractable = false; one time use  
    }

    public bool IsInteractable { get; set; }
}