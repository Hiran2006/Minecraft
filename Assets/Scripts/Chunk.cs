using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    World world;
    GameObject obj;
    ChunkCoordinate coord;

    Mesh mesh;

    byte[,,] blockData = new byte[WorldDate.chunkWidth, WorldDate.chunkHeight, WorldDate.chunkWidth];

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    int triIndex = 0;
    void InitialiseBlock()
    {
        for (int x = 0; x < WorldDate.chunkWidth; x++)
        {
            for (int z = 0; z < WorldDate.chunkWidth; z++)
            {
                for (int y = 0; y < WorldDate.chunkHeight; y++)
                {
                    blockData[x, y, z] = world.GetVoxelData(GetWorldPos(x, y, z));
                }
            }
        }
    }

    public Vector3 GetWorldPos(int x,int y, int z)
    {
        return new Vector3(coord.x, 0, coord.z) * WorldDate.chunkWidth + new Vector3(x, y, z);
    }

    public Chunk(World world, ChunkCoordinate coord)
    {
        this.world = world;
        this.coord = coord;

        obj = new GameObject($"Chunk {coord.x}, {coord.z}");
        obj.transform.position = new Vector3(coord.x, 0, coord.z) * WorldDate.chunkWidth;
        obj.transform.parent = world.gameObject.transform;

        mesh = new Mesh();
        obj.AddComponent<MeshRenderer>().material = world.mat;
        obj.AddComponent<MeshFilter>().mesh = mesh;

        InitialiseBlock();
        GenerateChunk();
    }

    private void GenerateChunk()
    {
        for (int x = 0; x < WorldDate.chunkWidth; x++)
        {
            for (int z = 0; z < WorldDate.chunkWidth; z++)
            {
                for (int y = 0; y < WorldDate.chunkHeight; y++)
                {
                    GenerateVoxel(new Vector3Int(x, y, z));
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void GenerateVoxel(Vector3Int pos)
    {
        for (int f = 0; f < 6; f++)
        {
            if (IsFaceToRender(pos, VoxelData.lookAt[f]))
            {
                for (int p = 0; p < 4; p++)
                {
                    vertices.Add(pos + VoxelData.vertices[VoxelData.triangles[f, p]]);
                }
                triangles.Add(triIndex);
                triangles.Add(triIndex + 1);
                triangles.Add(triIndex + 2);
                triangles.Add(triIndex + 2);
                triangles.Add(triIndex + 1);
                triangles.Add(triIndex + 3);
                triIndex += 4;
                AddTexture(world.blockType[blockData[pos.x, pos.y, pos.z]].GetTextureIndex(f));
            }
        }
    }

    void AddTexture(int textureID){
        float yOffset = (int)(textureID * VoxelData.blockTextureSize);
        float xOffset = textureID - yOffset * VoxelData.textureSize;
        yOffset *= VoxelData.blockTextureSize;
        xOffset *= VoxelData.blockTextureSize;
        yOffset = 1 - yOffset - VoxelData.blockTextureSize;
        for(int i = 0;i< 4; i++)
        {
            uvs.Add(new Vector2(xOffset, yOffset) + VoxelData.uvs[i] * VoxelData.blockTextureSize);
        }
    }

    bool IsFaceToRender(Vector3 pos,Vector3 direction)
    {
        Vector3 lookPos = pos + direction;
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;
        int lX = (int)lookPos.x;
        int lY = (int)lookPos.y;
        int lZ = (int)lookPos.z;
        if (blockData[x, y, z] != 0)
        {
            if (lX >= 0 && lX < WorldDate.chunkWidth && lY >= 0 && lY < WorldDate.chunkHeight && lZ >= 0 && lZ < WorldDate.chunkWidth)
            {
                if (blockData[lX, lY, lZ] == 0)
                {
                    return true;
                }
            }
            else
            {
                byte voxel = world.GetVoxelData(GetWorldPos(lX, lY, lZ));
                if (voxel == 0)
                {
                    return true;
                }
            }
        }
        return false;
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