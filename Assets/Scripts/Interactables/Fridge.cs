using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using UnityEngine;

public class Fridge : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void Interact()
    {
        GetComponentInChildren<FridgeUIManager>().OpenUI();
    }
}
