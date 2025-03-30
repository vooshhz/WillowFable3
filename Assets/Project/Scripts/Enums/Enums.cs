public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum CharacterState
{
    Idle,
    Running,
    Attacking,
    Casting
}

public enum PlayerFacing
{
    Up,
    Down,
    Left,
    Right
}

public enum ItemType
{
    Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum SceneName
{
    Scene_Intro_Scene,
    Scene_Field_Scene
}

public enum EventType
{
    // Login and authentication events
    LoginSuccessful,
    LoginFailed,
    CharacterDataLoaded,
    CharacterSelected,
    
    // Scene loading events
    PersistentSceneLoaded,
    PlayerSceneLoaded,
    PlayerUISceneLoaded,
    AllScenesLoaded,
    
    // Game system initialization events
    CustomNetworkPlayerPrefabSpawnned,
    NetworkManagerReady,
    FirebaseCharacterSynced,
    InventoryInitialized,
    PlayerInstantiated,
    PlayerUIConnected,
    
    // Player-related events
    PlayerPositionUpdated,
    PlayerEquipmentApplied,
    InventoryUpdated,
    
    // Scene transition events
    BeginSceneTransition,
    SceneTransitionComplete
}