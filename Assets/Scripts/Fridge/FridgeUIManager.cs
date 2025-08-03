using UnityEngine;

public class FridgeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject fridgeUIRoot;
    [SerializeField] private ProductDatabase database;
    [SerializeField] private Transform contentParent; // content in scroll view
    [SerializeField] private GameObject productUIPrefab;

    private void OnEnable()
    {
        fridgeUIRoot.SetActive(false);
        PopulateFridge();
    }

    private void PopulateFridge()
    {

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var product in database.allProducts)
        {
            var uiObj = Instantiate(productUIPrefab, contentParent);
            var ui = uiObj.GetComponent<ProductUI>();
            ui.Setup(product);
        }
    }

    public void CloseUI()
    {
        fridgeUIRoot.SetActive(false);
    }

    public void OpenUI()
    {
        fridgeUIRoot.SetActive(true);
    }
}