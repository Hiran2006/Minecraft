using TreeEditor;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material mat;
    public WorldGenerationLogic generationLogic = new();
    private void Start()
    {
        new Chunk(this,new ChunkCoordinate(0,0));
    }

    void RenderWorld()
    {
        
    }

    public class WorldGenerationLogic
    {
        public byte GetBlock(Vector3 pos)
        {
            float noise = Mathf.PerlinNoise(pos.x * .01f + .1f, pos.z * .01f + .1f) * VoxelData.chunkHeight;
            if (pos.y < noise)
                return 1;
            return 0;
        }
    }
}
