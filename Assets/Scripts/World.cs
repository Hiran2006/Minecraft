using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biome;
    public Transform debugScreen;
    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks,VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunk = new List<ChunkCoord>();
    public ChunkCoord playerCurrentChunkCoord;
    ChunkCoord playerLastChunkCoord;

    Queue<ChunkCoord> chunksToCreate = new Queue<ChunkCoord>();

    bool isChunkCreating = false;

    private void Start()
    {
        spawnPosition = new Vector3(VoxelData.WorldSizeInBlocks * .5f, VoxelData.ChunkHeight, VoxelData.WorldSizeInBlocks * .5f);
        GenerateWorld();
        player.transform.position = spawnPosition;
        playerLastChunkCoord = GetChunkCoordFromVector3(spawnPosition);
    }

    private void Update()
    {
        playerCurrentChunkCoord = GetChunkCoordFromVector3(player.position);

        if (playerLastChunkCoord != playerCurrentChunkCoord)
        {
            CheckViewDistance();
            playerLastChunkCoord = playerCurrentChunkCoord;
        }

        if (chunksToCreate.Count > 0 && !isChunkCreating)
            StartCoroutine("CreateChunks");

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.gameObject.SetActive(!debugScreen.gameObject.activeSelf);
    }

    void GenerateWorld()
    {
        for (int x = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; x < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; z < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; z++)
            {
                ChunkCoord c = new ChunkCoord(x, z);
                chunks[x, z] = new Chunk(c, this, true);
                activeChunk.Add(c);
            }
        }
    }

    IEnumerator CreateChunks()
    {
        isChunkCreating = true;
        while (chunksToCreate.Count > 0)
        {
            ChunkCoord c = chunksToCreate.Dequeue();
            chunks[c.x, c.z].Init();
            yield return null;
        }
        isChunkCreating = false;
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
                    ChunkCoord c= new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(c, this, false);
                        chunksToCreate.Enqueue(c);
                    }

                    else if (!chunks[x, z].isActive)
                        chunks[x, z].isActive = true;

                    activeChunk.Add(c);
                }
            }
        }
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        return blockTypes[GetVoxel(pos)].isSolid;
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
