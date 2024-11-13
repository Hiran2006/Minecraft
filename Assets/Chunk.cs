using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    byte[,,] blockData = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    int triIndex = 0;

    World world;
    ChunkCoordinate coord;

    public Chunk(World world,ChunkCoordinate coord)
    {
        this.world = world;
        this.coord = coord;

        mesh = new Mesh();
        GameObject obj = new GameObject();
        obj.transform.parent = world.transform;
        obj.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);

        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = world.mat;

        InitBLock();
        GenerateBlock();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    Vector3 chunkPosition { get { return new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth); } }

    void InitBLock()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    blockData[x, y, z] = world.generationLogic.GetBlock(new Vector3(x + chunkPosition.x, y, z + chunkPosition.z));
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
        if (blockData[pos.x, pos.y, pos.z] != 0)
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
                    triIndex += 4;
                }
            }
        }
    }

    bool IsFaceToDraw(Vector3Int pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.chunkWidth && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >= 0 && pos.z < VoxelData.chunkWidth)
            return blockData[pos.x, pos.y, pos.z]==0;
        return world.generationLogic.GetBlock(new Vector3(chunkPosition.x + pos.x, pos.y, chunkPosition.z + pos.z)) == 0;
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
}
