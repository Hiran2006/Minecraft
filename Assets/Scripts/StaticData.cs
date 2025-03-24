using UnityEngine;

public static class VoxelData
{
    public static readonly int textureSize = 16;

    public static float blockTextureSize
    {
        get { return 1f / textureSize; }
    }

    public static readonly Vector3[] vertices= new Vector3[]
    {
        new Vector3(0,0,0),//0
        new Vector3(1,0,0),//1
        new Vector3(0,1,0),//2
        new Vector3(1,1,0),//3
        new Vector3(0,0,1),//4
        new Vector3(1,0,1),//5
        new Vector3(0,1,1),//6
        new Vector3(1,1,1)//7
    };

    public static readonly int[,] triangles = new int[6, 4]
    {
        {5,7,4, 6},//Front
        {0, 2,1,3},//Back
        {1,3,5,7 },//Right
        {4,6,0,2 },//Left
        {2,6,3,7 },//Top
        {4,0,5,1 },//Bottom
    };

    public static readonly Vector3[] lookAt = new Vector3[6]
    {
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
    };

    public static readonly Vector2[] uvs = new Vector2[] {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1),
    };
}

public static class WorldDate
{
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 100;

    public static readonly int worldWidth = 100;
    public static readonly int renderDis = 3;
}
