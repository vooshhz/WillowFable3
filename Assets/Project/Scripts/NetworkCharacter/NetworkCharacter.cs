using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkCharacter : NetworkBehaviour
{
    [SerializeField] private CharacterEquipmentHandler equipmentHandler;
    [SerializeField] private CharacterStateManager stateManager;
    [SerializeField] private FirebaseCharacterSync firebaseSync;
    [SerializeField] private InitializeUI initializeUI;
    [SerializeField] private SceneLoader sceneLoader;
    public CharacterState CurrentState => stateManager.currentState;
    public PlayerFacing CurrentDirection => stateManager.currentDirection;
    public override void OnStartClient()
    {
        base.OnStartClient();
        equipmentHandler.ApplyCharacterEquipment();
    }
    void Start()
    {
        Debug.Log("[NetworkCharacter] Start() called.");

    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        firebaseSync.InitializeFirebase();
        Debug.Log("[NetworkCharacter] OnStartServer called.");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        StartCoroutine(WaitForNetworkReady());
        Debug.Log("[NetworkCharacter] OnStartLocalPlayer called.");
    }

    private IEnumerator WaitForNetworkReady()
    {
    Debug.Log("[NetworkCharacter] Coroutine started. Waiting for network ready...");
    yield return new WaitUntil(() =>
        CustomNetworkManager.IsNetworkManagerReady &&
        isLocalPlayer &&
        isOwned);

    Debug.Log("[NetworkCharacter] Network ready! Calling SetupUserData.");
    firebaseSync.SetupUserData();  // <-- CALL CmdSetUserData *inside here*, not earlier
    }

    [Command]
    public void CmdChangeEquipment(int newHead, int newBody, int newHair, int newTorso, int newLegs)
    {
        equipmentHandler.CmdChangeEquipment(newHead, newBody, newHair, newTorso, newLegs);
    }

    [Command]
    public void CmdSaveLocation(string sceneName)
    {
        firebaseSync.CmdSaveLocation(sceneName, transform.position);
    }

    [Command]
    public void CmdUpdateState(CharacterState newState, PlayerFacing newDirection)
    {
        stateManager.CmdUpdateState(newState, newDirection);
    }
}
