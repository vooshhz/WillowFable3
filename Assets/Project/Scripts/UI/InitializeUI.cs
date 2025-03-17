using UnityEngine;

public class InitializeUI : MonoBehaviour
{
    [SerializeField] public GameObject inventoryBar;

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InitializeInventoryBar();
        }
        else
        {
            Debug.LogError("InventoryManager instance not found.");
        }
    }
}
