using UnityEngine;

/// <summary>
/// Procedural dough mesh, Builds a flat grid, applies folds by rotating vertices
/// around a crease line, and animates vertices toward their target each frame
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SambosaMesh : MonoBehaviour
{
    public int gridSize = 20;
    public float sambosaWidth = 2f;
    public float sambosaHeight = 6f;
    public float smoothSpeed = 60f; //speed of the anmation
    
    private Mesh mesh;
    
// Three vertex arrays: baseVerts is the resting shape, targetVerts is the fold's
// destination, verts is what gets rendered, Each frame, verts lerps toward targetVerts
    private Vector3[] verts;
    private Vector3[] baseVerts;
    private Vector3[] targetVerts;
    
    private int[] xs;
    private int[] zs;

    void Awake()
    {
        CreateMesh();
    }
    void Update()
    {
        SmoothApply();
    }

    public void CreateMesh()
    {
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        int vertsPerSide = gridSize + 1;
        verts = new Vector3[vertsPerSide * vertsPerSide];
        
        xs = new int[vertsPerSide * vertsPerSide];
        zs = new int[vertsPerSide * vertsPerSide];

        float halfWidth = sambosaWidth * 0.5f;
        float halfHigh = sambosaHeight * 0.5f;
        int v = 0;

        for (int z = 0; z <= gridSize; z++)
        {
            float pz = Mathf.Lerp(-halfHigh, +halfHigh, (float)z / gridSize);

            for (int x = 0; x <= gridSize; x++)
            {
                float px = Mathf.Lerp(-halfWidth, +halfWidth, (float)x / gridSize);

                verts[v] = new Vector3(px, 0, pz);
                
                xs[v] = x;
                zs[v] = z;
                
                v++;
            }
        }

        baseVerts = verts.Clone() as Vector3[];
        targetVerts = verts.Clone() as Vector3[];
        
        int[] tris = new int[gridSize * gridSize * 6];

        int t = 0;
        for (int z = 0; z < gridSize; z++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int i = z * vertsPerSide + x;

                // Triangle 1
                tris[t++] = i;
                tris[t++] = i + vertsPerSide;
                tris[t++] = i + 1;

                // Triangle 2
                tris[t++] = i + 1;
                tris[t++] = i + vertsPerSide;
                tris[t++] = i + vertsPerSide + 1;
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }

    }
    
// Vertices on the positive side of foldNormal rotate around the crease
// vertices on the negative side stay put
    public void ApplyFold(Vector3 foldPoint, Vector3 foldNormal , Vector3 foldDir , float angle)
    {
        for (int i = 0; i < baseVerts.Length; i++)
        {
            Vector3 p = baseVerts[i];
            float side = Vector3.Dot(p - foldPoint, foldNormal);
            if (side <= 0)
            {
                targetVerts[i] = p;
            }
            else
            {
                Quaternion rot = Quaternion.AngleAxis(angle, foldDir);
                targetVerts[i] = foldPoint + rot * (p - foldPoint);
            }
            
        }
    }
// Per frame interpolation toward targetVerts Also updates the MeshCollider.
    private void SmoothApply()
    {
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = Vector3.Lerp(verts[i], targetVerts[i], Time.deltaTime * smoothSpeed);
        }
        
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var col = GetComponent<MeshCollider>();
        if (col != null)
        {
            col.sharedMesh = null;
            col.sharedMesh = mesh;
        }
    }
    
    public void CommitFold()
    {
        for (int i = 0; i < verts.Length; i++)
        {
            baseVerts[i] = verts[i];
        }
    }
    
    public int GetVertexCount()
    {
        return baseVerts.Length;
    }

    public Vector3 GetBaseVertexLocal(int index)
    {
        return baseVerts[index];
    }
    
    public bool IsEdgeVertex(int index)
    {
        return xs[index] == 0 || xs[index] == gridSize ||
               zs[index] == 0 || zs[index] == gridSize;
    }
    
    public Vector3 GetCurrentVertexLocal(int index)
    {
        return verts[index];
    }
    public void ResetToFlat()
    {
        if (baseVerts == null) return;
        
        int vertsPerSide = gridSize + 1;
        float halfWidth = sambosaWidth * 0.5f;
        float halfHigh = sambosaHeight * 0.5f;
    
        int v = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            float pz = Mathf.Lerp(-halfHigh, +halfHigh, (float)z / gridSize);
            for (int x = 0; x <= gridSize; x++)
            {
                float px = Mathf.Lerp(-halfWidth, +halfWidth, (float)x / gridSize);
                Vector3 flat = new Vector3(px, 0, pz);
                baseVerts[v] = flat;
                targetVerts[v] = flat;
                v++;
            }
        }
    }
}
