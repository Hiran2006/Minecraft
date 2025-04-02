using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material mat;


    Chunk[,] chunkData = new Chunk[WorldDate.worldWidth, WorldDate.worldWidth];

    public Transform player;
    [SerializeField]
    public BlockType[] blockType;

    private void Start()
    {
        player.position = WorldDate.worldWidth * .5f * (Vector3.right + Vector3.forward) + 90* Vector3.up;
        player.gameObject.SetActive(false);
        InitialRender();
        player.gameObject.SetActive(true);
    }

    void InitialRender()
    {
        ChunkCoordinate playerChunk = GetChunkCoord(player.position);
        for (int x = playerChunk.x - WorldDate.renderDis; x < playerChunk.x + WorldDate.renderDis; x++)
        {
            for (int z = playerChunk.z - WorldDate.renderDis; z < playerChunk.z + WorldDate.renderDis; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    public byte GetBlock(Vector3 block)
    {
        int x = (int)block.x;
        int y = (int)block.y;
        int z = (int)block.z;
        int bX = x % WorldDate.chunkWidth;
        int bY = y % WorldDate.chunkHeight;
        int bZ = z % WorldDate.chunkWidth;

        ChunkCoordinate coord = GetChunkCoord(block);

        return chunkData[coord.x, coord.z].blockData[bX, bY, bZ];

    }

    public ChunkCoordinate GetChunkCoord(Vector3 pos)
    {
        int x = (int)(pos.x / WorldDate.chunkWidth);
        int z = (int)(pos.z / WorldDate.chunkWidth);
        return new ChunkCoordinate(x, z);
    }

    void CreateNewChunk(int x, int z)
    {
        chunkData[x,z] = new Chunk(this,new ChunkCoordinate(x,z));
    }

    public byte GetVoxelData(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;
        float noise = Mathf.PerlinNoise((x + .1f) * .009f, (z + .1f) * .009f);
        if(y < noise * WorldDate.chunkHeight)
        {
            return 2;
        }
        return 0;
    }
}


[Serializable]
public class BlockType
{
    [SerializeField] string name;
    public bool isSolid = true;

    [SerializeField] byte frontFace;
    [SerializeField] byte backFace;
    [SerializeField] byte rightFace;
    [SerializeField] byte leftFace;
    [SerializeField] byte topFace;
    [SerializeField] byte bottomFace;

    public byte GetTextureIndex(int index)
    {
        switch (index)
        {
            case 0:return frontFace;
            case 1:return backFace;
            case 2:return rightFace;
            case 3:return leftFace;
            case 4:return topFace;
            case 5:return bottomFace;
            default:
                throw new Exception("The Texture Index is not found");
        }
    }
}
