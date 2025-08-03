using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner instance;
    
    [Header("Customer Prefabs")]
    [Tooltip("Array of customer prefabs in order (first 3 are regular customers, 4th is critic)")]
    public GameObject[] customerPrefabs = new GameObject[4];
    
    [Header("Spawn Settings")]
    public Transform spawnPoint; // Where customers spawn
    public float[] spawnIntervals = {45f, 45f, 45f, 45f}; // Time between customer spawns
    public float firstCustomerDelay = 10f; // Delay before first customer
    
    [Header("Events")]
    public UnityEvent onAllCustomersLeft; // Fired when all 3 regular customers were satisfied
    public UnityEvent onCustomerSpawned; // Fired when a customer spawns (passes customer index)
    
    // Track customer satisfaction
    private bool[] customerSatisfied = new bool[4];
    private int currentCustomerIndex = 0;
    private bool isSpawning = false;
    private List<Customer> activeCustomers = new List<Customer>();
    private int customersLeftCount = 0; // Track how many customers have left

    // Coroutine reference
    private Coroutine spawnCoroutine;
    
    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple CustomerSpawner instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Validate setup
        if (customerPrefabs.Length != 4)
        {
            Debug.LogError("CustomerSpawner requires exactly 4 customer prefabs!");
            return;
        }
        
        if (spawnPoint == null)
        {
            Debug.LogWarning("No spawn point set, using spawner's position");
            spawnPoint = transform;
        }
        
        // Initialize satisfaction tracking
        for (int i = 0; i < customerSatisfied.Length; i++)
        {
            customerSatisfied[i] = false;
        }
    }
    
    void OnEnable()
    {
        StartSpawning();
    }
    
    void OnDisable()
    {
        StopSpawning();
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnCoroutine = StartCoroutine(SpawnCustomers());
        }
    }
    
    public void StopSpawning()
    {
        if (isSpawning)
        {
            isSpawning = false;
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }
    
    private IEnumerator SpawnCustomers()
    {
        // Wait for initial delay
        yield return new WaitForSeconds(firstCustomerDelay);
        
        while (isSpawning && currentCustomerIndex < 3) // Only spawn first 3 customers initially
        {
            // Spawn regular customer
            SpawnCustomer(currentCustomerIndex);
            
            // Wait for next spawn
            yield return new WaitForSeconds(spawnIntervals[currentCustomerIndex]);
        }
        
        // After spawning all 3 regular customers, spawning pauses
        // Critic will be spawned later when all regular customers have left
        Debug.Log("All 3 regular customers spawned. Waiting for them to be served before critic decision.");
    }
    
    private void SpawnCustomer(int index)
    {
        if (customerPrefabs[index] == null)
        {
            Debug.LogError($"Customer prefab at index {index} is null!");
            return;
        }
        
        // Spawn the customer
        GameObject customerObj = Instantiate(customerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        Customer customer = customerObj.GetComponentInChildren<Customer>();
        
        if (customer != null)
        {
            customer.onCustomerSatisfied.AddListener(() => OnCustomerSatisfied(index));
            customer.onCustomerEnraged.AddListener(() => OnCustomerUnsatisfied(index));
            customer.onCustomerLeft.AddListener(() => OnCustomerLeft(index));
            
            activeCustomers.Add(customer);
            
            customer.TriggerCafeEntry();
        
            Debug.Log($"CustomerSpawner: Invoking onCustomerSpawned event for customer {currentCustomerIndex}");
            onCustomerSpawned?.Invoke();
            currentCustomerIndex++; //Move to next Customer
        }
        else
        {
            Debug.LogError($"Spawned customer prefab at index {index} doesn't have Customer component in any child!");
        }
        
        Debug.Log($"Spawned customer {index + 1} of 4");
    }
    
    private void OnCustomerSatisfied(int customerIndex)
    {
        if (customerIndex < 4) // Only track first 3 customers
        {
            customerSatisfied[customerIndex] = true;
            Debug.Log($"Customer {customerIndex + 1} was satisfied!");
        }
    }
    
    private void OnCustomerUnsatisfied(int customerIndex)
    {
        if (customerIndex < 4) // Only track first 3 customers
        {
            customerSatisfied[customerIndex] = false;
            Debug.Log($"Customer {customerIndex + 1} was unsatisfied!");
        }
    }
    
    private void OnCustomerLeft(int customerIndex)
    {
        customersLeftCount++;
        Debug.Log($"Customer {customerIndex + 1} left. Total customers left: {customersLeftCount}");
        
        // Remove customer from active list
        activeCustomers.RemoveAll(customer => customer == null);
        
        // Check if all 3 regular customers have left and critic hasn't been spawned yet
        if (customersLeftCount == 3 && currentCustomerIndex == 3)
        {
            // Time to decide about the critic
            if (AreNCustomersSatisfied(3))
            {
                Debug.Log("All 3 regular customers were satisfied! Spawning critic...");
                SpawnCustomer(3); // Spawn the critic (index 3)
            }
            else
            {
                Debug.Log("Not all regular customers were satisfied. Critic will not come today.");
                // All customers are done, fire the event
                onAllCustomersLeft?.Invoke();
                Debug.Log($"All customers left! Expected: 3, Left: {customersLeftCount}");
            }
        }
        else
        {
            // Determine how many customers we expected
            int expectedCustomers = GetExpectedCustomerCount();
            
            // Check if all expected customers have left
            if (customersLeftCount >= expectedCustomers)
            {
                onAllCustomersLeft?.Invoke();
                Debug.Log($"All customers left! Expected: {expectedCustomers}, Left: {customersLeftCount}");
            }
        }
    }
    
    private int GetExpectedCustomerCount()
    {
        // If we actually spawned the critic (4th customer), expect 4
        if (currentCustomerIndex >= 4)
        {
            return 4;
        }
        // Otherwise, expect the default 3 regular customers
        else
        {
            return 3;
        }
    }
    
    private bool AreNCustomersSatisfied(int n)
    {
        int satisfiedCount = 0;
        Debug.Log($"Checking customer satisfaction for critic decision:");
        for (int i = 0; i < 3; i++) // Only check first 3 customers for critic decision
        {
            if (customerSatisfied[i])
            {
                satisfiedCount++;
                Debug.Log($"  Customer {i + 1}: SATISFIED");
            }
            else
            {
                Debug.Log($"  Customer {i + 1}: NOT satisfied");
            }
        }
        Debug.Log($"Total satisfied: {satisfiedCount}/{n} required");
        return satisfiedCount >= n;
    }
    
    // Public methods for external control
    public void ResetSpawner()
    {
        StopSpawning();
        
        // Reset tracking
        currentCustomerIndex = 0;
        customersLeftCount = 0; // Reset customer left counter
        for (int i = 0; i < customerSatisfied.Length; i++)
        {
            customerSatisfied[i] = false;
        }
        
        // Clear active customers
        foreach (var customer in activeCustomers)
        {
            if (customer != null)
            {
                Destroy(customer.gameObject);
            }
        }
        activeCustomers.Clear();
    }
    
    public int GetCurrentCustomerIndex()
    {
        return currentCustomerIndex;
    }
    
    public bool[] GetCustomerSatisfactionStatus()
    {
        return (bool[])customerSatisfied.Clone();
    }
    
    public bool IsCurrentlySpawning()
    {
        return isSpawning;
    }
    
    public int GetCustomersLeftCount()
    {
        return customersLeftCount;
    }
    
    public int GetExpectedCustomersCount()
    {
        return GetExpectedCustomerCount();
    }
    
    // Debug method to force critic spawn (for testing)
    [ContextMenu("Force Critic Spawn")]
    public void ForceCriticSpawn()
    {
        if (currentCustomerIndex < 3)
        {
            Debug.LogWarning("Can't spawn critic yet - not all regular customers have been spawned");
            return;
        }
        
        // Set all customers as satisfied
        for (int i = 0; i < 3; i++)
        {
            customerSatisfied[i] = true;
        }
        
        Debug.Log("Forced all regular customers to be satisfied for testing");
        
        // Spawn critic if conditions are right
        if (currentCustomerIndex == 3)
        {
            SpawnCustomer(3);
        }
    }
    
    [ContextMenu("Check Customer Satisfaction")]
    public void DebugCheckSatisfaction()
    {
        Debug.Log("=== Customer Satisfaction Status ===");
        for (int i = 0; i < customerSatisfied.Length; i++)
        {
            Debug.Log($"Customer {i + 1}: {(customerSatisfied[i] ? "SATISFIED" : "NOT satisfied")}");
        }
        Debug.Log($"Current customer index: {currentCustomerIndex}");
        Debug.Log($"Customers left: {customersLeftCount}");
        Debug.Log($"Expected customers: {GetExpectedCustomerCount()}");
    }
}