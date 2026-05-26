using UnityEngine;

/// <summary>
/// Data asset describing a single customer's order. ScriptableObject so new
/// orders can be added from the Inspector without code changes.
/// </summary>
[CreateAssetMenu(fileName = "order", menuName = "Sambosa/Customer Order")]
public class CustomerOrder : ScriptableObject
{
    [Header("Order Contents")]
    [Tooltip("How many meat pieces this customer wants")]
    [Range(0, 10)]
    public int meatRequired;

    [Tooltip("How many cheese pieces this customer wants")]
    [Range(0, 10)]
    public int cheeseRequired;

    [Header("Order Settings")]
    [Tooltip("Time the customer waits before leaving")]
    public float timeLimit = 30f;

    [Tooltip("Display name shown in the order UI")]
    public string customerName;
}
