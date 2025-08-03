using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ingredients/Product")]
public class ProductData : ScriptableObject
{
    public int productID;
    public string productName;
    public IngredientCategory category;
    public bool AlreadyCooked;
    public IngredientState currentState = IngredientState.Raw;
    
    [Header("Sprites for states")]
    public Sprite rawIcon;
    public Sprite cookedIcon;
    public Sprite slicedIcon;

    public Sprite GetSpriteForState(IngredientState state)
    {
        return state switch
        {
            IngredientState.Raw => rawIcon,
            IngredientState.Sliced => slicedIcon,
            IngredientState.Cooked => cookedIcon,
            _ => rawIcon
        };
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(ProductData))]
    class ProductDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ProductData self = (ProductData)target;
            serializedObject.Update();
            if (self.category == IngredientCategory.Main)
                DrawDefaultInspector();
            else
            {
                DrawPropertiesExcluding(serializedObject, "slicedIcon");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}

public enum IngredientCategory
{
    Base,
    Main,
    Sauce
}

public enum IngredientState
{
    Raw,
    Sliced,
    Cooked
}
