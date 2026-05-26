using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles picking up and dropping filling pieces with the mouse
/// Press to grab, release to drop
/// </summary>
public class FillingInputHandler : MonoBehaviour
{ 
    
// Read by FoldInputHandler to suppress fold input while a piece is held
    public static bool IsHolding { get; private set; }
    
    public float hoverHeight = 1.5f;
    
    private FillingPiece held;
    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        
        if (GameStateMachine.Instance.CurrentState != SambosaGameState.Making) return;
        
        if (mouse.leftButton.wasPressedThisFrame && held == null)
        {
            TryPickup(mouse.position.ReadValue());
        }

        if (mouse.leftButton.wasReleasedThisFrame && held != null)
        {
            held.Drop();
            held = null;
            IsHolding = false;
        }
        
        if (held != null)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            Plane plane = new Plane(Vector3.up, new Vector3(0, hoverHeight, 0));
            if (plane.Raycast(ray, out float enter))
            {
                held.transform.position = ray.GetPoint(enter);
            }
        }
    }
    private void TryPickup(Vector2 mousePos)
    {
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("RAYCAST HIT: " + hit.collider.name);
            FillingPiece piece = hit.collider.GetComponent<FillingPiece>();
            if (piece != null)
            {
                piece.Pickup();
                held = piece;
                IsHolding = true;
                Debug.Log("PICKED UP");
            }
        }
        else
        {
            Debug.Log("RAYCAST HIT NOTHING");
        }
    }
}
