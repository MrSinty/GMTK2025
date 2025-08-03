using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookbookEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Image recipeQualityBackgroundImage;
    public Image dishImage;
    public Image baseImage;
    public Image mainImage;
    public Image sauceImage;

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
        
        if (recipeDescriptionText != null)
        {
            recipeDescriptionText.text = recipe.description;
        }

        if (recipeQualityBackgroundImage != null)
        {
            recipeQualityBackgroundImage.sprite = GetQualityBackgroundSprite(recipe.quality);
            recipeQualityBackgroundImage.color = Color.white; // Reset to white to show sprite properly
        }
        
        if (dishImage != null)
        {
            if (recipe.dishSprite != null)
            {
                dishImage.sprite = recipe.dishSprite;
                baseImage.sprite = recipe.baseSprite;
                mainImage.sprite = recipe.mainSprite;
                sauceImage.sprite = recipe.sauceSprite;

                dishImage.color = Color.white;
                baseImage.color = Color.white;
                mainImage.color = Color.white;
                sauceImage.color = Color.white;
            }
            else
            {
                dishImage.color = GetQualityColor(recipe.quality);
                baseImage.color = GetQualityColor(recipe.quality);
                mainImage.color = GetQualityColor(recipe.quality);
                sauceImage.color = GetQualityColor(recipe.quality);
            }
        }
    }
    
    private Color GetQualityColor(DishQuality quality)
    {
        return quality switch
        {
            DishQuality.Common => Color.white,
            DishQuality.Great => Color.green,
            DishQuality.Family => Color.yellow,
            DishQuality.Perfect => Color.magenta,
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