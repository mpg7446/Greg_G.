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

    // Start is called before the first frame update
    void Start()
    {
        // Instanciate Input Controller
        inputController = new InputController();

        // action Button
        inputController.MasterControls.P1_Action.performed += P1ActionPerformed;
        inputController.MasterControls.P1_Action.canceled += P1ActionCancelled;

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
    void P1MovementPerformed(InputAction.CallbackContext context)
    {
        movement[0] = context.ReadValue<Vector2>();
    }
    void P1MovementCanceled(InputAction.CallbackContext context)
    {
        movement[0] = Vector2.zero;
    }

    void P2MovementPerformed(InputAction.CallbackContext context)
    {
        movement[1] = context.ReadValue<Vector2>();
    }
    void P2MovementCanceled(InputAction.CallbackContext context)
    {
        movement[1] = Vector2.zero;
    }

    void P1ActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("p1 action pressed");
    }
    void P1ActionCancelled(InputAction.CallbackContext context)
    {
        Debug.Log("p1 action released");
    }

    void P2ActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("p2 action pressed");
    }
    void P2ActionCancelled(InputAction.CallbackContext context)
    {
        Debug.Log("p2 action released");
    }
}
