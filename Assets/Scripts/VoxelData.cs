
using UnityEngine;

public static class VoxelData
{
    public readonly static int chunkWidth = 5;
    public readonly static int chunkHeight = 5;

    public readonly static int worldSizeInChunks = 10;

    public readonly static int textureSizeInBlocks = 4;
    public static float normalizeTextureSize
    {
        get { return 1f / textureSizeInBlocks; }
    }

    public readonly static Vector3[] voxelVerts = new Vector3[]
    {
        new(0,0,0),//0
        new(1,0,0),//1
        new(0,1,0),//2
        new(1,1,0),//3
        new(0,0,1),//4
        new(1,0,1),//5
        new(0,1,1),//6
        new(1,1,1),//7
    };
    public readonly static int[,] voxelTris = new int[,]
    {
        {5,7,4,6},//Front
        {0,2,1,3},//Back
        {1,3,5,7},//Right
        {4,6,0,2 },//Left
        {2,6,3,7},//Top
        {4,0,5,1},//Bottom
    };
    public readonly static Vector3[] faceChecks = new Vector3[]
    {
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
    };
}