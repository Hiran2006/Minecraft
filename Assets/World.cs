using UnityEngine;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour
{
    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks,VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunk = new List<ChunkCoord>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    private void Start()
    {
        spawnPosition = new Vector3(VoxelData.WorldSizeInBlocks * .5f, VoxelData.ChunkHeight, VoxelData.WorldSizeInBlocks * .5f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(spawnPosition);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(transform.position);
        if (playerLastChunkCoord != playerChunkCoord)
        {
            CheckViewDistance();
        }
    }

    void GenerateWorld()
    {
        for (int x = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; x < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; z < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
        player.transform.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkCoord(x, z);
    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        List<ChunkCoord> previoslyActiveChunk = new List<ChunkCoord>(activeChunk);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                        CreateNewChunk(x, z);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunk.Add(new ChunkCoord(x, z));
                    }
                }

                for(int i = 0; i < previoslyActiveChunk.Count; i++)
                {
                    if (previoslyActiveChunk[i]==new ChunkCoord(x, z))
                    {
                        previoslyActiveChunk.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        foreach (var c in previoslyActiveChunk)
        {
            chunks[c.x, c.z].isActive = false;
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
        activeChunk.Add(new ChunkCoord(x, z));
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
