using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Create Input Controller Variable
    private PhotonView photonView;
    public static InputController inputController;
    public Vector2 movement;
    public bool action;

    // Start is called before the first frame update
    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
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
        else
        {
            enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (inputController != null)
        {
            Debug.Log("Input Manager: closing manager");
            inputController.Disable();
            inputController = null;
        }
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
