using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Customer : MonoBehaviour, IDialogueOptionReciever, IInteractable
{
    [Header("Movement Settings")]
    public Transform[] waypoints; // Waypoints to walk through
    public float lerpSpeed = 3f; // Speed of lerping between waypoints
    
    [Header("Seating")]
    public Transform seatPosition; // Final position where the customer will sit
    public bool isSeated = false;
    
    [Header("State")]
    public bool isWalking = false;
    public bool hasEnteredCafe = false;
    
    [Header("Customer State")]
    private float timeSinceOrder = 0f;
    public float patienceTime = 30f; // Time before customer gets impatient
    public float approachPatienceTime = 15f; // Time before customer leaves if not approached
    public float annoyedPatienceTime = 20f; // Time before customer leaves if asked for a hint
    public float extraPatienceMultiplier = 1.5f; // Multiplier for extra patience time
    
    [Header("Order System")]
    public int[] acceptableDishIds; // Array of acceptable dish IDs
    public int perfectDishId; // ID of the perfect dish
    
    [Header("Dialogues")]
    public Dialogue initialOrderDialogue; // Dialogue when customer first orders
    public Dialogue perfectDishDialogue; // Dialogue when perfect dish is given
    public Dialogue acceptableDishDialogue; // Dialogue when acceptable dish is given
    public Dialogue unacceptableDishDialogue; // Dialogue when unacceptable dish is given
    
    [Header("Events")]
    public UnityEvent onFamilyRecipeShared; // Event triggered when family recipe is shared
    public UnityEvent onCustomerSatisfied; // Event triggered when customer is satisfied
    public UnityEvent onCustomerEnraged; // Event triggered when customer is enraged
    
    [Header("Progress Bar")]
    public GameObject progressBarObject; // The progress bar UI object
    public GameObject progressBarOutline;
    public UnityEngine.UI.Image progressBarFill; // The fill image of the progress bar
    public float patienceProgress = 0f; // Current patience progress (0 to 1)
    
    // Customer interaction states
    public enum CustomerState
    {
        Walking,
        Seated,
        Talking,
        HasOrdered,
        Impatient,
        Satisfied,
        Enraged
    }
    
    public CustomerState currentState = CustomerState.Walking;
    public bool IsInteractable { get; set; } = false; // Start as false, will be set to true when seated
    
    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    private float approachTimer = 0f;
    private bool isPerfectDish = false; // Track if the current dialogue is for a perfect dish
    
    // Animation components (optional)
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set initial position to first waypoint if available
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }

        if (progressBarObject != null)
        {
            progressBarObject.SetActive(false);
        }
        if (progressBarOutline != null)
        {
            progressBarOutline.SetActive(false);
        }
    }
    
    void Update()
    {
        if (!hasEnteredCafe)
        {
            StartCoroutine(EnterCafe());
        }
        else if (isWalking && !isSeated)
        {
            HandleMovement();
        }
        else if (isSeated)
        {
            HandleSeatedBehavior();
        }
    }
    
    private void HandleSeatedBehavior()
    {
        switch (currentState)
        {
            case CustomerState.Seated:
                // Customer is seated, waiting for player to approach
                approachTimer += Time.deltaTime;
                patienceProgress = Mathf.Min(approachTimer / approachPatienceTime, 1f);
                UpdateProgressBar();
                
                if (approachTimer >= approachPatienceTime)
                {
                    // Customer leaves without being approached
                    LeaveCafe(CustomerState.Enraged);
                }
                break;
                
            case CustomerState.Talking:
                // Customer is talking to player - pause all timers
                // Progress bar stays at current level
                break;
                
            case CustomerState.HasOrdered:
                // Customer has ordered, waiting for food
                timeSinceOrder += Time.deltaTime;
                patienceProgress = Mathf.Min(timeSinceOrder / patienceTime, 1f);
                UpdateProgressBar();
                
                if (timeSinceOrder >= patienceTime && currentState != CustomerState.Impatient)
                {
                    currentState = CustomerState.Impatient;
                    Debug.Log("Customer is getting impatient!");
                }
                break;
                
            case CustomerState.Impatient:
                // Customer is impatient, might leave soon
                timeSinceOrder += Time.deltaTime;
                patienceProgress = 1f; // Extra patience time
                UpdateProgressBar();
                
                if (timeSinceOrder >= patienceTime * extraPatienceMultiplier) // Extra patience time
                {
                    LeaveCafe(CustomerState.Enraged);
                }
                break;
        }
    }
    
    private void UpdateProgressBar()
    {
        if (progressBarObject != null)
        {
            progressBarObject.SetActive(true);
        }
        if (progressBarOutline != null)
        {
            progressBarOutline.SetActive(true);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = patienceProgress;
            
            // Change color based on patience level
            if (patienceProgress < 0.5f)
            {
                progressBarFill.color = Color.green;
            }
            else if (patienceProgress < 0.8f)
            {
                progressBarFill.color = Color.yellow;
            }
            else
            {
                progressBarFill.color = Color.red;
            }
        }
    }
    
    private void HideProgressBar()
    {
        if (progressBarObject != null)
        {
            progressBarObject.SetActive(false);
        }
        if (progressBarOutline != null)
        {
            progressBarOutline.SetActive(false);
        }
        patienceProgress = 0f;
    }
    
    // IInteractable implementation - this is the main interaction method
    public void Interact()
    {
        if (!IsInteractable) return;
        
        // Get the item the player is holding (this will be implemented by the player)
        int playerHeldItem = GetPlayerHeldItem();
        
        if (currentState == CustomerState.Seated)
        {
            if (initialOrderDialogue != null)
            {
                currentState = CustomerState.Talking;

                DialogueManager.instance.onDialogueEnded.AddListener(OnInitialOrderDialogueEnded);
                DialogueManager.instance.StartDialogue(initialOrderDialogue);
            }
        }
        else if (currentState == CustomerState.HasOrdered || currentState == CustomerState.Impatient)
        {
            if (playerHeldItem == 0)
            {
                if (initialOrderDialogue != null)
                {
                    DialogueManager.instance.StartDialogue(initialOrderDialogue);
                }
            }
            else
            {
                // Player has an item - validate and respond
                ValidateAndRespondToItem(playerHeldItem);
            }
        }
    }
    
    private int GetPlayerHeldItem()
    {
        return 0;
    }
    
    // Validate the item and show appropriate dialogue
    private void ValidateAndRespondToItem(int itemId)
    {
        if (itemId == perfectDishId)
        {
            // Perfect dish - give family recipe
            currentState = CustomerState.Satisfied;
            isPerfectDish = true; // Mark this as a perfect dish
            if (perfectDishDialogue != null)
            {
                // Subscribe to dialogue end event before starting the dialogue
                DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
                DialogueManager.instance.StartDialogue(perfectDishDialogue);
            }
        }
        else if (IsAcceptableDish(itemId))
        {
            // Acceptable dish - customer is satisfied
            currentState = CustomerState.Satisfied;
            if (acceptableDishDialogue != null)
            {
                // Subscribe to dialogue end event before starting the dialogue
                DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
                DialogueManager.instance.StartDialogue(acceptableDishDialogue);
            }
        }
        else
        {
            // Unacceptable dish - customer is enraged
            currentState = CustomerState.Enraged;
            if (unacceptableDishDialogue != null)
            {
                // Subscribe to dialogue end event before starting the dialogue
                DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
                DialogueManager.instance.StartDialogue(unacceptableDishDialogue);
            }
        }
    }

    private void HandleAnnoyed()
    {
        patienceTime = annoyedPatienceTime;
    }
    
    // Check if a dish ID is in the acceptable dishes array
    private bool IsAcceptableDish(int dishId)
    {
        if (acceptableDishIds == null) return false;
        
        foreach (int acceptableId in acceptableDishIds)
        {
            if (acceptableId == dishId)
            {
                return true;
            }
        }
        return false;
    }
    
    // Customer leaves the cafe
    private void LeaveCafe(CustomerState exitState)
    {
        currentState = exitState;
        Debug.Log($"Customer leaving cafe in state: {exitState}");
        
        // Hide progress bar
        HideProgressBar();
        
        // Trigger appropriate event
        switch (exitState)
        {
            case CustomerState.Satisfied:
                onCustomerSatisfied?.Invoke();
                break;
            case CustomerState.Enraged:
                onCustomerEnraged?.Invoke();
                break;
        }
        
        // You can add leaving animation or movement here
        StartCoroutine(LeaveCafeCoroutine());
    }
    
    private IEnumerator LeaveCafeCoroutine()
    {
        // Walk back through waypoints in reverse
        for (int i = waypoints.Length - 1; i >= 0; i--)
        {
            yield return StartCoroutine(LerpToPosition(waypoints[i].position));
        }
        
        // Destroy or deactivate the customer
        gameObject.SetActive(false);
    }
    
    private IEnumerator EnterCafe()
    {
        hasEnteredCafe = true;
        isWalking = true;
        currentState = CustomerState.Walking;
        
        // Walk through waypoints
        for (int i = 1; i < waypoints.Length; i++)
        {
            currentWaypointIndex = i;
            targetPosition = waypoints[i].position;
            
            // Start lerping to next waypoint
            yield return StartCoroutine(LerpToPosition(targetPosition));
        }
        
        // Walk to seat
        if (seatPosition != null)
        {
            yield return StartCoroutine(LerpToPosition(seatPosition.position));
            isSeated = true;
            isWalking = false;
            currentState = CustomerState.Seated;
            approachTimer = 0f;
            
            // Optional: Play sitting animation
            if (animator != null)
            {
                animator.SetBool("IsSeated", true);
            }
            IsInteractable = true; // Set interactable to true when seated
        }
    }
    
    private void HandleMovement()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            targetPosition = waypoints[currentWaypointIndex].position;
            
            // Update sprite direction based on movement
            if (spriteRenderer != null)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction.x > 0.1f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (direction.x < -0.1f)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }
    
    private IEnumerator LerpToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPos);
        float startTime = Time.time;
        
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * lerpSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            Vector3 previousPos = transform.position;
            transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
            
            // Calculate motion vector
            Vector3 motionVector = (transform.position - previousPos).normalized;
            
            // Update animation if available
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
                animator.SetFloat("horizontal", -motionVector.x); // Invert horizontal to fix flipped animations
                animator.SetFloat("vertical", motionVector.y);
            }
            
            yield return null;
        }
        
        transform.position = targetPos;
        
        // Stop walking animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("horizontal", 0f);
            animator.SetFloat("vertical", 0f);
        }
    }
    
    // Method to manually trigger cafe entry (useful for testing)
    public void TriggerCafeEntry()
    {
        if (!hasEnteredCafe)
        {
            StartCoroutine(EnterCafe());
        }
    }
    
    // Method to reset customer state
    public void ResetCustomer()
    {
        hasEnteredCafe = false;
        isWalking = false;
        isSeated = false;
        currentWaypointIndex = 0;
        currentState = CustomerState.Walking;
        approachTimer = 0f;
        timeSinceOrder = 0f;
        
        // Hide progress bar
        HideProgressBar();
        
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsSeated", false);
        }
        IsInteractable = false; // Reset interactable state
    }

    public void OnDialogueOptionChosen(DialogueOption option)
    {
        Debug.Log("Player chose option: " + option.optionText);
        
        // Handle different dialogue effects
        switch (option.effect)
        {
            case DialogueOptionEffect.GiveAHint:
                Debug.Log("Player asked for a hint");
                HandleAnnoyed();
                break;
            default:
                Debug.Log("No specific effect for this option");
                break;
        }
    }

    private void OnInitialOrderDialogueEnded()
    {
        // Unsubscribe from the event
        DialogueManager.instance.onDialogueEnded.RemoveListener(OnInitialOrderDialogueEnded);
        
        // Start the timer after the dialogue ends
        currentState = CustomerState.HasOrdered;
        timeSinceOrder = 0f;
        patienceProgress = 0f;
        UpdateProgressBar();
    }
    
    private void OnDialogueEnded()
    {
        // Unsubscribe from the event
        DialogueManager.instance.onDialogueEnded.RemoveListener(OnDialogueEnded);
        
        // Trigger appropriate events based on customer state
        switch (currentState)
        {
            case CustomerState.Satisfied:
                // Check if this was a perfect dish
                if (isPerfectDish)
                {
                    onFamilyRecipeShared?.Invoke();
                }
                onCustomerSatisfied?.Invoke();
                break;
            case CustomerState.Enraged:
                onCustomerEnraged?.Invoke();
                break;
        }
        
        // Reset the perfect dish flag
        isPerfectDish = false;
    }
}
