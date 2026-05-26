using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads mouse input and converts an edge drag into fold parameters,
/// then calls SambosaMesh.ApplyFold
/// </summary>
[RequireComponent(typeof(SambosaMesh))]
public class FoldInputHandler : MonoBehaviour
{
    
    public float grabPixelRadius = 180f;  // how close in pixels to count as a grab
    public float foldAngle = 180f;        // how far to fold (180 = full)
    
    private SambosaMesh dough;       // the mesh we control
    private Camera cam;              // cached camera
    private Vector3 grabbedPoint;    // where the drag started (local space)
    private bool dragging;           // are we currently dragging?
    
    
    void Start()
    {
        dough = GetComponent<SambosaMesh>();
        cam = Camera.main;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        
        if (FillingInputHandler.IsHolding) return; 
        
        if (GameStateMachine.Instance.CurrentState != SambosaGameState.Making) return;
        
        //press
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("FOLD INPUT — wasPressed, IsHolding=" + FillingInputHandler.IsHolding);   // ← add this
            if (TryGrab(mouse.position.ReadValue()))
            {
                dragging = true;
            }
        }
        
        //grab
        if (dragging && mouse.leftButton.isPressed)
        {
            if (GetMouseOnDoughPlane(mouse.position.ReadValue(), out Vector3 mouseLocal))
            {
                UpdateFoldFromDrag(mouseLocal);
            }
        }
        
        //release
        if (mouse.leftButton.wasReleasedThisFrame && dragging)
        {
            dough.CommitFold();
            dragging = false;
        }
    }

    private bool TryGrab(Vector2 mousePos)
    {
        float best = float.MaxValue;
        int bestIndex = -1;

        for (int i = 0; i < dough.GetVertexCount(); i++)
        {
            if (!dough.IsEdgeVertex(i)) continue;
            
            Vector3 local = dough.GetBaseVertexLocal(i);
            Vector3 world = transform.TransformPoint(local);
            Vector3 screen = cam.WorldToScreenPoint(world);
            
            if (screen.z < 0) continue;
            
            float d = Vector2.Distance(mousePos, new Vector2(screen.x, screen.y));

            if (d < best)
            {
                best = d;
                bestIndex = i;
            }
        }
        
        if (best < grabPixelRadius)
        {
            grabbedPoint = dough.GetBaseVertexLocal(bestIndex);
            return true;
        }

        return false;
    }

    private bool GetMouseOnDoughPlane(Vector2 mousePos, out Vector3 localPoint)
    {
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        Plane plane = new Plane(transform.up, transform.position);
        
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 world = ray.GetPoint(enter);
            
            localPoint = transform.InverseTransformPoint(world);
            return true;
        }
        
        localPoint = Vector3.zero;
        return false;
    }

    private void UpdateFoldFromDrag(Vector3 mouseLocal)
    {
        Vector3 toMouse = mouseLocal - grabbedPoint;
        
        if (toMouse.magnitude < 0.05f) return;
        
        Vector3 foldPoint = (grabbedPoint + mouseLocal) * 0.5f;
        Vector3 foldNormal = (grabbedPoint - mouseLocal).normalized;
        Vector3 foldDir = Vector3.Cross(Vector3.up, foldNormal).normalized;
        
        if (foldDir.magnitude < 0.01f) foldDir = Vector3.right;
        
        dough.ApplyFold(foldPoint, foldNormal, foldDir, foldAngle);
    }
}
