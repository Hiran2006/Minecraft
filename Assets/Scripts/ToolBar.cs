using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class ToolBar : MonoBehaviour
{
    public ItemSlot[] slots;
    World world;
    Player player;
    public RectTransform highlighter;
    public RectTransform toolBarContainer;

    int slotIndex = 0;

    public int selectedBlockID
    {
        get
        {
            if (slots[slotIndex].blockID != 0)
                return slots[slotIndex].blockID;
            else
                return -1;
        }
    }

    private void Start()
    {
        world = GameObject.FindAnyObjectByType<World>().GetComponent<World>();
        player = GetComponent<Player>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].blockID != 0)
            {
                var temp = toolBarContainer.GetChild(i).GetChild(0).GetComponent<Image>();
                temp.sprite = world.blockTypes[slots[i].blockID].icon;
                temp.enabled = true;
            }
        }

    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                slotIndex++;
                if (slotIndex >= slots.Length) slotIndex = 0;
            }
            else
            {
                slotIndex--;
                if (slotIndex < 0) slotIndex = slots.Length - 1;
            }
            highlighter.localPosition = new Vector2((slotIndex - slots.Length / 2) * 120, 0);

        }
    }
}

[System.Serializable]
public struct ItemSlot
{
    public byte blockID;
}
