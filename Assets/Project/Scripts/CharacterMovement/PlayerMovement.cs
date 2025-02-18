using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private PlayerInput playerInput;

    private CharacterAnimator characterAnimator;

    // Tracks the direction the player is facing
    private PlayerFacing playerFacing = PlayerFacing.Down;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<CharacterAnimator>(); // Get Animator Reference

        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        MovePlayer();
        UpdatePlayerFacing();
        UpdateAnimationState();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = new Vector3(moveInput.x, moveInput.y, 0);
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    // Determines which direction the player is facing based on the movement input
    private void UpdatePlayerFacing()
    {
        if (moveInput.y > 0)
        {
            playerFacing = PlayerFacing.Up;
        }
        else if (moveInput.y < 0)
        {
            playerFacing = PlayerFacing.Down;
        }
        else if (moveInput.x < 0)
        {
            playerFacing = PlayerFacing.Left;
        }
        else if (moveInput.x > 0)
        {
            playerFacing = PlayerFacing.Right;
        }
        // If moveInput is (0,0), do nothing. Facing stays the same.
    }

    private void UpdateAnimationState()
    {
        if (moveInput != Vector2.zero)
        {
            characterAnimator.PlayRun(playerFacing);
        }
        else
        {
            characterAnimator.PlayIdle(playerFacing);
        }
    }
}
