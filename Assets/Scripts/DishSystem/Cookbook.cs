using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cookbook : MonoBehaviour, IInteractable
{
    [Header("Cookbook Settings")]
    public List<CookbookEntry> specificRecipes = new List<CookbookEntry>();
    private List<CookbookEntry> allRecipes = new List<CookbookEntry>();

    [Header("UI References")]
    public CookbookUI cookbookUI;
    
    [Header("Interaction")]
    public string interactionPrompt = "Open Cookbook";

    public ProductDatabase allProducts;

    public bool IsInteractable { get; set; } = true; // Start as false, will be set to true when seated
    
    private static Cookbook instance;
    public static Cookbook Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Cookbook>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeRecipes();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeRecipes()
    {
        List<ProductData> baseProd = new List<ProductData>();
        List<ProductData> mainProd = new List<ProductData>();
        List<ProductData> sauceProd = new List<ProductData>();

        foreach (var product in allProducts.allProducts)
        {
            switch (product.category)
            {
                case IngredientCategory.Base:
                    baseProd.Add(product);
                    break;
                case IngredientCategory.Main:
                    mainProd.Add(product);
                    break;
                case IngredientCategory.Sauce:
                    sauceProd.Add(product);
                    break;
                default:
                    break;
            }
        }

        foreach (var recipe in specificRecipes)
        {
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        if (i * 100 + j * 10 + k == recipe.dishID)
                        {
                            allRecipes.Add(recipe);
                        }
                        else
                        {
                            CookbookEntry entry = ScriptableObject.CreateInstance<CookbookEntry>();
                            allRecipes.Add(entry.SetupEntry(i * 100 + j * 10 + k,
                                MakeDishName(baseProd[i - 1], mainProd[j - 1], sauceProd[k - 1]),
                                RandomString(20, 25),
                                DishQuality.Common,
                                baseProd[i - 1].GetSpriteForState(IngredientState.Cooked),
                                mainProd[j - 1].GetSpriteForState(IngredientState.Cooked),
                                sauceProd[k - 1].GetSpriteForState(IngredientState.Cooked)
                                )
                                );
                        }
                    }
                }
            }
        }

        //// Common dishes (basic combinations)
        //allRecipes.Add(new CookbookEntry(600, "Simple Rice Bowl", "A basic rice bowl with simple ingredients", DishQuality.Common));
        //allRecipes.Add(new CookbookEntry(700, "Basic Fish Dish", "Simple fish preparation", DishQuality.Common));

        //// Great dishes (better combinations)
        //allRecipes.Add(new CookbookEntry(650, "Chicken Rice Bowl", "A delicious bowl of rice topped with tender chicken", DishQuality.Great));
        //allRecipes.Add(new CookbookEntry(750, "Spicy Fish Rice", "Rice served with flaky fish and a kick of spicy sauce", DishQuality.Great));

        //// Family dishes (complex combinations)
        //allRecipes.Add(new CookbookEntry(680, "Family Feast Rice", "A hearty rice dish perfect for family gatherings", DishQuality.Family));
        //allRecipes.Add(new CookbookEntry(780, "Celebration Fish Bowl", "A special fish dish for celebrations", DishQuality.Family));

        //// Perfect dishes (masterpiece combinations)
        //allRecipes.Add(new CookbookEntry(690, "Master Chef's Rice", "A masterpiece of culinary art", DishQuality.Perfect));
        //allRecipes.Add(new CookbookEntry(790, "Legendary Fish Feast", "A legendary dish that will be remembered", DishQuality.Perfect));

        //allRecipes[0].isUnlocked = true;
        //allRecipes[1].isUnlocked = true;
        //allRecipes[2].isUnlocked = true;
        //allRecipes[3].isUnlocked = true;
        //allRecipes[4].isUnlocked = true;
    }

    public void CheckAndUnlockRecipe(Dish dish)
    {
        int dishID = dish.GetDishID();
        
        foreach (var recipe in allRecipes)
        {
            if (recipe.dishID == dishID && !recipe.isUnlocked)
            {
                recipe.dishName = dish.dishName;
                recipe.description = RandomString(15, 20);
                recipe.baseSprite = dish.baseSprite.sprite;
                recipe.mainSprite = dish.mainSprite.sprite;
                recipe.sauceSprite = dish.sauceSprite.sprite;
                recipe.Unlock();
                Debug.Log($"New recipe unlocked: {recipe.dishName} ({recipe.quality})!");
                
                // Notify UI if available
                if (cookbookUI != null)
                {
                    cookbookUI.OnRecipeUnlocked(recipe);
                }
                break;
            }
        }
    }

    public string MakeDishName(ProductData baseIng, ProductData mainIng, ProductData sauceIng)
    {
        string dishName = string.Empty;

        dishName += baseIng.productName + "ed ";
        dishName += mainIng.productName + " with ";
        dishName += sauceIng.productName + " sauce";

        return dishName;
    }

    public List<CookbookEntry> GetRecipesByQuality(DishQuality quality)
    {
        return allRecipes.Where(recipe => recipe.quality == quality && recipe.isUnlocked).ToList();
    }

    public List<CookbookEntry> GetAllUnlockedRecipes()
    {
        return allRecipes.Where(recipe => recipe.isUnlocked).ToList();
    }

    public int GetUnlockedCount()
    {
        return allRecipes.Count(recipe => recipe.isUnlocked);
    }

    public int GetTotalCount()
    {
        return allRecipes.Count;
    }

    public string RandomString(int minCharAmount, int maxCharAmount)
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz";

        string newString = string.Empty;
        int charAmount = Random.Range(minCharAmount, maxCharAmount);
        for (int i = 0; i < charAmount; i++)
        {
            newString += glyphs[Random.Range(0, glyphs.Length)];
        }

        return newString;
    }

    // IInteractable implementation
    public void Interact()
    {
        if (cookbookUI != null)
        {
            cookbookUI.OpenCookbook();
        }
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
} 