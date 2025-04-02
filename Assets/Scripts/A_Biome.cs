using UnityEngine;

[System.Serializable]
public abstract class A_Biome
{
    public string Name;

    public abstract void GetBlock(Vector3 pos);
}