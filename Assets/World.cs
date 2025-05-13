using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
}

[System.Serializable]
public class BlockType
{
    public string name;
    public bool isSolid;
}
