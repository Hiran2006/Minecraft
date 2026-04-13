using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
    Mesh mesh;
    World world;
    GameObject chunkObj;
    public ChunkCoord coord;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] blocks = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    int vertIndex = 0;
    public Chunk(ChunkCoord coord, World world)
    {
        this.world = world;
        this.coord = coord;

        chunkObj = new GameObject($"Chunk {coord.x}, {coord.z}");
        chunkObj.transform.SetParent(world.transform);
        chunkObj.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);


        InitBlocks();

    }

    public void Init()
    {
        mesh = new Mesh();
        chunkObj.AddComponent<MeshFilter>().mesh = mesh;
        chunkObj.AddComponent<MeshRenderer>().material = world.material;
        UpdateChunk();


    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void ClearMeshData()
    {
        vertIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }


    public bool isActive
    {
        get { return chunkObj.activeSelf; }
        set { chunkObj.SetActive(value); }
    }

    void InitBlocks()
    {

        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    blocks[x, y, z] = world.GetBlockMap(chunkPos + new Vector3(x, y, z));
                }
            }
        }
    }

    Vector3 chunkPos
    {
        get { return new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth); }
    }

    private void CreateVoxel(Vector3 pos)
    {
        for (int f = 0; f < 6; f++)
        {
            if (!IsVisible(pos + VoxelData.faceChecks[f]))
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
    }
    void AddUvs(byte index)
    {
        float y = (int)(index * VoxelData.normalizeTextureSize);
        float x = index - y * VoxelData.textureSizeInBlocks;
        y *= VoxelData.normalizeTextureSize;
        x *= VoxelData.normalizeTextureSize;
        y = 1f - y - VoxelData.normalizeTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizeTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizeTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizeTextureSize, y + VoxelData.normalizeTextureSize));
    }
    public void UpdateChunk()
    {
        ClearMeshData();
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (IsVisible(pos))
                        CreateVoxel(pos);
                }
            }
        }
        UpdateMesh();
    }
    private bool IsVisible(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;
        if (x >= 0 && x < VoxelData.chunkWidth && y >= 0 && y < VoxelData.chunkHeight && z >= 0 && z < VoxelData.chunkWidth)
        {
            return world.blockTypes[blocks[x, y, z]].isSolid;
        }
        return world.CheckBlock(chunkPos + new Vector3(x, y, z));
    }
}


public class ChunkCoord
{
    public int x;
    public int z;
    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public static bool operator ==(ChunkCoord a, ChunkCoord b)
    {
        return (a.x == b.x && a.z == b.z);
    }

    public static bool operator !=(ChunkCoord a, ChunkCoord b)
    {
        return !(a == b);
    }
}