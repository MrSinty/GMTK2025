using UnityEngine;
using UnityEngine.U2D;


public class TrashBin : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        IsInteractable = true;
    }

    public override void Interact()
    {
       GameObject.FindGameObjectWithTag("ItemSlot").GetComponent<ItemSlot>().Set(null);
    }
}