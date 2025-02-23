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
    private Rigidbody2D rb;

    private PlayerFacing playerFacing = PlayerFacing.Down;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
        characterAnimator = GetComponentInChildren<CharacterAnimator>(); // Ensure it's found in child objects
        networkCharacter = GetComponent<NetworkCharacter>(); // Ensure reference to network state

        if (characterAnimator == null)
        {
            Debug.LogError("CharacterAnimator component is missing on Player or its child objects!");
        }

        // Input System setup
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
    }

    private void FixedUpdate() // Use FixedUpdate for physics-based movement
    {
        if (!isLocalPlayer) return; // Prevent controlling other players

        MovePlayer();
        UpdatePlayerFacing();
        UpdateAnimationState();
    }

    private void MovePlayer()
    {
        Vector2 moveVector = moveInput * moveSpeed * Time.fixedDeltaTime;

        // Apply movement using Rigidbody2D for proper collision handling
        rb.MovePosition(rb.position + moveVector);
    }

    private void UpdatePlayerFacing()
    {
        if (moveInput == Vector2.zero) return; // No movement, keep current facing direction

        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        if (absX > absY) // Prioritize Left/Right
        {
            playerFacing = (moveInput.x > 0) ? PlayerFacing.Right : PlayerFacing.Left;
        }
        else if (absY > absX) // Prioritize Up/Down
        {
            playerFacing = (moveInput.y > 0) ? PlayerFacing.Up : PlayerFacing.Down;
        }
        else // Handle exact diagonal case (tie)
        {
            playerFacing = (moveInput.x > 0) ? PlayerFacing.Right : PlayerFacing.Left;
        }
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
