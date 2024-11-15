using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
static class VoxelData
{
    public static int renderDistance = 5;
    public static int worldSize = 100;
    public static int chunkWidth = 16;
    public static int chunkHeight = 126;

    public static int textureSize = 3;
    public readonly static float blockTextureSize = 1f / textureSize;

    public static Vector3[] voxelVertices = new Vector3[]
    {
        new Vector3(0,0,0),//0
        new Vector3(1,0,0),//1
        new Vector3(0,1,0),//2
        new Vector3(1,1,0),//3
        new Vector3(0,0,1),//4
        new Vector3(0,1,1),//5
        new Vector3(1,0,1),//6
        new Vector3(1,1,1),//7
    };
    public static int[,] voxelTriangles = new int[,]
    {
        {6,7,4,5 },//front
        {0,2,1,3 },//back
        {1,3,6,7 },//right
        {4,5,0,2 },//left
        {2,5,3,7 },//top
        {4,0,6,1 }//bottom
    };
    public static Vector3Int[] faceCheck = new Vector3Int[]
    {
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0),
    };
}
