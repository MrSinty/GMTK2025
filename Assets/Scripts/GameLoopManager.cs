using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager instance;
    
    
    
    [Header("Events")]
    public UnityEvent onGameCycleComplete; // Fired when game cycle completes
    public UnityEvent onEndGameTriggered; // Fired when end game is triggered
    
    [Header("Debug")]
    public bool debugMode = false;
    
    // Game state tracking
    private bool isWaitingForDoorInteraction = false;
    private bool criticWasSatisfied = false;
    private bool gameEnded = false;
    private Customer currentCritic = null;
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple GameLoopManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Subscribe to CustomerSpawner events  
        if (CustomerSpawner.instance != null)
        {
            SubscribeToCustomerSpawner();
        }
        else
        {
            Debug.LogWarning("GameLoopManager: CustomerSpawner instance not found at start, will retry in Update");
        }
        

    }
    
    void Update()
    {
        // Keep trying to subscribe if CustomerSpawner isn't ready yet
        if (CustomerSpawner.instance != null && !IsSubscribedToCustomerSpawner())
        {
            SubscribeToCustomerSpawner();
        }
    }
    
    private bool IsSubscribedToCustomerSpawner()
    {
        return CustomerSpawner.instance.onAllCustomersLeft != null;
    }
    
    private void SubscribeToCustomerSpawner()
    {
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.onAllCustomersLeft.AddListener(OnAllCustomersLeft);
            CustomerSpawner.instance.onCustomerSpawned.AddListener(OnCustomerSpawned);
            Debug.Log("GameLoopManager: Subscribed to CustomerSpawner events");
        }
    }
    
    private void OnCustomerSpawned()
    {
        // Check if this is the critic (4th customer)
        if (CustomerSpawner.instance != null && CustomerSpawner.instance.GetCurrentCustomerIndex() == 4)
        {
            if (debugMode) Debug.Log("GameLoopManager: Critic spawned, setting up event listeners");
            
            // Wait a frame to ensure critic is fully initialized
            StartCoroutine(SetupCriticWithDelay());
        }
    }
    
    private System.Collections.IEnumerator SetupCriticWithDelay()
    {
        yield return null; // Wait one frame
        FindAndSetupCritic();
    }
    
    private void FindAndSetupCritic()
    {
        // Find all Customer components in the scene
        Customer[] customers = FindObjectsOfType<Customer>();
        
        foreach (Customer customer in customers)
        {
            // Check if this is a Critic
            if (customer is Critic critic)
            {
                currentCritic = critic;
                
                // Subscribe to critic's satisfaction and departure events
                critic.onCustomerSatisfied.AddListener(OnCriticSatisfied);
                critic.onCustomerLeft.AddListener(OnCriticLeft);
                
                if (debugMode) Debug.Log("GameLoopManager: Subscribed to critic events");
                break;
            }
        }
    }
    
    private void OnCriticSatisfied()
    {
        if (debugMode) Debug.Log("GameLoopManager: Critic was satisfied!");
        criticWasSatisfied = true;
    }
    
    private void OnCriticLeft()
    {
        if (debugMode) Debug.Log("GameLoopManager: Critic left the cafe");
        
        // If critic was satisfied and left, enable door interaction
        if (criticWasSatisfied && !gameEnded)
        {
            isWaitingForDoorInteraction = true;
            if (debugMode) Debug.Log("GameLoopManager: Critic left satisfied - enabling door interaction");
            
            // Enable door interaction and subscribe to event
            if (DoorOpener.instance != null)
            {
                DoorOpener.instance.EnableDoorInteraction();
                DoorOpener.instance.onDoorInteraction.AddListener(OnDoorInteraction);
            }
        }
        
        // Clean up critic references
        if (currentCritic != null)
        {
            currentCritic.onCustomerSatisfied.RemoveListener(OnCriticSatisfied);
            currentCritic.onCustomerLeft.RemoveListener(OnCriticLeft);
            currentCritic = null;
        }
    }
    
    private void OnAllCustomersLeft()
    {
        if (debugMode) Debug.Log("GameLoopManager: All customers have left, starting game cycle reset");
        
        HandleGameCycleComplete();
    }
    
    private void HandleGameCycleComplete()
    {
        // Check if critic was satisfied
        criticWasSatisfied = CheckIfCriticWasSatisfied();
        
        if (debugMode) Debug.Log($"GameLoopManager: Critic was satisfied: {criticWasSatisfied}");
        
        // Reset the customer spawner
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.ResetSpawner();
            if (debugMode) Debug.Log("GameLoopManager: CustomerSpawner reset");
        }
        
        // Fire game cycle complete event
        onGameCycleComplete?.Invoke();
        
        // If critic was satisfied, we'll wait for the critic to actually leave
        // Door interaction will be enabled when critic leaves satisfied
        if (!criticWasSatisfied)
        {
            // Critic wasn't satisfied, restart spawning for next cycle
            if (CustomerSpawner.instance != null)
            {
                CustomerSpawner.instance.StartSpawning();
            }
        }
    }
    
    private bool CheckIfCriticWasSatisfied()
    {
        if (CustomerSpawner.instance == null) return false;
        
        // Get customer satisfaction status
        bool[] satisfactionStatus = CustomerSpawner.instance.GetCustomerSatisfactionStatus();
        
        // Check if we had 4 customers (including critic) and all were satisfied
        if (satisfactionStatus.Length >= 4)
        {
            // Check if all 4 customers (including critic) were satisfied
            for (int i = 0; i < 4; i++)
            {
                if (!satisfactionStatus[i])
                {
                    return false;
                }
            }
            return true;
        }
        
        return false;
    }
    

    
    private void OnDoorInteraction()
    {
        if (!isWaitingForDoorInteraction) return;
        
        if (debugMode) Debug.Log("GameLoopManager: Door interaction detected, triggering end game");
        
        // Clean up door interaction listener
        if (DoorOpener.instance != null)
        {
            DoorOpener.instance.onDoorInteraction.RemoveListener(OnDoorInteraction);
        }
        
        isWaitingForDoorInteraction = false;
        gameEnded = true;
        
        // Fire end game event
        onEndGameTriggered?.Invoke();
    }
    
    // Public methods for external control
    public void RestartGameLoop()
    {
        gameEnded = false;
        isWaitingForDoorInteraction = false;
        criticWasSatisfied = false;
        
        // Clean up critic references
        if (currentCritic != null)
        {
            currentCritic.onCustomerSatisfied.RemoveListener(OnCriticSatisfied);
            currentCritic.onCustomerLeft.RemoveListener(OnCriticLeft);
            currentCritic = null;
        }
        
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.ResetSpawner();
            CustomerSpawner.instance.StartSpawning();
        }
    }
    
    public bool IsWaitingForDoorInteraction()
    {
        return isWaitingForDoorInteraction;
    }
    
    public bool WasCriticSatisfied()
    {
        return criticWasSatisfied;
    }
    
    public bool HasGameEnded()
    {
        return gameEnded;
    }
    

    
    // Debug methods
    [ContextMenu("Force Game Cycle Complete")]
    public void ForceGameCycleComplete()
    {
        OnAllCustomersLeft();
    }
    
    [ContextMenu("Force Door Interaction")]
    public void ForceDoorInteraction()
    {
        OnDoorInteraction();
    }
    

    
    [ContextMenu("Force Critic Satisfied and Enable Door")]
    public void ForceCriticSatisfiedAndEnableDoor()
    {
        if (debugMode) Debug.Log("GameLoopManager: Force enabling door interaction for testing");
        
        criticWasSatisfied = true;
        isWaitingForDoorInteraction = true;
        
        if (DoorOpener.instance != null)
        {
            DoorOpener.instance.EnableDoorInteraction();
            DoorOpener.instance.onDoorInteraction.AddListener(OnDoorInteraction);
        }
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.onAllCustomersLeft.RemoveListener(OnAllCustomersLeft);
            CustomerSpawner.instance.onCustomerSpawned.RemoveListener(OnCustomerSpawned);
        }
        
        if (currentCritic != null)
        {
            currentCritic.onCustomerSatisfied.RemoveListener(OnCriticSatisfied);
            currentCritic.onCustomerLeft.RemoveListener(OnCriticLeft);
        }
        
        if (DoorOpener.instance != null)
        {
            DoorOpener.instance.onDoorInteraction.RemoveListener(OnDoorInteraction);
        }
    }
}