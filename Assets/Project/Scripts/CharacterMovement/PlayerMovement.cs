using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private PlayerInput playerInput;
    private CharacterAnimator characterAnimator;
    private NetworkCharacter networkCharacter; // Reference to the network state handler

    private PlayerFacing playerFacing = PlayerFacing.Down;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponentInChildren<CharacterAnimator>(); // Ensures it's found in child objects
        networkCharacter = GetComponent<NetworkCharacter>(); // Ensure reference to network state

        if (characterAnimator == null)
        {
            Debug.LogError("CharacterAnimator component is missing on Player or its child objects!");
        }

        // Input System setup
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (!isLocalPlayer) return; // Prevent controlling other players

        MovePlayer();
        UpdatePlayerFacing();
        UpdateAnimationState();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = new Vector3(moveInput.x, moveInput.y, 0);
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void UpdatePlayerFacing()
    {
        if (moveInput.y > 0) playerFacing = PlayerFacing.Up;
        else if (moveInput.y < 0) playerFacing = PlayerFacing.Down;
        else if (moveInput.x < 0) playerFacing = PlayerFacing.Left;
        else if (moveInput.x > 0) playerFacing = PlayerFacing.Right;
    }

    private void UpdateAnimationState()
    {
        if (characterAnimator == null || networkCharacter == null) return; // Prevent errors

        CharacterState newState = (moveInput != Vector2.zero) ? CharacterState.Running : CharacterState.Idle;

        // Send state update to the network if it has changed
        if (newState != networkCharacter.currentState || playerFacing != networkCharacter.currentDirection)
        {
            networkCharacter.CmdUpdateState(newState, playerFacing);
        }
    }
}
