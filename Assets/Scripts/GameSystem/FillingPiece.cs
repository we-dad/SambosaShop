using UnityEngine;

/// <summary>
/// A single filling object, Falls under gravity until it touches the dough,
/// then sticks to the nearest vertex and follows the fold
/// </summary>
public enum FillingType { Meat, Cheese }

public class FillingPiece : MonoBehaviour
{
    public FillingType type;
    private Rigidbody rb;
    
    private int stuckVertexIndex = -1;
    private SambosaMesh stuckTo;
    private Vector3 stickReferenceLocal;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    } 
    
// Blends the piece offset based on how far the stuck vertex has moved since
// the piece landed, Flat dough = piece sits on top, Folded dough = piece sinks
// inside, so the fold appears to wrap it
    void Update()
    {
        if (stuckTo != null && stuckVertexIndex >= 0)
        {
            Vector3 currentLocal = stuckTo.GetCurrentVertexLocal(stuckVertexIndex);
            float liftAmount = Vector3.Distance(stickReferenceLocal, currentLocal);

            float blend = Mathf.Clamp01(liftAmount * 4f);
            float offset = Mathf.Lerp(0.15f, -0.15f, blend);

            Vector3 worldVert = stuckTo.transform.TransformPoint(currentLocal);
            transform.position = worldVert + stuckTo.transform.up * offset;
        }
    }
    
// Freezes physics so the piece follows the cursor instead of falling
    public void Pickup()
    {
        stuckTo = null;
        stuckVertexIndex = -1;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
    public void Drop()
    {
        rb.isKinematic = false;
    }
    public void StickToDough(SambosaMesh dough, int vertexIndex)
    {
        stuckTo = dough;
        stuckVertexIndex = vertexIndex;
        stickReferenceLocal = dough.GetCurrentVertexLocal(vertexIndex);
    }
    
    // First touch handler, find the nearest vertex and lock to it
    void OnCollisionEnter(Collision collision)
    {
        SambosaMesh dough = collision.gameObject.GetComponent<SambosaMesh>();
        if (dough == null) return;
        if (stuckTo != null) return;

        Vector3 hitWorld = collision.contacts[0].point;
        Vector3 hitLocal = dough.transform.InverseTransformPoint(hitWorld);

        int closestIndex = -1;
        float best = float.MaxValue;
        int count = dough.GetVertexCount();
        for (int i = 0; i < count; i++)
        {
            Vector3 v = dough.GetCurrentVertexLocal(i);
            float d = Vector3.Distance(hitLocal, v);
            if (d < best)
            {
                best = d;
                closestIndex = i;
            }
        }

        if (closestIndex >= 0)
        {
            StickToDough(dough, closestIndex);
            rb.isKinematic = true; 
        }
    }
    
    public bool IsStuck()
    {
        return stuckTo != null;
    }
    
}
