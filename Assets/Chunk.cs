using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] blockData = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    int triIndex = 0;

    World world;
    ChunkCoordinate coord;
    GameObject obj;
    Vector3 chunkPosition;

    public bool isRendered
    {
        get { return obj.activeSelf; }
        set { obj.SetActive(value); }
    }

    Vector3 GetPositionVector(int x, int y, int z)
    {
        return chunkPosition + new Vector3(x, y, z);
    }

    public Chunk(World world,ChunkCoordinate coord)
    {
        this.world = world;
        this.coord = coord;


        chunkPosition = new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);

        mesh = new Mesh();
        obj = new GameObject($"Chunk {chunkPosition.x}, {chunkPosition.z}");
        obj.transform.parent = world.transform;
        obj.transform.position = chunkPosition;

        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = world.mat;

        InitBLock();
    }
    public void GenerateMesh()
    {
        GenerateBlock();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void InitBLock()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    blockData[x, y, z] = world.generationLogic.GetBlock(GetPositionVector(x,y,z));
                }
            }
        }
    }

    void GenerateBlock()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    GenerateVoxel(new Vector3Int(x, y, z));
                }
            }
        }
    }

    void GenerateVoxel(Vector3Int pos)
    {
        if (world.blocktypes[blockData[pos.x, pos.y, pos.z]].isSolid)
        {
            for (int f = 0; f < 6; f++)
            {
                if (IsFaceToDraw(pos + VoxelData.faceCheck[f]))
                {
                    for (int v = 0; v < 4; v++)
                    {
                        vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[f, v]]);
                    }
                    triangles.Add(triIndex);
                    triangles.Add(triIndex + 1);
                    triangles.Add(triIndex + 2);
                    triangles.Add(triIndex + 2);
                    triangles.Add(triIndex + 1);
                    triangles.Add(triIndex + 3);
                    AddUvs(world.blocktypes[blockData[pos.x,pos.y,pos.z]].GetFaceID(f));
                    triIndex += 4;
                }
            }
        }
    }

    void AddUvs(int f)
    {
        float x = f % VoxelData.textureSize;
        float y = (int)(f * VoxelData.blockTextureSize);
        x *= VoxelData.blockTextureSize;
        y *= VoxelData.blockTextureSize;
        y = 1 - y - VoxelData.blockTextureSize;
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.blockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.blockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.blockTextureSize, y + VoxelData.blockTextureSize));
    }

    bool IsFaceToDraw(Vector3Int pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.chunkWidth && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >= 0 && pos.z < VoxelData.chunkWidth)
            return !world.blocktypes[blockData[pos.x, pos.y, pos.z]].isSolid;
        return !world.blocktypes[world.generationLogic.GetBlock(GetPositionVector(pos.x, pos.y, pos.z))].isSolid;
    }
}


public class ChunkCoordinate
{
    public int x;
    public int z;
    public ChunkCoordinate(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public static bool IsEqual(ChunkCoordinate a, ChunkCoordinate b)
    {
        if(a.x!=b.x || a.z != b.z)
        {
            return false;
        }
        return true;
    }
}
