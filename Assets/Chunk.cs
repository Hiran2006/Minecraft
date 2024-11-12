using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk:MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();


    int triIndex = 0;

    private void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
        GenerateBlock();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
    void GenerateBlock()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    GenerateVoxel(new Vector3(x, y, z));
                }
            }
        }
    }
    void GenerateVoxel(Vector3 pos)
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

    bool IsFaceToDraw(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.chunkWidth && pos.y >= 0 && pos.y < VoxelData.chunkWidth && pos.z >= 0 && pos.z < VoxelData.chunkWidth)
            return false;
        return true;
    }
}
