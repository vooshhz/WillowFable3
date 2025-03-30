using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.SceneManagement;
using Mirror;


public class InitializeUI : MonoBehaviour
{
    [SerializeField] public GameObject inventoryBar;

    void Start()
    {
        EventManager.Instance.Subscribe(EventType.PlayerEquipmentApplied, InitializeUIComponents);
    }

    void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EventType.PlayerEquipmentApplied, InitializeUIComponents);
    }

    public void SetCameraFollow(Transform target)
    {
                if (target == null)
        {
            Debug.LogError("SetCameraFollow called with null target.");
            return;
        }

        StartCoroutine(WaitAndSetFollow(target));

    }

    private IEnumerator WaitAndSetFollow(Transform target)
    {
        while (!SceneManager.GetSceneByName("PlayerUIScene").isLoaded)
            yield return null;

        CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
        if (cam != null)
        {
            cam.Follow = target;
        }
    }
    private void InitializeUIComponents()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InitializeInventoryBar();
            LoadInventoryData();
            SetCameraFollow(NetworkClient.localPlayer.transform);
            EventManager.Instance.TriggerEvent(EventType.PlayerUIConnected);
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
