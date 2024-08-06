using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Create Input Controller Variable
    public static InputController inputController;
    public Vector2 movement;
    public bool action;

    // Start is called before the first frame update
    private void Start()
    {
        // Instanciate Input Controller
        inputController = new InputController();

        // action Button
        action = false;

        inputController.MasterControls.P1_Action.performed += ActionPerformed;
        inputController.MasterControls.P1_Action.canceled += ActionCanceled;

        // Movement
        movement = new Vector2(0, 0);

        inputController.MasterControls.P1_Movement.performed += MovementPerformed;
        inputController.MasterControls.P1_Movement.canceled += MovementCanceled;

        inputController.Enable();
    }

    private void OnDestroy()
    {
        Debug.Log("Input Manager: closing manager");
        inputController.Disable();
        inputController = null;
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

    // Player 1 Action
    void ActionPerformed(InputAction.CallbackContext context)
    {
        action = true;
    }
    void ActionCanceled(InputAction.CallbackContext context)
    {
        action = false;
    }
}
