using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    public BiomData biomData;
    public Material mat;
    public BlockType[] blocktypes;


    public WorldGenerationLogic generationLogic;
    Chunk[,] chunk = new Chunk[VoxelData.worldSize, VoxelData.worldSize];
    List<ChunkCoordinate> activeChunk = new List<ChunkCoordinate>();
    ChunkCoordinate lastChunk;
    private void Start()
    {
        generationLogic = new WorldGenerationLogic(biomData);

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
        List<Chunk> renderList = new List<Chunk>();
        for (int x = coord.x - VoxelData.renderDistance; x < coord.x + VoxelData.renderDistance; x++)
        {
            for (int z = coord.z - VoxelData.renderDistance; z < coord.z + VoxelData.renderDistance; z++)
            {
                if (chunk[x, z] == null)
                {
                    ChunkCoordinate c = new ChunkCoordinate(x, z);
                    chunk[x, z] = new Chunk(this, c);
                    activeChunk.Add(c);
                    renderList.Add(chunk[x,z]);
                }
                else if (!chunk[x,z].isRendered)
                {
                    chunk[x,z].isRendered = true;
                    activeChunk.Add(new ChunkCoordinate(x, z));
                }
            }
        }
        int count = activeChunk.Count;
        for (int i = 0; i < count;)
        {
            int x = activeChunk[i].x;
            int z = activeChunk[i].z;
            if (x >= coord.x - VoxelData.renderDistance && x < coord.x + VoxelData.renderDistance && z >= coord.z - VoxelData.renderDistance && z < coord.z + VoxelData.renderDistance) i++;
            else
            {
                chunk[x, z].isRendered = false;
                activeChunk.RemoveAt(i);
                count--;
            }
        }
        GenerateChunkMesh(renderList.ToArray());
    }

    void GenerateChunkMesh(Chunk[] list)
    {
        foreach(Chunk chunk in list)
        {
            chunk.GenerateMesh();
        }
    }

    ChunkCoordinate GetChunkCoord(Vector3 pos)
    {
        return new ChunkCoordinate((int)pos.x / VoxelData.chunkWidth, (int)pos.z / VoxelData.chunkWidth);
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

public class WorldGenerationLogic
{
    BiomData BiomData;
    public WorldGenerationLogic(BiomData biom)
    {
        this.BiomData= biom;
    }
    public byte GetBlock(Vector3 pos)
    {
        float blockHeight = Mathf.PerlinNoise(pos.x / VoxelData.chunkWidth * .09f + .1f, pos.z / VoxelData.chunkWidth * .09f + .1f) * (BiomData.maxHeight-BiomData.minHeight);
        if (pos.y < blockHeight + BiomData.minHeight && !IsCave(pos, 3.1f))
            return 1;
        return 0;
    }
    bool IsCave(Vector3 pos,float scale)
    {
        float xCoord = pos.x / VoxelData.chunkWidth * scale;
        float yCoord = pos.y / VoxelData.chunkHeight * scale;
        float zCoord = pos.z / VoxelData.chunkWidth * scale;

        // Combine multiple layers of 2D Perlin Noise to approximate 3D noise
        float xy = Mathf.PerlinNoise(xCoord, yCoord);
        float xz = Mathf.PerlinNoise(xCoord, zCoord);
        float yz = Mathf.PerlinNoise(yCoord, zCoord);

        float noise = (xy + xz + yz) / 3;
        if (BiomData.caveTreshold < noise) return true;
        return false;
    }
}
