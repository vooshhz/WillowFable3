using Mirror;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class CharacterEquipmentHandler : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int headItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int bodyItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int hairItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int torsoItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int legsItem;

    [SerializeField] private CharacterAnimator characterAnimator;
    private string userId;
    private string characterId;
    private DatabaseReference dbRef;

    void Start()
    {
        EventManager.Instance.Subscribe(EventType.PlayerInstantiated, ApplyCharacterEquipment);
    }
    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe(EventType.PlayerInstantiated, ApplyCharacterEquipment);
        }
    }
        public void ApplyCharacterEquipment()
    {
        if (characterAnimator == null) return;

        characterAnimator.headItemNumber = headItem;
        characterAnimator.bodyItemNumber = bodyItem;
        characterAnimator.hairItemNumber = hairItem;
        characterAnimator.torsoItemNumber = torsoItem;
        characterAnimator.legsItemNumber = legsItem;
        characterAnimator.RefreshCurrentFrame();

        EventManager.Instance.TriggerEvent(EventType.PlayerEquipmentApplied);
    }

    private void OnEquipmentChanged(int oldValue, int newValue)
    {
        ApplyCharacterEquipment();
    }

    [Command]
    public void CmdChangeEquipment(int newHead, int newBody, int newHair, int newTorso, int newLegs)
    {
        if (!isServer) return;

        headItem = newHead;
        bodyItem = newBody;
        hairItem = newHair;
        torsoItem = newTorso;
        legsItem = newLegs;

        SaveEquipmentToFirebase();
    }

    private void SaveEquipmentToFirebase()
    {
        if (dbRef == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(characterId)) return;

        var updates = new System.Collections.Generic.Dictionary<string, object>
        {
            [$"users/{userId}/characters/{characterId}/equipment/head"] = headItem,
            [$"users/{userId}/characters/{characterId}/equipment/body"] = bodyItem,
            [$"users/{userId}/characters/{characterId}/equipment/hair"] = hairItem,
            [$"users/{userId}/characters/{characterId}/equipment/torso"] = torsoItem,
            [$"users/{userId}/characters/{characterId}/equipment/legs"] = legsItem
        };

        dbRef.UpdateChildrenAsync(updates);
    }

    public void SetFirebaseRefs(DatabaseReference refDb, string uid, string cid)
    {
        dbRef = refDb;
        userId = uid;
        characterId = cid;
    }
}