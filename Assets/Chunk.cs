using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer renderer;
    public MeshFilter filter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangle = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    private void Start()
    {
        for(int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
        UpdateMesh();
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for(int f = 0; f < 6; f++)
        {
            for (int p = 0; p < 6; p++)
            {
                int triIndex = VoxelData.voxelTris[f, p];
                vertices.Add(VoxelData.voxelVerts[triIndex]+pos);
                triangle.Add(vertexIndex);
                vertexIndex++;
            }
        }
    }

    void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangle.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }

}
