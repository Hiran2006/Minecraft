using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    public Material mat;
    public WorldGenerationLogic generationLogic = new();

    Chunk[,] chunk = new Chunk[VoxelData.worldSize, VoxelData.worldSize];
    List<ChunkCoordinate> activeChunk = new List<ChunkCoordinate>();
    ChunkCoordinate lastChunk;
    private void Start()
    {
        player.position = new Vector3(VoxelData.worldSize * VoxelData.chunkWidth / 2, VoxelData.chunkHeight, VoxelData.worldSize * VoxelData.chunkWidth / 2);
        lastChunk = GetChunkCoord(player.position);
        RenderAndUnrenderWorld(lastChunk);
    }
    private void Update()
    {
        ChunkCoordinate playerChunk = GetChunkCoord(player.position);
        if (!ChunkCoordinate.IsEqual(playerChunk, lastChunk))
        {
            RenderAndUnrenderWorld(playerChunk);
            lastChunk = playerChunk;
        }
        
    }
    void RenderAndUnrenderWorld(ChunkCoordinate coord)
    {
        for (int x = coord.x - VoxelData.renderDistance; x < coord.x + VoxelData.renderDistance; x++)
        {
            for (int z = coord.z - VoxelData.renderDistance; z < coord.z + VoxelData.renderDistance; z++)
            {
                if (chunk[x, z] == null)
                {
                    GenerateChunk(new ChunkCoordinate(x, z));
                }
                else if (!chunk[x,z].isRendered)
                {
                    chunk[x,z].isRendered = true;
                    activeChunk.Add(new ChunkCoordinate(x, z));
                }
            }
        }
        for (int i = 0; i < activeChunk.Count;)
        {
            int x = activeChunk[i].x;
            int z = activeChunk[i].z;
            if (x >= coord.x - VoxelData.renderDistance && x < coord.x + VoxelData.renderDistance && z >= coord.z - VoxelData.renderDistance && z < coord.z + VoxelData.renderDistance) i++;
            else
            {
                chunk[x, z].isRendered = false;
                activeChunk.RemoveAt(i);
            }
        }
    }

    void GenerateChunk(ChunkCoordinate coord)
    {
        activeChunk.Add(coord);
        chunk[coord.x, coord.z] = new Chunk(this, coord);
    }

    ChunkCoordinate GetChunkCoord(Vector3 pos)
    {
        return new ChunkCoordinate((int)pos.x / VoxelData.chunkWidth, (int)pos.z / VoxelData.chunkWidth);
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
