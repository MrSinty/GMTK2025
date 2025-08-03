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
    
    protected override void ValidateAndRespondToItem(int itemId)
    {
        DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
        
        if (itemId == perfectDishId)
        {
            currentState = CustomerState.Satisfied;
            isPerfectDish = true;
            if (perfectDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(perfectDishDialogue);
            }
        }
        else if (itemId == familyDishId1)
        {
            if (familyDish1Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish1Dialogue);
            }
        }
        else if (itemId == familyDishId2)
        {
            if (familyDish2Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish2Dialogue);
            }
        }
        else if (itemId == familyDishId3)
        {
            if (familyDish3Dialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDish3Dialogue);
            }
        }
        else
        {
            if (unacceptableDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(unacceptableDishDialogue);
            }
        }
    }
    
    protected override void OnDialogueEnded()
    {
        DialogueManager.instance.onDialogueEnded.RemoveListener(OnDialogueEnded);
        
        switch (currentState)
        {
            case CustomerState.Satisfied:
                if (isPerfectDish)
                {
                    onCustomerSatisfied?.Invoke();
                }
                LeaveCafe(CustomerState.Satisfied);
                break;
            case CustomerState.Enraged:
                onCustomerEnraged?.Invoke();
                LeaveCafe(CustomerState.Enraged);
                break;
            default:
                break;
        }
        
        isPerfectDish = false;
    }
    
    public bool IsFamilyDish(int dishId)
    {
        return dishId == familyDishId1 || dishId == familyDishId2 || dishId == familyDishId3;
    }
    
    public int GetFamilyDishNumber(int dishId)
    {
        if (dishId == familyDishId1) return 1;
        if (dishId == familyDishId2) return 2;
        if (dishId == familyDishId3) return 3;
        return 0; // Not a family dish
    }

    protected override void RespondToDish(int dishId)
    {
        if (dishId == perfectDishId)
        {
            currentState = CustomerState.Satisfied;
        }
        else
        {
            currentState = CustomerState.Enraged;
        }
    }
} 