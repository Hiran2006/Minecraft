using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer renderer;
    public MeshFilter filter;
    public World world;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private void Start()
    {
        PopulateVoxelMap();
        UpdateMeshData();
        UpdateMesh();
    }
    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = 0;
                }
            }
        }
    }

    void UpdateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;

        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for(int f = 0; f < 6; f++)
        {
            if (!CheckVoxel(pos + VoxelData.faceCheck[f]))
            {
                for (int p = 0; p < 4; p++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[f, p]]);
                }

                AddTexture(0);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+3);

                vertexIndex += 4;
            }
        }
    }

    void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }

    void AddTexture(int textureID)
    {
        float y = (int)(textureID / VoxelData.textureAtlasSizeInBlocks);
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);
        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1 - y - VoxelData.NormalizedBlockTextureSize;

        for(int i =0; i < 4; i++)
        {
            uvs.Add(new Vector2(x, y) + VoxelData.voxelUvs[i] * VoxelData.NormalizedBlockTextureSize);
        }
    }

}
