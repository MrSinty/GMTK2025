using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Customer : MonoBehaviour, IInteractable
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
    protected float timeSinceOrder = 0f;
    public float patienceTime = 30f; // Time before customer gets impatient
    public float approachPatienceTime = 15f; // Time before customer leaves if not approached
    public float annoyedPatienceTime = 20f; // Time before customer leaves if asked for a hint
    public float extraPatienceMultiplier = 1.5f; // Multiplier for extra patience time
    
    [Header("Order System")]
    public int playerHeldItem = 0;
    public int[] acceptableDishIds; // Array of acceptable dish IDs
    public int perfectDishId; // ID of the perfect dish
    public int familyDishId; // ID of the family dish (hidden for Critic)
    
    [Header("Dialogues")]
    public Dialogue initialOrderDialogue;
    public Dialogue hasOrderedDialogue;
    public Dialogue unacceptableDishDialogue;
    public Dialogue acceptableDishDialogue;
    public Dialogue perfectDishDialogue;
    public Dialogue familyDishDialogue; // Family dish dialogue (hidden for Critic)
    
    [Header("Events")]
    public UnityEvent<int> onFamilyRecipeShared; // Event triggered when family recipe is shared, passes the recipe ID
    public UnityEvent onCustomerSatisfied; // Event triggered when customer is satisfied
    public UnityEvent onCustomerEnraged; // Event triggered when customer is enraged
    public UnityEvent onCustomerLeft; // Event triggered when customer is enraged
    
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
    
    protected int currentWaypointIndex = 0;
    protected Vector3 targetPosition;
    protected float approachTimer = 0f;
    protected bool isPerfectDish = false; // Track if the current dialogue is for a perfect dish
    
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
    
    protected void HandleSeatedBehavior()
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
    
    protected void UpdateProgressBar()
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
    
    protected void HideProgressBar()
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
        

        playerHeldItem = GetPlayerHeldItem();
        
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
                if (hasOrderedDialogue != null)
                {
                    DialogueManager.instance.StartDialogue(hasOrderedDialogue);
                }
            }
            else
            {
                // Player has an item - validate and respond
                ValidateAndRespondToItem(playerHeldItem);
            }
        }
    }
    
    protected virtual int GetPlayerHeldItem()
    {
        return playerHeldItem;
    }
    
    // Validate the item and show appropriate dialogue
    protected virtual void ValidateAndRespondToItem(int itemId)
    {
        DialogueManager.instance.onDialogueEnded.AddListener(OnDialogueEnded);
        DialogueManager.instance.onDialogueOptionChosen.AddListener(OnDialogueOptionChosen);
        
        if (itemId == perfectDishId)
        {
            isPerfectDish = true; // Mark this as a perfect dish
            if (perfectDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(perfectDishDialogue);
            }
        }
        else if (itemId == familyDishId)
        {
            if (familyDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(familyDishDialogue);
            }
        }
        else if (IsAcceptableDish(itemId))
        {
            if (acceptableDishDialogue != null)
            {
                DialogueManager.instance.StartDialogue(acceptableDishDialogue);
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

    protected virtual void RespondToDish(int dishId)
    {
        if (IsAcceptableDish(dishId) || dishId == perfectDishId || dishId == familyDishId)
        {
            currentState = CustomerState.Satisfied;
        }
        else
        {
            currentState = CustomerState.Enraged;
        }
    }

    protected void HandleAnnoyed()
    {
        patienceTime = annoyedPatienceTime;
    }
    
    // Check if a dish ID is in the acceptable dishes array
    protected bool IsAcceptableDish(int dishId)
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
    protected void LeaveCafe(CustomerState exitState)
    {
        currentState = exitState;
        Debug.Log($"Customer leaving cafe in state: {exitState}");
        
        // Hide progress bar
        HideProgressBar();
        if (animator != null)
        {
            animator.SetBool("IsSeated", false);
            animator.SetBool("IsWalking", true);
        }
        
        // You can add leaving animation or movement here
        StartCoroutine(LeaveCafeCoroutine());
    }
    
    protected IEnumerator LeaveCafeCoroutine()
    {
        // Walk back through waypoints in reverse
        for (int i = waypoints.Length - 1; i >= 0; i--)
        {
            yield return StartCoroutine(LerpToPosition(waypoints[i].position));
        }
        
        // Destroy or deactivate the customer
        gameObject.SetActive(false);
        onCustomerLeft?.Invoke();
    }
    
    protected IEnumerator EnterCafe()
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
    
    protected void HandleMovement()
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
    
    protected IEnumerator LerpToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPos);
        float startTime = Time.time;
        
        Vector3 motionVector = (targetPos - startPos).normalized;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * lerpSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
            
            // Update animation if available
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
                animator.SetFloat("horizontal", motionVector.x); // Invert horizontal to fix flipped animations
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

    public void OnDialogueOptionChosen(DialogueOptionEffect optionEffect)
    {
        Debug.Log("Player chose option: " + optionEffect);
        switch (optionEffect)
        {
            case DialogueOptionEffect.GiveAHint:
                Debug.Log("Player asked for a hint");
                HandleAnnoyed();
                break;
            case DialogueOptionEffect.ServeDish:
                Debug.Log("Player served a dish");
                RespondToDish(playerHeldItem);
                break;
            default:
                Debug.Log("No specific effect for this option");
                break;
        }
        DialogueManager.instance.onDialogueOptionChosen.RemoveListener(OnDialogueOptionChosen);
    }

    protected void OnInitialOrderDialogueEnded()
    {
        // Unsubscribe from the event
        DialogueManager.instance.onDialogueEnded.RemoveListener(OnInitialOrderDialogueEnded);
        
        // Start the timer after the dialogue ends
        currentState = CustomerState.HasOrdered;
        timeSinceOrder = 0f;
        patienceProgress = 0f;
        UpdateProgressBar();
    }
    
    protected virtual void OnDialogueEnded()
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
                    onFamilyRecipeShared?.Invoke(perfectDishId);
                }
                onCustomerSatisfied?.Invoke();
                Debug.Log("Customer satisfied!");
                LeaveCafe(CustomerState.Satisfied);
                break;
            case CustomerState.Enraged:
                onCustomerEnraged?.Invoke();
                Debug.Log("Customer enraged! and leaving");
                LeaveCafe(CustomerState.Enraged);
                break;
            default:
                break;
        }
        
        // Reset the perfect dish flag
        isPerfectDish = false;
    }
}
