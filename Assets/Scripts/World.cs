using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
    public Transform player;
    public Material material;
    public BiomeAttribute biome;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<Chunk> activeChunk = new List<Chunk>();
    List<Chunk> chunkToCreate = new List<Chunk>();

    ChunkCoord playerLastChunkCoord;
    Vector3 playerlastPos;

    bool isCreating = false;

    void Start()
    {
        GenerateWorld();
    }

    void Update()
    {
        if (((int)player.position.x != (int)playerlastPos.x || (int)player.position.z != (int)playerlastPos.z))
        {
            ChunkCoord currentChunkCoord = GetChunkCoordFromPos(player.position);
            if (currentChunkCoord != playerLastChunkCoord)
            {
                playerLastChunkCoord = currentChunkCoord;
                ReRenderChunks();
            }
            playerlastPos = player.position;
        }
    }

    ChunkCoord GetChunkCoordFromPos(Vector3 pos)
    {
        return new ChunkCoord(Mathf.FloorToInt(pos.x / VoxelData.chunkWidth), Mathf.FloorToInt(pos.z / VoxelData.chunkWidth));
    }

    private void GenerateWorld()
    {
        player.transform.position = new Vector3(VoxelData.worldSizeInChunks * VoxelData.chunkWidth / 2, VoxelData.chunkHeight - 50f, VoxelData.worldSizeInChunks * VoxelData.chunkWidth / 2);
        playerLastChunkCoord = GetChunkCoordFromPos(player.position);
        for (int x = VoxelData.worldSizeInChunks / 2 - VoxelData.renderDistanceInChunks; x <= VoxelData.worldSizeInChunks / 2 + VoxelData.renderDistanceInChunks; x++)
        {
            for (int z = VoxelData.worldSizeInChunks / 2 - VoxelData.renderDistanceInChunks; z <= VoxelData.worldSizeInChunks / 2 + VoxelData.renderDistanceInChunks; z++)
            {
                CreateChunk(x, z);
            }
        }

        StartCoroutine(LoadNewChunk());
    }

    void ReRenderChunks()
    {
        for (int i = 0; i < activeChunk.Count; i++)
        {
            if (playerLastChunkCoord.x - VoxelData.renderDistanceInChunks > activeChunk[i].coord.x || playerLastChunkCoord.x + VoxelData.renderDistanceInChunks < activeChunk[i].coord.x ||
                playerLastChunkCoord.z - VoxelData.renderDistanceInChunks > activeChunk[i].coord.z || playerLastChunkCoord.z + VoxelData.renderDistanceInChunks < activeChunk[i].coord.z)
            {
                activeChunk[i].isActive = false;
                activeChunk.RemoveAt(i);
                i--;
            }
        }
        for (int x = playerLastChunkCoord.x - VoxelData.renderDistanceInChunks; x <= playerLastChunkCoord.x + VoxelData.renderDistanceInChunks; x++)
        {
            for (int z = playerLastChunkCoord.z - VoxelData.renderDistanceInChunks; z <= playerLastChunkCoord.z + VoxelData.renderDistanceInChunks; z++)
            {
                if (x >= 0 && x < VoxelData.worldSizeInChunks && z >= 0 && z < VoxelData.worldSizeInChunks)
                {
                    if (chunks[x, z] == null)
                    {
                        CreateChunk(x, z);
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunk.Add(chunks[x, z]);
                    }
                }
            }
        }
        if (!isCreating)
            StartCoroutine(LoadNewChunk());
    }

    IEnumerator LoadNewChunk()
    {
        isCreating = true;
        while (chunkToCreate.Count > 0)
        {
            chunkToCreate[0].Init();
            chunkToCreate.RemoveAt(0);
        }
        isCreating = false;
        yield return null;
    }

    public void EditChunk(Vector3 pos, byte newBlock)
    {
        if (!IsBlockInWorld(pos)) return;
        ChunkCoord coord = GetChunkCoordFromPos(pos);
        int xblock = (int)(pos.x - coord.x * VoxelData.chunkWidth);
        int zblock = (int)(pos.z - coord.z * VoxelData.chunkWidth);
        chunks[coord.x, coord.z].blocks[xblock, (int)pos.y, zblock] = newBlock;
        chunks[coord.x, coord.z].UpdateChunk();
        if (xblock == 0)
        {
            chunks[coord.x - 1, coord.z].UpdateChunk();
        }
        else if (xblock == VoxelData.chunkWidth - 1)
        {
            chunks[coord.x + 1, coord.z].UpdateChunk();
        }
        if (zblock == 0)
        {
            chunks[coord.x, coord.z - 1].UpdateChunk();
        }
        else if (zblock == VoxelData.chunkWidth - 1)
        {
            chunks[coord.x, coord.z + 1].UpdateChunk();
        }
    }

    private void CreateChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunk.Add(chunks[x, z]);
        chunkToCreate.Add(chunks[x, z]);
    }

    public bool IsSolidBlock(Vector3 pos)
    {

        if (!IsBlockInWorld(pos)) return false;
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);
        ChunkCoord coord = GetChunkCoordFromPos(new Vector3(xCheck, yCheck, zCheck));
        int xblock = xCheck - coord.x * VoxelData.chunkWidth;
        int zblock = zCheck - coord.z * VoxelData.chunkWidth;
        return blockTypes[chunks[coord.x, coord.z].blocks[xblock, yCheck, zblock]].isSolid;
    }

    public bool IsBlockInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.worldSizeInChunks * VoxelData.chunkWidth && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >= 0 && pos.z < VoxelData.worldSizeInChunks * VoxelData.chunkWidth)
            return true;
        else
            return false;
    }

    public bool IsTransparent(Vector3 pos)
    {
        if (!IsBlockInWorld(pos)) return true;
        ChunkCoord coord = GetChunkCoordFromPos(pos);
        int xblock = (int)(pos.x - coord.x * VoxelData.chunkWidth);
        int zblock = (int)(pos.z - coord.z * VoxelData.chunkWidth);
        if (chunks[coord.x, coord.z] == null) return blockTypes[GetBlockMap(pos)].isTransparent;
        return blockTypes[chunks[coord.x, coord.z].blocks[xblock, (int)pos.y, zblock]].isTransparent;
        
    }
    public byte GetBlockMap(Vector3 pos)
    {
        if (pos.y < 0 || pos.y >= VoxelData.chunkHeight)
            return 0;
        if (pos.y == 0)
            return 1;

        int terrianHeight = (int)(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0.1f, biome.terrianScale) * biome.terrianHeight) + biome.solidGroundHeight;
        if (terrianHeight == pos.y)
            return 4;
        if(pos.y<terrianHeight)
            return 2;
        return 0;
    } 
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
    public bool isTransparent;
    public Sprite icon;
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