using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ingredients/Product Database")]
public class ProductDatabase : ScriptableObject
{
    public List<ProductData> allProducts;
}
