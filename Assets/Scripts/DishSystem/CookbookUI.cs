using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookbookUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cookbookPanel;
    public Transform recipeContainer;
    public GameObject recipeEntryPrefab;
    public Button closeButton;

    private List<CookbookEntry> allRecipes;
    private List<GameObject> currentEntries = new List<GameObject>();
    
    private void Start()
    {
        // Set up button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCookbook);
            
        // Hide the cookbook initially
        if (cookbookPanel != null)
            cookbookPanel.SetActive(false);
            
        // Get reference to cookbook
        if (Cookbook.Instance != null)
        {
            Cookbook.Instance.cookbookUI = this;
        }
    }
    
    public void OpenCookbook()
    {
        if (cookbookPanel != null)
        {
            cookbookPanel.SetActive(true);
            RefreshCookbook();
        }
    }
    
    public void CloseCookbook()
    {
        if (cookbookPanel != null)
            cookbookPanel.SetActive(false);
    }
    
    private void RefreshCookbook()
    {
        if (Cookbook.Instance == null) return;
        
        allRecipes = Cookbook.Instance.GetAllUnlockedRecipes();
        DisplayUnlockedRecipes();
    }
    
    private void DisplayUnlockedRecipes()
    {
        // Clear current entries
        foreach (var entry in currentEntries)
        {
            if (entry != null)
                Destroy(entry);
        }
        currentEntries.Clear();
        
        if (recipeContainer == null) return;
        
        foreach (var recipe in allRecipes)
        {
            CreateRecipeEntry(recipe);
        }
    }
    
    private void CreateRecipeEntry(CookbookEntry recipe)
    {
        if (recipeEntryPrefab == null || recipeContainer == null) return;
        
        GameObject entryObj = Instantiate(recipeEntryPrefab, recipeContainer);
        currentEntries.Add(entryObj);
        
        // Set up the recipe entry UI
        var entryUI = entryObj.GetComponent<CookbookEntryUI>();
        if (entryUI != null)
        {
            entryUI.SetupEntry(recipe, this);
        }
    }
    
    public void OnRecipeUnlocked(CookbookEntry recipe)
    {
        // This is called when a new recipe is unlocked
        Debug.Log($"Recipe unlocked in UI: {recipe.dishName} ({recipe.quality})!");
        
        // Refresh the cookbook if it's currently open
        if (cookbookPanel != null && cookbookPanel.activeSelf)
        {
            RefreshCookbook();
        }
        
        // Show unlock notification
        ShowUnlockNotification(recipe);
    }
    
    private void ShowUnlockNotification(CookbookEntry recipe)
    {
        // Create a simple notification
        Debug.Log($"🎉 New {recipe.quality} Recipe Discovered: {recipe.dishName}!");
    }
} 