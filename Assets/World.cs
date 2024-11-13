using UnityEngine;

public class World : MonoBehaviour
{
    public Material mat;
    private void Start()
    {
        new Chunk(this);
    }
}
