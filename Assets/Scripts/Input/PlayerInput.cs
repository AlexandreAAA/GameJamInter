using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool interact;
    public bool attack;
    public bool crouch;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool analogMovement;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        AttackInput(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        CrouchInput(value.isPressed);
    }

    

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        if (!crouch)
        {
            jump = newJumpState;
        }
    }

    public void InteractInput(bool newInteract)
    {
        interact = newInteract;
    }

    public void AttackInput(bool newAattck)
    {
        attack = newAattck;
    }

    public void CrouchInput(bool newCrouchState)
    {
        crouch = !crouch;
    }


}
