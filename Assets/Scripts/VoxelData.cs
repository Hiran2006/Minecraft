    using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeInChunks = 10;
    public static int WorldSizeInBlocks
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static int ViewDistanceInChunks = 3;

    public static readonly int textureAtlasSizeInBlocks = 33;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    } 

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0,0,0),// 0
        new Vector3(1,0,0),// 1
        new Vector3(1,1,0),// 2
        new Vector3(0,1,0),// 3
        new Vector3(0,0,1),// 4
        new Vector3(1,0,1),// 5
        new Vector3(1,1,1),// 6
        new Vector3(0,1,1),// 7
    };

    public static readonly int[,] voxelTris = new int[6, 4]
    {
        { 5,6,4,7},// Front
        { 0,3,1,2},// Back
        { 1,2,5,6},// Right
        { 4,7,0,3},// Left
        { 3,7,2,6},// Top
        { 4,0,5,1}// Bottom
    };

    public static readonly Vector3[] faceCheck = new Vector3[6]
    {
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
    };

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1)
    };
}
