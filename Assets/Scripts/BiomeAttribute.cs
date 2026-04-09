using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttribute", menuName = "Minecraft/BiomeAttribute", order = 1)]
public class BiomeAttribute : ScriptableObject
{
    public string biomeName;

    public int solidGroundHeight;
    public int terrianHeight;
    public float terrianScale;

}