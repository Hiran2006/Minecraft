using TreeEditor;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material mat;
    public WorldGenerationLogic generationLogic = new();
    private void Start()
    {
        RenderWorld(GetChunkCoord(transform.position));
    }

    void RenderWorld(ChunkCoordinate coord)
    {
        for (int x = coord.x - VoxelData.renderDistance; x < coord.x + VoxelData.renderDistance; x++)
        {
            for (int z = coord.z - VoxelData.renderDistance; z < coord.z + VoxelData.renderDistance; z++)
            {
                GenerateChunk(new ChunkCoordinate(x, z));
            }
        }
    }

    void GenerateChunk(ChunkCoordinate coord)
    {
        new Chunk(this, coord);
    }

    ChunkCoordinate GetChunkCoord(Vector3 pos)
    {
        return new ChunkCoordinate((int)pos.x, (int)pos.z);
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
