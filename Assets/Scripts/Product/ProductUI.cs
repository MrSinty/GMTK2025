using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    private ProductData productData;

    public void Setup(ProductData data)
    {
        productData = data;
        iconImage.sprite = data.icon;
        nameText.text = data.productName;
    }

    public void OnClick()
    {
        FridgeItemSelector.SelectItem(productData);
    }
}
