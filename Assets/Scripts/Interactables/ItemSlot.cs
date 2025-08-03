using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public float _speed = 3f;
    public float _amplitude = 0.001f;
    
    private ProductData _product;

    // Start is called before the first frame update
    void Start()
    {
        _product = ScriptableObject.CreateInstance<ProductData>();
        _product.productID = -1;
        _product.productName = "Empty";
        _product.currentState = IngredientState.Raw;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = transform.position.y + Mathf.Sin(Time.time * _speed) * _amplitude;
        transform.position = new Vector2(transform.position.x, newY);
    }
    
    public void Set(ProductData Item)
    {
        GameObject[] Tables = GameObject.FindGameObjectsWithTag("Table");
        _product = Item;
        if (!_product)
        {
            GetComponent<SpriteRenderer>().sprite = null;
            foreach (GameObject table in Tables)
            {
                
                if (table.GetComponent<Table>()._productData != null)
                    table.GetComponent<Table>().IsInteractable = false;
                table.GetComponent<Table>().IsInteractable = true;
            }
            return;
        }
        Vector3 pos = transform.position;
        pos.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        GetComponent<SpriteRenderer>().sprite = Item.GetSpriteForState(Item.currentState);
        foreach (GameObject table in Tables)
        {
            table.GetComponent<Table>().IsInteractable = true;
        }
    }

    public int GetProductID()
    {
       return _product.productID;
    }

    public Sprite GetProductSprite()
    {
        return _product.GetSpriteForState(_product.currentState);
    }

    public void Upgrade()
    {

    }

    public ProductData Get()
    {
        return _product;
    }
}