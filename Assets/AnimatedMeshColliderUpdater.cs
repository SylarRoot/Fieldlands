using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer), typeof(MeshCollider))]
public class AnimatedMeshColliderUpdater : MonoBehaviour
{
    SkinnedMeshRenderer skinned;
    MeshCollider meshCol;
    Mesh[] meshes = new Mesh[2]; // Double buffer for mesh data
    int currentIndex = 0;

    void Awake()
    {
        skinned = GetComponent<SkinnedMeshRenderer>();
        meshCol = GetComponent<MeshCollider>();
        meshCol.convex = true; // Convex is required for dynamic Rigidbodies

        // Initialize both meshes in the buffer
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i] = new Mesh();
        }
    }

    void LateUpdate()
    {
        // Verify component references
        if (skinned == null || meshCol == null)
        {
            Debug.LogError("Missing components: SkinnedMeshRenderer or MeshCollider not set up.");
            return;
        }

        // Determine which mesh to use this frame
        int meshIndex = currentIndex;
        currentIndex = (currentIndex + 1) % meshes.Length;

        // Bake the skinned mesh into the current mesh
        skinned.BakeMesh(meshes[meshIndex]);

        // Verify valid mesh data
        if (meshes[meshIndex].vertexCount == 0)
        {
            Debug.LogError("Baked mesh has no vertices. Check SkinnedMeshRenderer setup.");
            return;
        }

        // Update the collider with the new mesh
        meshCol.sharedMesh = meshes[meshIndex];
    }

    void OnDestroy()
    {
        // Cleanup allocated meshes
        foreach (Mesh mesh in meshes)
        {
            if (mesh != null)
            {
                Destroy(mesh);
            }
        }
    }
}