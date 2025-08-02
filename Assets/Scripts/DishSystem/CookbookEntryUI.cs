using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookbookEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeQualityText;
    public Image recipeQualityBackgroundImage;
    public Image recipeImage;

    [Header("Quality Background Sprites")]
    public Sprite commonBackgroundSprite;
    public Sprite greatBackgroundSprite;
    public Sprite familyBackgroundSprite;
    public Sprite perfectBackgroundSprite;

    private CookbookEntry recipe;
    private CookbookUI cookbookUI;
    
    public void SetupEntry(CookbookEntry recipe, CookbookUI ui)
    {
        this.recipe = recipe;
        this.cookbookUI = ui;
        
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        if (recipeNameText != null)
        {
            recipeNameText.text = recipe.dishName;
            recipeNameText.color = GetQualityColor(recipe.quality);
        }
        
        if (recipeQualityText != null)
        {
            recipeQualityText.text = recipe.quality.ToString();
            recipeQualityText.color = GetQualityColor(recipe.quality);
        }

        if (recipeQualityBackgroundImage != null)
        {
            recipeQualityBackgroundImage.sprite = GetQualityBackgroundSprite(recipe.quality);
            recipeQualityBackgroundImage.color = Color.white; // Reset to white to show sprite properly
        }
        
        if (recipeImage != null)
        {
            if (recipe.dishSprite != null)
            {
                recipeImage.sprite = recipe.dishSprite;
                recipeImage.color = Color.white;
            }
            else
            {
                recipeImage.color = GetQualityColor(recipe.quality);
            }
        }
    }
    
    private Color GetQualityColor(DishQuality quality)
    {
        return quality switch
        {
            DishQuality.Common => Color.white,
            DishQuality.Great => Color.green,
            DishQuality.Family => Color.blue,
            DishQuality.Perfect => Color.yellow,
            _ => Color.white
        };
    }

    private Sprite GetQualityBackgroundSprite(DishQuality quality)
    {
        return quality switch
        {
            DishQuality.Common => commonBackgroundSprite,
            DishQuality.Great => greatBackgroundSprite,
            DishQuality.Family => familyBackgroundSprite,
            DishQuality.Perfect => perfectBackgroundSprite,
            _ => commonBackgroundSprite
        };
    }
    
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
} 