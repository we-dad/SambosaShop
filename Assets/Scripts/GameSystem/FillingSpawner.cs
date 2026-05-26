using UnityEngine;

/// <summary>
/// Spawns the initial meat and cheese pieces above each plate at game start
/// </summary>
public class FillingSpawner : MonoBehaviour
{
    public FillingPiece meatPrefab;
    public FillingPiece cheesePrefab;
    public Transform meatPlate;
    public Transform cheesePlate;
    
    public int meatCount = 5;
    public int cheeseCount = 5;
    public float spawnHeight = 1f;
    
    public float spread = 0.6f;
    
    void Start()
    {
        for (int i = 0; i < meatCount; i++)
        {
            Vector3 pos = meatPlate.position + new Vector3(Random.Range(-spread, spread), spawnHeight, Random.Range(-spread, spread));
            Instantiate(meatPrefab, pos, Quaternion.identity);
        }
        for (int i = 0; i < cheeseCount; i++)
        {
            Vector3 pos = cheesePlate.position + new Vector3(Random.Range(-spread, spread), spawnHeight, Random.Range(-spread, spread));
            Instantiate(cheesePrefab, pos, Quaternion.identity);
        }
    }

}
