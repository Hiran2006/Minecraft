using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
    void Start()
    {
        new Chunk(this);
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
    [Header("Texture Indices")]
    public byte frontFace;
    public byte backFace;
    public byte rightFace;
    public byte leftFace;
    public byte topFace;
    public byte bottomFace;

    public byte GetFaceIndex(int face)
    {
        switch (face)
        {
            case 0:
                return frontFace;
            case 1:
                return backFace;
            case 2:
                return rightFace;
            case 3:
                return leftFace;
            case 4:
                return topFace;
            case 5:
                return bottomFace;
            default:
                Debug.LogError("Invalid face index: " + face);
                return 0;
        }
    }
}