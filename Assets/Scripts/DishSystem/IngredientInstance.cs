using UnityEngine;

public class IngredientInstance : MonoBehaviour
{
    public ProductData data;
    [SerializeField] private new SpriteRenderer renderer;

    [HideInInspector] public IngredientState state;
    [HideInInspector] public Sprite curSprite;
    [HideInInspector] public string ingredientName;

    private void Awake()
    {
        state = IngredientState.Raw;
        if (data.AlreadyCooked)
            state = IngredientState.Cooked;
        curSprite = data.GetSpriteForState(state);

        renderer.sprite = curSprite;
        ingredientName = data.productName;
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
        curSprite = data.GetSpriteForState(state);

        renderer.sprite = curSprite;

        Debug.Log($"[IngredientInstance] Updated sprite to {curSprite.name} for {data.productName} with state {state}");
    }

    public void HideSprite()
    {
        renderer.sprite = null;
    }

    public void ReturnSprite()
    {
        renderer.sprite = curSprite;
    }
}
