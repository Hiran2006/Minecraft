using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    GameObject chunkObject;
    World world;
    MeshRenderer renderer;
    MeshFilter filter;

    int vertexIndex = 0;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public ushort[,,] voxelMap = new ushort[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    bool _isActive;
    public bool isVoxelMapPopulated = false;
    public Chunk(ChunkCoord coord,World world,bool generateOnLoad)
    {
        this.coord = coord;
        this.world = world;

        if (generateOnLoad)
            Init();
    }

    public void Init()
    {
        chunkObject = new GameObject($"Chunk {coord.x}, {coord.z}");
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x, 0f, coord.z) * VoxelData.ChunkWidth;

        filter = chunkObject.AddComponent<MeshFilter>();
        renderer = chunkObject.AddComponent<MeshRenderer>();

        renderer.material = world.material;

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
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
        isVoxelMapPopulated = true;
    }

    void UpdateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (world.blockTypes[voxelMap[x, y, z]].isSolid)
                        AddVoxelDataToChunk(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public bool isActive
    {
        get { return _isActive; }
        set {
            _isActive = value;
            if(chunkObject!=null)
                chunkObject.SetActive(value);
        }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }

    bool IsVoxelInChunk(int x,int y,int z)
    {
        return !(x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1);
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.CheckForVoxel(pos + position);

        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    public ushort GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);


        return voxelMap[xCheck, yCheck, zCheck];
    } 

    void AddVoxelDataToChunk(Vector3Int pos)
    {
        for(int f = 0; f < 6; f++)
        {
            if (!CheckVoxel(pos + VoxelData.faceCheck[f]))
            {
                ushort blockID = voxelMap[pos.x, pos.y, pos.z];
                for (int p = 0; p < 4; p++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[f, p]]);
                }

                AddTexture(world.blockTypes[blockID].GetTextureID(f));

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
        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);
        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1 - y - VoxelData.NormalizedBlockTextureSize;

        for(int i = 0; i < 4; i++)
        {
            uvs.Add(new Vector2(x, y) + VoxelData.voxelUvs[i] * VoxelData.NormalizedBlockTextureSize);
        }
    }
}

public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public ChunkCoord(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);

        x /= VoxelData.ChunkWidth;
        z /= VoxelData.ChunkWidth;

        this.x = x;
        this.z = z;
    }

    public static bool operator ==(ChunkCoord l,ChunkCoord r)
    {
        return l.x == r.x && l.z == r.z;
    }
    public static bool operator !=(ChunkCoord l, ChunkCoord r)
    {
        return !(l == r);
    }
    public override bool Equals(object l)
    {
        return true;
    }
    public override int GetHashCode()
    {
        return 0;
    }
    public static bool GetHashCode(ChunkCoord l, ChunkCoord r)
    {
        return l == r;
    }
}
