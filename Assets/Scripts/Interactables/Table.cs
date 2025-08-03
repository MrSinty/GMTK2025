using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : Interactable
{
    public ProductData _productData;

    // Start is called before the first frame update
    void Start()
    {
        IsInteractable = false;
        _productData = null;
    }

    public override void Interact()
    {
        ItemSlot itemSlot = GameObject.FindGameObjectWithTag("ItemSlot").GetComponent<ItemSlot>();
        if (_productData == null)
        {
            _productData = itemSlot.Get();
            GetComponent<SpriteRenderer>().sprite = _productData.GetSpriteForState(_productData.currentState);
            itemSlot.Set(null);
            return;
        }
        if (itemSlot.Get() == null)
        {
            itemSlot.Set(_productData);
            _productData = null;
            GetComponent<SpriteRenderer>().sprite = null;
            return;   
        }
        
        ProductData temp = itemSlot.Get();
        itemSlot.Set(_productData);
        _productData = temp;
        GetComponent<SpriteRenderer>().sprite = _productData.GetSpriteForState(_productData.currentState);
    }

    public new void Tint()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 0.5f, 1f);
    }


    public new void Untint()
    {
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
    }
}