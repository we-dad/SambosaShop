using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays PASS or FAIL text after the player submits a samosa
/// </summary>
public class ResultPopup : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public float displayDuration = 1.2f;

    void Start()
    {
        resultText.text = "";
    }

    public void ShowResult(bool success)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(success));
    }
    
    private IEnumerator ShowRoutine(bool success)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = success ? "PASS!" : "FAIL!";
        resultText.color = success ? Color.green : Color.red;
        
        yield return new WaitForSeconds(displayDuration);
        
        resultText.gameObject.SetActive(false);
    }
}