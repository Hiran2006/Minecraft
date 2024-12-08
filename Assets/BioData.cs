using UnityEngine;

[CreateAssetMenu(fileName = "BiomData", menuName = "Scriptable Objects/BiomData")]
public class BiomData : ScriptableObject
{
    public int minHeight;
    public int maxHeight;
    [Range(0f, 1f)]
    public float caveTreshold;
}
