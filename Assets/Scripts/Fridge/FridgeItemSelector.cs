using UnityEngine;
using System;

public static class FridgeItemSelector
{
    public static Action<ProductData> OnItemSelected;

    public static void SelectItem(ProductData product)
    {
        Debug.Log("Selected product: " + product.productName);

        OnItemSelected?.Invoke(product);    

        var ui = GameObject.FindObjectOfType<FridgeUIManager>();
        if (ui != null)
        {
            ui.CloseUI();
        }
    }
}