using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Create Input Controller Variable
    public static InputController inputController;
    public Vector2 movement = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        // Instanciate Input Controller
        inputController = new InputController();

        // Jump Button
        inputController.MasterControls.Jump.performed += JumpPerformerd;
        inputController.MasterControls.Jump.canceled += JumpCanceled;

        // Attack Button
        inputController.MasterControls.Attack.performed += AttackPerformerd;
        inputController.MasterControls.Attack.canceled += AttackCanceled;

        // Movement
        inputController.MasterControls.Movement.performed += MovementPerformed;
        inputController.MasterControls.Movement.canceled += MovementCanceled;

        inputController.Enable();
    }

    // Input Functions
    void MovementPerformed(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    void MovementCanceled(InputAction.CallbackContext context)
    {
        movement = Vector2.zero;
    }

    void JumpPerformerd(InputAction.CallbackContext context)
    {
        Debug.Log("jump pressed");
        if (context.control.device is UnityEngine.InputSystem.XInput.XInputControllerWindows)
        {
            Debug.Log("XBOX");
        }
    }
    void JumpCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("jump released");
    }

    void AttackPerformerd(InputAction.CallbackContext context)
    {
        Debug.Log("attack pressed");
    }
    void AttackCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("attack released");
    }
}
