using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttribute", menuName = "Minecraft/BiomeAttribute", order = 1)]
public class BiomeAttribute : ScriptableObject
{
    public string biomeName;

    public int minHeight;
    public int terrianHeight;
    public double terrianScale;

    public byte surfaceBlock;
    public byte subSurfaceBlock;
    public byte innerBlock;

    public double treeDesity;

    public float temperature;
    public float humidity;

}