using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CookbookEntry
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

    public CookbookEntry(int dishID, string dishName, string description, DishQuality quality)
    {
        this.dishID = dishID;
        this.dishName = dishName;
        this.description = description;
        this.quality = quality;
        this.isUnlocked = false;
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