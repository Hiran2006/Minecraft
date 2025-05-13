using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 5;
    public static readonly int ChunkHeight = 5;

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

    public static readonly int[,] voxelTris = new int[6, 6]
    {
        { 5,6,4,4,6,7},// Front
        { 0,3,1,1,3,2},// Back
        { 1,2,5,5,2,6},// Right
        { 4,7,0,0,7,3},// Left
        { 3,7,2,2,7,6},// Top
        { 4,0,5,5,0,1}// Bottom
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
}
