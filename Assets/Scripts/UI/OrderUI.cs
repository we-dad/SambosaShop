using UnityEngine;
using TMPro;

/// <summary>
/// Displays the current order text, Subscribes to OnStateChanged to update
/// when a new customer arrives
/// </summary>
public class OrderUI : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    
    void Start()
    {
        GameStateMachine.Instance.OnStateChanged += HandleStateChanged;
    
        // Also handle the case where Start runs after the state has already been set
        if (GameStateMachine.Instance.CurrentState == SambosaGameState.Making)
            HandleStateChanged(SambosaGameState.Making);
    }

    void OnDestroy()
    {
        if (GameStateMachine.Instance != null)
            GameStateMachine.Instance.OnStateChanged -= HandleStateChanged;
    }
    
    private void HandleStateChanged(SambosaGameState newState)
    {
        if (newState == SambosaGameState.Making)
        {
            CustomerOrder order = OrderManager.Instance.CurrentOrder;
            orderText.text = order.customerName + " wants:\n" + 
                             order.meatRequired + " meat\n" + 
                             order.cheeseRequired + " cheese";
        }
    }
}