using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.XR;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    public Material mat;
    public BlockType[] blocktypes;


    public WorldGenerationLogic generationLogic = new();
    Chunk[,] chunk = new Chunk[VoxelData.worldSize, VoxelData.worldSize];
    List<ChunkCoordinate> activeChunk = new List<ChunkCoordinate>();
    ChunkCoordinate lastChunk;
    private void Start()
    {
        player.position = new Vector3(VoxelData.worldSize * VoxelData.chunkWidth / 2, VoxelData.chunkHeight + 2f, VoxelData.worldSize * VoxelData.chunkWidth / 2);
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
        List<ChunkCoordinate> renderList = new List<ChunkCoordinate>();
        for (int x = coord.x - VoxelData.renderDistance; x < coord.x + VoxelData.renderDistance; x++)
        {
            for (int z = coord.z - VoxelData.renderDistance; z < coord.z + VoxelData.renderDistance; z++)
            {
                if (chunk[x, z] == null)
                {
                    renderList.Add(new ChunkCoordinate(x, z));
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
        StartCoroutine(GenerateChunk(renderList.ToArray()));
    }

    IEnumerator GenerateChunk(ChunkCoordinate[] list)
    {
        foreach(ChunkCoordinate coord in list)
        {
            activeChunk.Add(coord);
            chunk[coord.x, coord.z] = new Chunk(this, coord);
        }
        yield return null;
    }

    ChunkCoordinate GetChunkCoord(Vector3 pos)
    {
        return new ChunkCoordinate((int)pos.x / VoxelData.chunkWidth, (int)pos.z / VoxelData.chunkWidth);
    }

    public class WorldGenerationLogic
    {
        public byte GetBlock(Vector3 pos)
        {
            float noise = Mathf.PerlinNoise(pos.x/VoxelData.chunkWidth * .09f + .1f, pos.z/VoxelData.chunkWidth * .09f + .1f) * VoxelData.chunkHeight;
            if (pos.y < 50 || (pos.y < noise && pos.y < 100))
                return 1;
            return 0;
        }
    }

    [System.Serializable]
    public class BlockType
    {
        public string blockName;
        public bool isSolid;
        public byte frontFace;
        public byte backFace;
        public byte rightFace;
        public byte leftFace;
        public byte topFace;
        public byte bottomFace;

        public byte GetFaceID(int faceID)
        {
            switch (faceID)
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
                    Debug.Log("No face correspond to faceID " + faceID);
                    return 0;
            }
        }
    }
}
