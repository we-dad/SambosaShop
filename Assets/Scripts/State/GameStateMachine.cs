using System;
using UnityEngine;

/// <summary>
/// Central state controller,Other scripts subscribe to OnStateChanged rather
/// than polling currentState
/// </summary>
public enum SambosaGameState
{
    Uninitialized,
    WaitingForCustomer,
    Making,    
    Sealing,
    Judging,
    GameOver
}
public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }
    
    public SambosaGameState CurrentState => currentState;
    public event Action<SambosaGameState> OnStateChanged;
    
    private SambosaGameState currentState;
    
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
        //this way to call the event to make initialize properly
        ChangeState(SambosaGameState.WaitingForCustomer);
        
    }
    
    public void ChangeState(SambosaGameState newState)
    {
        // Guard transitions so the event doesn't fire when nothing changes
        if (newState == currentState) return;
        
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
        
        Debug.Log(currentState);
    }
    
}
