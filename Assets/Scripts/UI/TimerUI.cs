using UnityEngine;
using TMPro;

/// <summary>
/// Displays the remaining game time, read from OrderManager each frame
/// </summary>
public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    
    void Update()
    {
        if (OrderManager.Instance == null) return;
        
        float time = OrderManager.Instance.TimeRemaining;
        timerText.text = Mathf.CeilToInt(time).ToString();
    }
}