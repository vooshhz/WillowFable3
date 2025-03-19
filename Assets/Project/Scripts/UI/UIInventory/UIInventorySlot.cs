using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;
using System.Collections;

public class UIInventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GameObject draggedItem;
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    private Transform playerTransform;

      // Track if this slot is currently selected
    private bool isSelected = false;
    
    // Reference to the currently selected slot (static so only one slot can be selected at a time)
    private GameObject selectedItemVisual;

    private static UIInventorySlot currentlySelectedSlot = null;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private int slotNumber = 0;

    private void Awake() 
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    private void Start() 
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        StartCoroutine(FindLocalPlayer());
    }

    private void Update() 
    {
        // If this slot is selected and player clicks on the game world (not UI)
        if (isSelected && Input.GetMouseButtonDown(0))
        {
            // Check if the click was on a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                DropSelectedItemAtPlayerPosition();
                UnselectItem();
            }
        }
        
        // Move the selected item visual to follow the mouse
        if (isSelected && selectedItemVisual != null)
        {
            selectedItemVisual.transform.position = Input.mousePosition;
        }
    }

     public void OnPointerClick(PointerEventData eventData)
    {
        // If there's already a selected slot and it's not this one, perform swap
        if (currentlySelectedSlot != null && currentlySelectedSlot != this)
        {
            // Get the slot numbers
            int fromSlot = currentlySelectedSlot.slotNumber;
            int toSlot = this.slotNumber;
            
            // Perform the swap in InventoryManager
            InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, fromSlot, toSlot);
            
            //Destroy inventory text box
            DestroyInventoryTextBox();

            // Unselect the current slot
            currentlySelectedSlot.UnselectItem();
            
            return;
        }
        // Toggle selected state
        if (!isSelected)
        {
            SelectItem();
        }
        else
        {
            UnselectItem();
        }
    }

    private void SelectItem()
    {
        if (itemDetails == null) return;

        // Set as currently selected
        isSelected = true;
        currentlySelectedSlot = this;
        
        // Highlight the slot
        inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);
        
        // Create visual representation
        selectedItemVisual = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);
        
        // Set the visual's image
        Image selectedItemImage = selectedItemVisual.GetComponentInChildren<Image>();
        selectedItemImage.sprite = inventorySlotImage.sprite;
        
        // Set initial position to mouse
        selectedItemVisual.transform.position = Input.mousePosition;
    }

    private void UnselectItem()
    {
        isSelected = false;
        
        if (currentlySelectedSlot == this)
        {
            currentlySelectedSlot = null;
        }
        
        // Remove highlight
        inventorySlotHighlight.color = new Color(1f, 1f, 1f, 0f);
        
        // Destroy the visual
        if (selectedItemVisual != null)
        {
            Destroy(selectedItemVisual);
            selectedItemVisual = null;
        }
    }

    private IEnumerator FindLocalPlayer()
    {
        while (playerTransform == null)
            {
                if (NetworkClient.localPlayer != null)
                {
                    playerTransform = NetworkClient.localPlayer.transform;
                    Debug.Log("Local player transform found: " + playerTransform.position);
                }
                yield return new WaitForSeconds(0.5f);
            }
    }

    private void DropSelectedItemAtPlayerPosition()
    {
        if (itemDetails == null || !itemDetails.canBeDropped) return;
        
        // Check if player transform exists
        if (playerTransform == null)
        {
            Debug.LogError("Player transform reference is missing!");
            return;
        }
        
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned!");
            return;
        }
        
        // Get player position with offset
        Vector3 dropPosition = playerTransform.position + new Vector3(0, 2, 0);
        
        // Create the item at player position
        GameObject newItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity, parentItem);
        
        // Set up the item
        Item itemComponent = newItem.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.Init(itemDetails.itemCode);
            
            // Use InventoryManager to remove item from inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemDetails.itemCode);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemQuantity != 0)
        {
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();
                
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }

            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }
}
