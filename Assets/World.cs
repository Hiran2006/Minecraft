using UnityEngine;
using System;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
}

[System.Serializable]
public class BlockType
{
    // front back right left top bottom
    public string name;
    public bool isSolid;

    [Header("Texture values")]
    [SerializeField] int frontFaceTexture; 
    [SerializeField] int backFaceTexture; 
    [SerializeField] int rightFaceTexture; 
    [SerializeField] int leftFaceTexture; 
    [SerializeField] int topFaceTexture; 
    [SerializeField] int bottomFaceTexture;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return frontFaceTexture;
            case 1:
                return backFaceTexture;
            case 2:
                return rightFaceTexture;
            case 3:
                return leftFaceTexture;
            case 4:
                return topFaceTexture;
            case 5:
                return bottomFaceTexture;
            default:
                throw new Exception($"Index is Outoff range at texture face index {faceIndex}");
        }
    }
}
