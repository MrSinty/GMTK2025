using System.Collections.Generic;
using UnityEngine;

public class Dish : PickUp
{
    public SpriteRenderer baseSprite;
    public SpriteRenderer mainSprite;
    public SpriteRenderer sauceSprite;

    private Dictionary<IngredientCategory, IngredientInstance> ingredients = new();

    private void Start()
    {
        var ingreds = GetComponentsInChildren<IngredientInstance>();
        foreach (var ingred in ingreds)
        {
            AddIngredient(ingred);
        }
    }

    public void AddIngredient(IngredientInstance instance)
    {
        if (ingredients.ContainsKey(instance.data.category))
        {
            Debug.Log("Can't add ingredient to dish, already contains this type of ingredient");
            return;
        }
        
        ingredients[instance.data.category] = instance;

        switch (instance.data.category)
        {
            case IngredientCategory.Base:
                baseSprite.sprite = instance.curSprite;
                break;
            case IngredientCategory.Main:
                mainSprite.sprite = instance.curSprite;
                break;
            case IngredientCategory.Sauce:
                sauceSprite.sprite = instance.curSprite;
                break;
            default:
                Debug.Log("No ingredient category was given!");
                return;
        }

        instance.HideSprite();
    }

    public int GetDishID()
    {
        int totalID = 0;
        foreach (var pair in ingredients)
        {
            totalID += pair.Value.GetActualID();
        }
        return totalID;
    }

    public bool IsFullyReady()
    {
        return ingredients.Count == 3 && !(GetDishID() >= 1000);
    }

    public bool MatchesClientOrder(int requestedID)
    {
        return IsFullyReady() && GetDishID() == requestedID;
    }

    public MatchResult CompareToClientOrder(int requestedID)
    {
        int currentID = GetDishID();
        if (!IsFullyReady()) return MatchResult.Bad;
        if (currentID == requestedID) return MatchResult.Full;
        if (currentID % 10 == requestedID % 10 
            || (currentID / 10) % 10 == (requestedID / 10) % 10 
            || ((currentID / 10) / 10) % 10 == ((requestedID / 10) / 10) % 10)
            return MatchResult.Partial;
        return MatchResult.Bad;
    }

    
}

public enum MatchResult
{
    Full,
    Partial,
    Bad
}