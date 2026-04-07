using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
    Mesh mesh;
    World world;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] blocks = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    int vertIndex = 0;
    public Chunk(World world)
    {
        this.world = world;

        GameObject chunkObj = new GameObject();
        mesh = new Mesh();
        chunkObj.AddComponent<MeshFilter>().mesh = mesh;
        chunkObj.AddComponent<MeshRenderer>().material = world.material;

        InitBlocks();
        CreateBlocks();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void InitBlocks()
    {

        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    blocks[x, y, z] = 0;
                }
            }
        }
    }

    private void CreateVoxel(Vector3 pos)
    {
        for (int f = 0; f < 6; f++)
        {
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[f, i]]);
            }

            triangles.Add(vertIndex);
            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 2);
            triangles.Add(vertIndex + 2);
            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 3);

            vertIndex += 4;

            AddUvs(world.blockTypes[blocks[(int)pos.x, (int)pos.y, (int)pos.z]].GetFaceIndex(f));

        }
    }
    void AddUvs(byte index)
    {
        float y = (int)(index * VoxelData.normalizeTextureSize);
        float x = index - y * VoxelData.textureSizeInBlocks;
        y *= VoxelData.normalizeTextureSize;
        x *= VoxelData.normalizeTextureSize;
        y=1f-y-VoxelData.normalizeTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizeTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizeTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizeTextureSize, y + VoxelData.normalizeTextureSize));
    }
    private void CreateBlocks()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    CreateVoxel(new Vector3(x, y, z));
                }
            }
        }
    }
}
