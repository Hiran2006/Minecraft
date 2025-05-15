using UnityEngine;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biome;

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
        player.transform.position = spawnPosition;
        playerLastChunkCoord = GetChunkCoordFromVector3(spawnPosition);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if (playerLastChunkCoord != playerChunkCoord)
        {
            CheckViewDistance();
            playerLastChunkCoord = playerChunkCoord;
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

        for (int i = 0; i < activeChunk.Count; i++)
        {
            ChunkCoord c = activeChunk[i];
            if (c.x < coord.x - VoxelData.ViewDistanceInChunks || c.x > coord.x + VoxelData.ViewDistanceInChunks
                || c.z < coord.z - VoxelData.ViewDistanceInChunks || c.z > coord.z + VoxelData.ViewDistanceInChunks)
            {

                chunks[c.x, c.z].isActive = false;
                activeChunk.RemoveAt(i);
                i--;
            }
        }

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x <= coord.x + VoxelData.ViewDistanceInChunks; x++) 
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z <= coord.z + VoxelData.ViewDistanceInChunks; z++) 
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
            }
        }
    }

    public bool CheckForVoxel(float _x, float _y, float _z)
    {
        int xCheck = Mathf.FloorToInt(_x);
        int yCheck = Mathf.FloorToInt(_y);
        int zCheck = Mathf.FloorToInt(_z);

        int xChunk = xCheck / VoxelData.ChunkWidth;
        int zChunk = zCheck / VoxelData.ChunkWidth;

        xCheck -= xChunk * VoxelData.ChunkWidth;
        zCheck -= zChunk * VoxelData.ChunkWidth;

        return blockTypes[chunks[xChunk, zChunk].voxelMap[xCheck, yCheck, zCheck]].isSolid;
    }

    public ushort GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);
        if (!IsVoxelInWorld(pos))
            return 0;

        if (yPos == 0)
            return 1;

        // Basic Terrain Generation
        int terrainHeight = Mathf.FloorToInt(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale) * (biome.terrainHeight - biome.solidGroundHeight));
        ushort voxelValue = 0;

        terrainHeight += biome.solidGroundHeight;
        if (yPos == terrainHeight)
            voxelValue = 2;
        else if (yPos < terrainHeight && yPos > terrainHeight - 3)
            voxelValue = 3;
        else if (yPos < terrainHeight)
            voxelValue = 4;
        if(voxelValue == 4  )
        {
            foreach(var lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
            }
        }
        return voxelValue;

    }

    void CreateNewChunk(int x,int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunk.Add(new ChunkCoord(x, z));
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        return (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks);
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
    [SerializeField] ushort frontFaceTexture;
    [SerializeField] ushort backFaceTexture;
    [SerializeField] ushort rightFaceTexture;
    [SerializeField] ushort leftFaceTexture; 
    [SerializeField] ushort topFaceTexture; 
    [SerializeField] ushort bottomFaceTexture;

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
