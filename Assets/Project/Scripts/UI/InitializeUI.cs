using Firebase.Firestore;
using UnityEngine;

public class InitializeUI : MonoBehaviour
{
    [SerializeField] public GameObject inventoryBar;

    void Start()
    {
        InitializeUIComponents();

        LoadInventoryData();
    }

    private void InitializeUIComponents()
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

    private void LoadInventoryData()
    {
        if (InventoryManager.Instance != null)
        {
            // Load inventory data from Firebase
            InventoryManager.Instance.LoadInventoryFromFirebase();
            Debug.Log("Loading inventory data from Firebase requested");
        }
    }
}
