using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Create Input Controller Variable
    public static InputController inputController;
    public List<Vector2> movement = new List<Vector2>();
    public List<bool> action = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        // Instanciate Input Controller
        inputController = new InputController();

        // action Button
        action.Add(false);
        action.Add(false);

        inputController.MasterControls.P1_Action.performed += P1ActionPerformed;
        inputController.MasterControls.P1_Action.canceled += P1ActionCancelled;

        inputController.MasterControls.P2_Action.performed += P2ActionPerformed;
        inputController.MasterControls.P2_Action.canceled += P2ActionCancelled;

        // Movement
        movement.Add(new Vector2(0,0));
        movement.Add(new Vector2(0,0));

        inputController.MasterControls.P1_Movement.performed += P1MovementPerformed;
        inputController.MasterControls.P1_Movement.canceled += P1MovementCanceled;

        inputController.MasterControls.P2_Movement.performed += P2MovementPerformed;
        inputController.MasterControls.P2_Movement.canceled += P2MovementCanceled;


        inputController.Enable();
    }

    // Input Functions
    // Player 1 Movement
    void P1MovementPerformed(InputAction.CallbackContext context)
    {
        movement[0] = context.ReadValue<Vector2>();
    }
    void P1MovementCanceled(InputAction.CallbackContext context)
    {
        movement[0] = Vector2.zero;
    }
    // Player 2 Movement
    void P2MovementPerformed(InputAction.CallbackContext context)
    {
        movement[1] = context.ReadValue<Vector2>();
    }
    void P2MovementCanceled(InputAction.CallbackContext context)
    {
        movement[1] = Vector2.zero;
    }

    // Player 1 Action
    void P1ActionPerformed(InputAction.CallbackContext context)
    {
        action[0] = true;
    }
    void P1ActionCancelled(InputAction.CallbackContext context)
    {
        action[0] = false;
    }
    // Player 2 Action
    void P2ActionPerformed(InputAction.CallbackContext context)
    {
        action[1] = true;
    }
    void P2ActionCancelled(InputAction.CallbackContext context)
    {
        action[1] = false;
    }
}
