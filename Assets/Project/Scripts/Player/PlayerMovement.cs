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
    
    [SyncVar]
    private bool movementEnabled = true;
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

    private void FixedUpdate()
{
    if (!isLocalPlayer || !movementEnabled) return;
    
        MovePlayer();
        UpdatePlayerFacing();
        UpdateAnimationState();
}
    private void MovePlayer()
    {
        if(!movementEnabled) return;

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
    public PlayerFacing GetCurrentFacing()
        {
            return playerFacing;
        }

    public bool IsMoving()
        {
            return moveInput != Vector2.zero;
        }

    // Methods to enable/disable movement locally
    public void EnableMovement()
    {
        if (isLocalPlayer)
        {
            CmdSetMovementEnabled(true);
        }
    }
    
    public void DisableMovement()
    {
        if (isLocalPlayer)
        {
            CmdSetMovementEnabled(false);
        }
    }
    
    // Command to set movement state on the server
    [Command]
    private void CmdSetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        RpcSyncMovementState(enabled);
    }
    
    // Notify all clients about the movement state change
    [ClientRpc]
    private void RpcSyncMovementState(bool enabled)
    {
        movementEnabled = enabled;
        
        // If movement is disabled, also stop any current movement
        if (!enabled)
        {
            moveInput = Vector2.zero;
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }
    
    // Public method to check if movement is currently enabled
    public bool IsMovementEnabled()
    {
        return movementEnabled;
    }

}
