using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Fridge/Product Database")]
public class ProductDatabase : ScriptableObject
{
    public List<ProductData> allProducts;
}
