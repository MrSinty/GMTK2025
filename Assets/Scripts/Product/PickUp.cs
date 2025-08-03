using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int ID;
    public Sprite Sprite;

    public PickUp(ProductData product)
    {
        ID = product.productID;
        Sprite = product.GetSpriteForState(product.currentState);
    }

    public PickUp(Dish dish)
    {
        ID = dish.GetDishID();
        Sprite = dish.baseSprite.sprite;
    }
    
    public PickUp()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}