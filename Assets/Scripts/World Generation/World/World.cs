using UnityEngine;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World
{
    public class World : MonoBehaviour
    {
        [Header("World Settings")]
        public int worldSeed = 12345;
        public int renderDistance = 4;
        public ChunkLoader chunkLoader;

        [Header("Atmosphere Settings")]
        public Color fogColor = new(0.7f, 0.8f, 0.9f);
        public bool useLinearFog = true;
        public float fogDensity = 0.02f;

        private void Awake()
        {
            Random.InitState(worldSeed);
            SetupFog();
        }

        public void SetupFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;

            if (useLinearFog)
            {
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = (renderDistance - 1) * Chunk.chunkSize;
                RenderSettings.fogEndDistance = renderDistance * Chunk.chunkSize * 1.2f;
            }
            else
            {
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = fogDensity;
            }

            Debug.Log($"Fog configured for worldSeed {worldSeed}");
        }

        public void ApplyBiomeFog(Color color, float startMultiplier, float endMultiplier)
        {
            RenderSettings.fogColor = color;
            RenderSettings.fogStartDistance = renderDistance * Chunk.chunkSize * startMultiplier;
            RenderSettings.fogEndDistance = renderDistance * Chunk.chunkSize * endMultiplier;
        }
    }
}