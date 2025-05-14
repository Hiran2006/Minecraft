using UnityEngine;
using System;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks,VoxelData.WorldSizeInChunks];

    private void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int x = 0; x < VoxelData.WorldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldSizeInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    public short GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;
        if (pos.y < 1)
            return 1;
        else if (pos.y == VoxelData.ChunkHeight - 1)
            return 3;
        else
            return 2;
    }

    void CreateNewChunk(int x,int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        return (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1);
    }
    bool IsVoxelInWorld(Vector3 pos)
    {
        return (pos.x >= 0 && pos.x < VoxelData.WorldSizeInBlocks && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInBlocks);
    }
}
[System.Serializable]
public class BlockType
{
    // front back right left top bottom
    public string name;
    public bool isSolid;
    [Header("Texture values")]
    [SerializeField] int frontFaceTexture;
    [SerializeField] int backFaceTexture;
    [SerializeField] int rightFaceTexture;
    [SerializeField] int leftFaceTexture; 
    [SerializeField] int topFaceTexture; 
    [SerializeField] int bottomFaceTexture;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return frontFaceTexture;
            case 1:
                return backFaceTexture;
            case 2:
                return rightFaceTexture;
            case 3:
                return leftFaceTexture;
            case 4:
                return topFaceTexture;
            case 5:
                return bottomFaceTexture;
            default:
                throw new Exception($"Index is Outoff range at texture face index {faceIndex}");
        }
    }
}
