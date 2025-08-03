using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Critic : Customer
{
    [Header("Critic Family Dishes")]
    public int familyDishId1; // First family dish option
    public int familyDishId2; // Second family dish option  
    public int familyDishId3; // Third family dish option
    
    [Header("Critic Dialogues")]
    public Dialogue familyDish1Dialogue; // Dialogue for first family dish
    public Dialogue familyDish2Dialogue; // Dialogue for second family dish
    public Dialogue familyDish3Dialogue; // Dialogue for third family dish
    
    [Header("Critic Events")]
    public UnityEvent onCriticServed; // Event triggered when perfect dish is served
    
    private bool isPerfectDishServed = false;
    
    // Override the ValidateAndRespondToItem method to handle critic-specific logic
    protected override void ValidateAndRespondToItem(int itemId)
    {
        DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
        
        if (itemId == perfectDishId)
        {
            // Perfect dish - only this satisfies the critic
            currentState = CustomerState.Satisfied;
            isPerfectDish = true;
            isPerfectDishServed = true;
            if (perfectDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(perfectDishDialogue);
            }
        }
        else if (itemId == familyDishId1)
        {
            // First family dish - just dialogue, doesn't satisfy
            if (familyDish1Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish1Dialogue);
            }
        }
        else if (itemId == familyDishId2)
        {
            // Second family dish - just dialogue, doesn't satisfy
            if (familyDish2Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish2Dialogue);
            }
        }
        else if (itemId == familyDishId3)
        {
            // Third family dish - just dialogue, doesn't satisfy
            if (familyDish3Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish3Dialogue);
            }
        }
        else
        {
            // Unacceptable dish - customer is enraged
            currentState = CustomerState.Enraged;
            if (unacceptableDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(unacceptableDishDialogue);
            }
        }
    }
    
    // Override the OnDialogueEnded method to handle critic-specific events
    protected override void OnDialogueEnded()
    {
        // Unsubscribe from the event
        DialogueManager.instance.onDialogueEnded.RemoveListener(OnDialogueEnded);
        
        // Trigger appropriate events based on customer state
        switch (currentState)
        {
            case CustomerState.Satisfied:
                // Check if this was a perfect dish
                if (isPerfectDishServed)
                {
                    onCriticServed?.Invoke();
                }
                onCustomerSatisfied?.Invoke();
                LeaveCafe(CustomerState.Satisfied);
                break;
            case CustomerState.Enraged:
                onCustomerEnraged?.Invoke();
                LeaveCafe(CustomerState.Enraged);
                break;
            default:
                // For family dish dialogues that don't change state, just end the dialogue
                break;
        }
        
        // Reset the perfect dish flag
        isPerfectDish = false;
        isPerfectDishServed = false;
    }
    
    // Public method to check if a dish is one of the family dishes
    public bool IsFamilyDish(int dishId)
    {
        return dishId == familyDishId1 || dishId == familyDishId2 || dishId == familyDishId3;
    }
    
    // Public method to get which family dish number (1, 2, or 3) a dish ID corresponds to
    public int GetFamilyDishNumber(int dishId)
    {
        if (dishId == familyDishId1) return 1;
        if (dishId == familyDishId2) return 2;
        if (dishId == familyDishId3) return 3;
        return 0; // Not a family dish
    }
} 