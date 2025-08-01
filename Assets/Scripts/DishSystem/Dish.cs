using System.Collections.Generic;
using UnityEngine;

public class Dish
{
    private Dictionary<IngredientCategory, IngredientInstance> ingredients = new();

    public void AddIngredient(IngredientInstance instance)
    {
        if (ingredients.ContainsKey(instance.data.category))
        {
            Debug.Log("Can't add ingredient to dish, already contains this type of ingredient");
            return;
        }

        ingredients[instance.data.category] = instance;
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