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
        player.position = (Vector3.right + Vector3.forward) * WorldDate.worldWidth * .5f;
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

    ChunkCoordinate GetChunkCoord(Vector3 pos)
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
        int height = (int)(noise * WorldDate.chunkHeight);
        if (y > height)
        {
            return 0;
        }
        else if (y > height - 3)
            return 2;
        return 1;
    }
}


[Serializable]
public class BlockType
{
    [SerializeField] string name;
    [SerializeField] bool isSolid = true;

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
