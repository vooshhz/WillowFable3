using Mirror;
using UnityEngine;

public class CharacterStateManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnStateChanged))] public CharacterState currentState = CharacterState.Idle;
    [SyncVar(hook = nameof(OnDirectionChanged))] public PlayerFacing currentDirection = PlayerFacing.Down;

    [SerializeField] private CharacterAnimator characterAnimator;

    private void OnStateChanged(CharacterState oldState, CharacterState newState)
    {
        ApplyCharacterState(newState, currentDirection);
    }

    private void OnDirectionChanged(PlayerFacing oldDirection, PlayerFacing newDirection)
    {
        ApplyCharacterState(currentState, newDirection);
    }

    public void ApplyCharacterState(CharacterState state, PlayerFacing direction)
    {
        if (characterAnimator == null) return;

        switch (state)
        {
            case CharacterState.Idle:
                characterAnimator.PlayIdle(direction);
                break;
            case CharacterState.Running:
                characterAnimator.PlayRun(direction);
                break;
        }
    }

    [Command]
    public void CmdUpdateState(CharacterState newState, PlayerFacing newDirection)
    {
        if (!isServer) return;

        currentState = newState;
        currentDirection = newDirection;
    }
}