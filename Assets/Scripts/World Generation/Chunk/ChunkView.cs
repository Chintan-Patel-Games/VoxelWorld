using UnityEngine;

namespace VoxelWorld.WorldGeneration.Chunks
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ChunkView : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
        }
    }
}