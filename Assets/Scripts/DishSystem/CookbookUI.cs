using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CookbookUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cookbookPanel;
    public Transform recipeContainer;
    public GameObject recipeEntryPrefab;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public Button closeButton;
    
    [Header("Quality Tabs")]
    public Button commonTabButton;
    public Button greatTabButton;
    public Button familyTabButton;
    public Button perfectTabButton;
    
    [Header("Recipe Detail Panel")]
    public GameObject recipeDetailPanel;
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public TextMeshProUGUI recipeQualityText;
    public Image recipeImage;
    
    private List<CookbookEntry> allRecipes;
    private List<GameObject> currentEntries = new List<GameObject>();
    private DishQuality currentQualityFilter = DishQuality.Common;
    
    private void Start()
    {
        // Set up button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCookbook);
            
        // Set up quality tab buttons
        if (commonTabButton != null)
            commonTabButton.onClick.AddListener(() => SetQualityFilter(DishQuality.Common));
        if (greatTabButton != null)
            greatTabButton.onClick.AddListener(() => SetQualityFilter(DishQuality.Great));
        if (familyTabButton != null)
            familyTabButton.onClick.AddListener(() => SetQualityFilter(DishQuality.Family));
        if (perfectTabButton != null)
            perfectTabButton.onClick.AddListener(() => SetQualityFilter(DishQuality.Perfect));
            
        // Hide the cookbook initially
        if (cookbookPanel != null)
            cookbookPanel.SetActive(false);
        if (recipeDetailPanel != null)
            recipeDetailPanel.SetActive(false);
            
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
        if (recipeDetailPanel != null)
            recipeDetailPanel.SetActive(false);
    }
    
    private void RefreshCookbook()
    {
        if (Cookbook.Instance == null) return;
        
        allRecipes = Cookbook.Instance.GetAllUnlockedRecipes();
        UpdateProgressText();
        DisplayRecipesByQuality(currentQualityFilter);
    }
    
    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            int unlocked = Cookbook.Instance.GetUnlockedCount();
            int total = Cookbook.Instance.GetTotalCount();
            progressText.text = $"Recipes: {unlocked}/{total}";
        }
    }
    
    private void SetQualityFilter(DishQuality quality)
    {
        currentQualityFilter = quality;
        DisplayRecipesByQuality(quality);
        UpdateTabButtons();
    }
    
    private void UpdateTabButtons()
    {
        // Update tab button visuals based on current selection
        if (commonTabButton != null)
            commonTabButton.interactable = currentQualityFilter != DishQuality.Common;
        if (greatTabButton != null)
            greatTabButton.interactable = currentQualityFilter != DishQuality.Great;
        if (familyTabButton != null)
            familyTabButton.interactable = currentQualityFilter != DishQuality.Family;
        if (perfectTabButton != null)
            perfectTabButton.interactable = currentQualityFilter != DishQuality.Perfect;
    }
    
    private void DisplayRecipesByQuality(DishQuality quality)
    {
        // Clear current entries
        foreach (var entry in currentEntries)
        {
            if (entry != null)
                Destroy(entry);
        }
        currentEntries.Clear();
        
        if (recipeContainer == null) return;
        
        var recipesInQuality = Cookbook.Instance.GetRecipesByQuality(quality);
        
        foreach (var recipe in recipesInQuality)
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
        else
        {
            // Fallback if no CookbookEntryUI component
            SetupBasicEntry(entryObj, recipe);
        }
    }
    
    private void SetupBasicEntry(GameObject entryObj, CookbookEntry recipe)
    {
        // Set up basic text and image components
        var nameText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = recipe.dishName;
            nameText.color = GetQualityColor(recipe.quality);
        }
        
        var button = entryObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowRecipeDetail(recipe));
        }
    }
    
    public void ShowRecipeDetail(CookbookEntry recipe)
    {
        if (recipeDetailPanel == null) return;
        
        recipeDetailPanel.SetActive(true);
        
        if (recipeNameText != null)
            recipeNameText.text = recipe.dishName;
        if (recipeDescriptionText != null)
            recipeDescriptionText.text = recipe.description;
        if (recipeQualityText != null)
            recipeQualityText.text = $"Quality: {recipe.quality}";
        if (recipeImage != null && recipe.dishSprite != null)
            recipeImage.sprite = recipe.dishSprite;
    }
    
    public void HideRecipeDetail()
    {
        if (recipeDetailPanel != null)
            recipeDetailPanel.SetActive(false);
    }
    
    private Color GetQualityColor(DishQuality quality)
    {
        return quality switch
        {
            DishQuality.Common => Color.white,
            DishQuality.Great => Color.green,
            DishQuality.Family => Color.blue,
            DishQuality.Perfect => Color.yellow,
            _ => Color.white
        };
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