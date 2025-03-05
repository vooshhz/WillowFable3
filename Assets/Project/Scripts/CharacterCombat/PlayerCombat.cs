using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : NetworkBehaviour
{
    public float attackCooldown = 1f;
    public float spellCastCooldown = 2f;
    
    private bool isAttacking = false;
    private bool isCasting = false;
    private float attackCooldownTimer = 0f;
    private float spellCastCooldownTimer = 0f;
    
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private CharacterAnimator characterAnimator;
    private NetworkCharacter networkCharacter;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        characterAnimator = GetComponentInChildren<CharacterAnimator>();
        networkCharacter = GetComponent<NetworkCharacter>();
        
        playerInput.actions["MeleeAttack"].performed += ctx => TryAttack();
        playerInput.actions["SpellCast"].performed += ctx => TrySpellCast();
    }
    
    private void Update()
    {
        if (!isLocalPlayer) return;
        
        ManageCooldowns();
    }
    
    private void ManageCooldowns()
    {
        if (isAttacking)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                isAttacking = false;
                UpdateNetworkState();
            }
        }
        
        if (isCasting)
        {
            spellCastCooldownTimer -= Time.deltaTime;
            if (spellCastCooldownTimer <= 0)
            {
                isCasting = false;
                UpdateNetworkState();
            }
        }
    }
    
    private void TryAttack()
    {
        if (!isLocalPlayer || isAttacking || isCasting) return;
        
        isAttacking = true;
        attackCooldownTimer = attackCooldown;
        
        PlayerFacing facing = playerMovement.GetCurrentFacing();
        characterAnimator.PlayAnimation($"thrust_{facing.ToString().ToLower()}", false);

        Debug.Log("MeleeAttack used!");
        
        UpdateNetworkState();
    }
    
    private void TrySpellCast()
    {
        if (!isLocalPlayer || isAttacking || isCasting) return;
        
        isCasting = true;
        spellCastCooldownTimer = spellCastCooldown;
        
        PlayerFacing facing = playerMovement.GetCurrentFacing();
        characterAnimator.PlayAnimation($"spellcast_{facing.ToString().ToLower()}", false);
        
        Debug.Log("Spellcast used!");

        UpdateNetworkState();
    }
    
    private void UpdateNetworkState()
    {
        if (networkCharacter == null) return;
        
        CharacterState newState;
        PlayerFacing facing = playerMovement.GetCurrentFacing();
        
        if (isCasting)
            newState = CharacterState.Casting;
        else if (isAttacking)
            newState = CharacterState.Attacking;
        else
            newState = playerMovement.IsMoving() ? CharacterState.Running : CharacterState.Idle;
        
        networkCharacter.CmdUpdateState(newState, facing);
    }
    
    public bool IsPerformingAction()
    {
        return isAttacking || isCasting;
    }
}