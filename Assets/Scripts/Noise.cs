using UnityEngine;

public static class Noise
{
    public static float Get2DPerlin(Vector2 pos, float offset, float scale)
    {
        return Mathf.PerlinNoise((pos.x + .1f) / VoxelData.chunkWidth * scale + offset, (pos.y + .1f) / VoxelData.chunkWidth * scale + offset);
    }

    public static bool Get3DPerlinBool(Vector3 pos, float offset, float scale, float threshold)
    {
        float xy = Mathf.PerlinNoise((pos.x + .1f) / VoxelData.chunkWidth * scale + offset, (pos.y + .1f) / VoxelData.chunkWidth * scale + offset);
        float yz = Mathf.PerlinNoise((pos.y + .1f) / VoxelData.chunkWidth * scale + offset, (pos.z + .1f) / VoxelData.chunkWidth * scale + offset);
        float xz = Mathf.PerlinNoise((pos.x + .1f) / VoxelData.chunkWidth * scale + offset, (pos.z + .1f) / VoxelData.chunkWidth * scale + offset);
        return (xy + yz + xz) / 3f > threshold;
    }
}
