using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerMovement : PlayerMovement
{
    protected override void Action()
    {
        if (base.inStack)
        {
            base.LeaveStack();
        } else
        {
            Debug.Log("Action pressed and not in stack, possibly have this change player customization??");
        }
    }
}
