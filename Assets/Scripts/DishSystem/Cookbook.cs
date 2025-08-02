using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cookbook : MonoBehaviour, IInteractable
{
    [Header("Cookbook Settings")]
    public List<CookbookEntry> allRecipes = new List<CookbookEntry>();
    
    [Header("UI References")]
    public CookbookUI cookbookUI;
    
    [Header("Interaction")]
    public string interactionPrompt = "Open Cookbook";

    public bool IsInteractable { get; set; } = false; // Start as false, will be set to true when seated
    
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
        // Common dishes (basic combinations)
        allRecipes.Add(new CookbookEntry(600, "Simple Rice Bowl", "A basic rice bowl with simple ingredients", DishQuality.Common));
        allRecipes.Add(new CookbookEntry(700, "Basic Fish Dish", "Simple fish preparation", DishQuality.Common));
        
        // Great dishes (better combinations)
        allRecipes.Add(new CookbookEntry(650, "Chicken Rice Bowl", "A delicious bowl of rice topped with tender chicken", DishQuality.Great));
        allRecipes.Add(new CookbookEntry(750, "Spicy Fish Rice", "Rice served with flaky fish and a kick of spicy sauce", DishQuality.Great));
        
        // Family dishes (complex combinations)
        allRecipes.Add(new CookbookEntry(680, "Family Feast Rice", "A hearty rice dish perfect for family gatherings", DishQuality.Family));
        allRecipes.Add(new CookbookEntry(780, "Celebration Fish Bowl", "A special fish dish for celebrations", DishQuality.Family));
        
        // Perfect dishes (masterpiece combinations)
        allRecipes.Add(new CookbookEntry(690, "Master Chef's Rice", "A masterpiece of culinary art", DishQuality.Perfect));
        allRecipes.Add(new CookbookEntry(790, "Legendary Fish Feast", "A legendary dish that will be remembered", DishQuality.Perfect));
    }

    public void CheckAndUnlockRecipe(Dish dish)
    {
        if (!dish.IsFullyReady()) return;

        int dishID = dish.GetDishID();
        
        foreach (var recipe in allRecipes)
        {
            if (recipe.dishID == dishID && !recipe.isUnlocked)
            {
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