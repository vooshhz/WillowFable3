using System;
using UnityEngine;

public static class EventHandler
{
    //  Player Movement Event
    public static event Action<Vector2> OnPlayerMoved;
    public static void CallPlayerMoved(Vector2 movementInput)
    {
        OnPlayerMoved?.Invoke(movementInput);
    }

    //  Character Animation Event
    public static event Action<CharacterState, PlayerFacing> OnAnimationStateChanged;
    public static void CallAnimationStateChanged(CharacterState state, PlayerFacing direction)
    {
        OnAnimationStateChanged?.Invoke(state, direction);
    }
}
