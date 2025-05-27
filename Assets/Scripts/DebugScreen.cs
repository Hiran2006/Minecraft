using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading;

public class DebugScreen : MonoBehaviour
{
    World world;
    TMP_Text text;


    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        string debugText= "Hiran, Minecraft";
        debugText += '\n';
        debugText += $"FPS {(int)(1 / Time.unscaledDeltaTime)}";
        debugText += '\n';
        debugText += $"Block : {Mathf.FloorToInt(world.player.position.x)}, {Mathf.FloorToInt(world.player.position.y)}, {Mathf.FloorToInt(world.player.position.z)}";
        debugText += '\n';
        debugText += $"Chunk : {world.playerCurrentChunkCoord.x}, {world.playerCurrentChunkCoord.z}";
        text.text = debugText;
    }
}
