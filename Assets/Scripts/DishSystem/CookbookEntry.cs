using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Ingredients/Recipe")]
public class CookbookEntry : ScriptableObject
{
    public int dishID;
    public string dishName;
    public string description;
    public DishQuality quality;
    public Sprite dishSprite;
    public Sprite baseSprite;
    public Sprite mainSprite;
    public Sprite sauceSprite;

    public bool isUnlocked;

    public CookbookEntry(int dishID, string dishName, string description, DishQuality quality, Sprite baseSprite, Sprite mainSprite, Sprite sauceSprite)
    {
        this.dishID = dishID;
        this.dishName = dishName;
        this.description = description;
        this.quality = quality;
        this.isUnlocked = false;
        this.baseSprite = baseSprite;
        this.mainSprite = mainSprite;
        this.sauceSprite = sauceSprite;
    }

    public CookbookEntry SetupEntry(int dishID, string dishName, string description, DishQuality quality, Sprite baseSprite, Sprite mainSprite, Sprite sauceSprite)
    {
        this.dishID = dishID;
        this.dishName = dishName;
        this.description = description;
        this.quality = quality;
        this.isUnlocked = false;
        this.baseSprite = baseSprite;
        this.mainSprite = mainSprite;
        this.sauceSprite = sauceSprite;

        return this;
    }

    public void Unlock()
    {
        isUnlocked = true;
    }
}

public enum DishQuality
{
    Common,
    Great,
    Family,
    Perfect
} 