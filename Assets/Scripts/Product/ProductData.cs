using UnityEngine;

[CreateAssetMenu(menuName = "Fridge/Product")]
public class ProductData : ScriptableObject
{
    public string productName;
    public Sprite icon;
    public int productID;
}
