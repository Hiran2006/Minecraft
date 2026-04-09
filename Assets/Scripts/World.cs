using System.Collections.Generic;
using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    public Material material;
    public BiomeAttribute biome;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<Chunk> activeChunk = new List<Chunk>();

    ChunkCoord playerLastChunkCoord;
    Vector3 playerlastPos;

    void Start()
    {
        GenerateWorld();
    }

    void Update()
    {
        //if (((int)player.position.x != (int)playerlastPos.x || (int)player.position.z != (int)playerlastPos.z))
        //{
        //    ChunkCoord currentChunkCoord = GetChunkCoordFromPos(player.position);
        //    if (currentChunkCoord != playerLastChunkCoord)
        //    {
        //        playerLastChunkCoord = currentChunkCoord;
        //        ReRenderChunks();
        //    }
        //    playerlastPos = player.position;
        //}
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
    private void CreateChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunk.Add(chunks[x, z]);
    }

    public byte GetBlockMap(Vector3 pos)
    {
        if (pos.y < 0 || pos.y >= VoxelData.chunkHeight)
            return 0;
        if (pos.y == 0)
            return 1;

        if (!Noise.Get3DPerlinBool(pos, 0.1f, biome.terrianScale, 0.5f))
        {
            int terrianHeight = (int)(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0.1f, biome.terrianScale) * biome.terrianHeight) + biome.solidGroundHeight;
            if (terrianHeight == pos.y)
                return 4;
            if (terrianHeight > pos.y)
                return 2;
        }
        return 0;
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