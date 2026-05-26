using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Game loop driver, picks the next customer, judges submissions, resets the
/// dough between customers, and ends the game when the timer reaches zero
/// </summary>
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    public List<CustomerOrder> availableOrders;
    public float timeBeforeNextCustomer = 1.5f;
    public float gameDuration = 60f; 
    public ResultPopup resultPopup;
    
    private float timeRemaining;
    private int customersServed;
    private int customersPassed;
    private bool gameActive;
    
    public float TimeRemaining => timeRemaining;
    public int CustomersServed => customersServed;
    public int CustomersPassed => customersPassed;
    
    private CustomerOrder currentOrder;
    public CustomerOrder CurrentOrder => currentOrder;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        timeRemaining = gameDuration;
        customersServed = 0;
        customersPassed = 0;
    }
    
    void Update()
    {
        if (!gameActive) return;
    
        timeRemaining -= Time.deltaTime;
    
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }
        
        if (Keyboard.current == null) return;
    
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SubmitOrder();
    }

    public void StartGame()
    {
        gameActive = true;
        NextCustomer();
    }
    
    // Resets the dough, picks a random order, transitions to Making
    private void NextCustomer()
    {
        ResetSambosa();
        
        if (availableOrders.Count == 0)
        {
            Debug.LogWarning("No orders available!");
            return;
        }
    
        currentOrder = availableOrders[Random.Range(0, availableOrders.Count)];
        Debug.Log("New customer: " + currentOrder.customerName + 
                  " wants " + currentOrder.meatRequired + " meat, " + 
                  currentOrder.cheeseRequired + " cheese");
    
        GameStateMachine.Instance.ChangeState(SambosaGameState.Making);
    }
    
// Counts pieces stuck to the dough, compares to the current order, displays
// the result, and queues the next customer after a short delay
    public void SubmitOrder()
    {
        if (currentOrder == null) return;
    
        int meatCount = 0;
        int cheeseCount = 0;
    
        FillingPiece[] allPieces = FindObjectsByType<FillingPiece>();
        foreach (FillingPiece piece in allPieces)
        {
            if (piece.IsStuck())
            {
                if (piece.type == FillingType.Meat) meatCount++;
                else if (piece.type == FillingType.Cheese) cheeseCount++;
            }
        }
    
        bool success = (meatCount == currentOrder.meatRequired) && 
                       (cheeseCount == currentOrder.cheeseRequired);
    
        resultPopup.ShowResult(success);
        Debug.Log("Submitted: " + meatCount + " meat, " + cheeseCount + " cheese. " +
                  (success ? "PASS!" : "FAIL!"));
        
        customersServed++;
        if (success) customersPassed++;
    
        GameStateMachine.Instance.ChangeState(SambosaGameState.Judging);
        Invoke("NextCustomer", timeBeforeNextCustomer);
    }
    
    public void ResetSambosa()
    {

        FillingPiece[] allPieces = FindObjectsByType<FillingPiece>();
        foreach (FillingPiece piece in allPieces)
        {
            if (piece.IsStuck())
            {
                Destroy(piece.gameObject);
            }
        }
    
        SambosaMesh dough = FindAnyObjectByType<SambosaMesh>();
        if (dough != null)
        {
            dough.ResetToFlat();
        }
    }
    
    private void EndGame()
    {
        gameActive = false;
        CancelInvoke();  // Cancel any pending NextCustomer
        GameStateMachine.Instance.ChangeState(SambosaGameState.GameOver);
    }
}
