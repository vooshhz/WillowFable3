using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField] private int _itemCode;

    private SpriteRenderer spriteRenderer;

    public int ItemCode { get { return _itemCode; } set {_itemCode = value;}}

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(ItemCode !=0)
        {
            Init(ItemCode);
        }        
    }

    public void Init(int itemCodeParam)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            // If item type is reapable then add nudgeable component
            if(itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }

            // If item type is reapable then add nudgeable component
            if(itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }

              // Add floating component to specific item types
            if (itemDetails.itemType == ItemType.Seed ||
                itemDetails.itemType == ItemType.Commodity ||
                itemDetails.itemType == ItemType.Watering_tool ||
                itemDetails.itemType == ItemType.Hoeing_tool ||
                itemDetails.itemType == ItemType.Chopping_tool ||
                itemDetails.itemType == ItemType.Breaking_tool ||
                itemDetails.itemType == ItemType.Reaping_tool ||
                itemDetails.itemType == ItemType.Collecting_tool)
            {
                gameObject.AddComponent<ItemFloat>();
            }
        }
    }

    public void InitAfterDrop (int itemCodeParam)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            // If item type is reapable then add nudgeable component
            if(itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }

            // If item type is reapable then add nudgeable component
            if(itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }

}