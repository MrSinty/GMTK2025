using UnityEngine;

public class IngredientInstance
{
    public ProductData data;
    public IngredientState state;

    public IngredientInstance(ProductData data)
    {
        this.data = data;
        this.state = IngredientState.Raw;
        if (data.category == IngredientCategory.Sauce)
            this.state = IngredientState.Cooked;
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
        return state == IngredientState.Cooked;
    }

    public void Cook()
    {
        if (state == IngredientState.Cooked)
        {
            Debug.Log("Can't cook this!");
            return;
        }

        if (state == IngredientState.Sliced && data.category == IngredientCategory.Main)
        {
            state = IngredientState.Cooked;
            UpdateVisual();
        }
        if (data.category == IngredientCategory.Base)
        {
            state = IngredientState.Cooked;
            UpdateVisual();
        }
    }

    public void Slice()
    {
        if (data.category == IngredientCategory.Main && state == IngredientState.Raw)
        {
            state = IngredientState.Sliced;
            UpdateVisual();
        }
        else
        {
            Debug.Log("Can't slice this!");
            return;
        }
    }

    public void UpdateVisual()
    {
        var newSprite = data.GetSpriteForState(state);

        // TODO: Make UI logic to change sprites when ingredient is cooked
        Debug.Log($"[IngredientInstance] Updated sprite to {newSprite.name} for {data.productName} with state {state}");
    }
}
