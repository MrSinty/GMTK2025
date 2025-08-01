using UnityEngine;

public class IngredientInstance
{
    public ProductData data;
    public IngredientState state;

    public IngredientInstance(ProductData data)
    {
        this.data = data;
        this.state = IngredientState.Raw;
    }

    public int GetActualID()
    {
        int id = data.productID;

        if (state == IngredientState.Raw)
            id += 1000;

        return id;
    }

    public bool IsFullyPrepared()
    {
        if (data.category == IngredientCategory.Main)
            return state == IngredientState.CookedAndSliced;

        return state == IngredientState.Cooked;
    }

    public void Cook()
    {
        if (state == IngredientState.Raw)
        {
            state = IngredientState.Cooked;
            UpdateVisual();
        }
    }

    public void Slice()
    {
        if (data.category == IngredientCategory.Main && state == IngredientState.Cooked)
        {
            state = IngredientState.CookedAndSliced;
            UpdateVisual();
        }
    }

    public void UpdateVisual()
    {
        var newSprite = data.GetSpriteForState(state);

        // TODO: Make UI logic to change sprites when ingredient is cooked
        Debug.Log($"[IngredientInstance] Updated sprite to {newSprite.name} for {data.productName} with state {state}");
    }
}
