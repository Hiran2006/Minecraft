using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];

    void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < VoxelData.worldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.worldSizeInChunks; z++)
            {
                CreateChunk(x, z);
            }
        }
    }

    private void CreateChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
    [Header("Texture Indices")]
    public byte frontFace;
    public byte backFace;
    public byte rightFace;
    public byte leftFace;
    public byte topFace;
    public byte bottomFace;

    public byte GetFaceIndex(int face)
    {
        switch (face)
        {
            case 0:
                return frontFace;
            case 1:
                return backFace;
            case 2:
                return rightFace;
            case 3:
                return leftFace;
            case 4:
                return topFace;
            case 5:
                return bottomFace;
            default:
                Debug.LogError("Invalid face index: " + face);
                return 0;
        }
    }
}