using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerManager : PlayerManager
{
    protected override void Action()
    {
        /*if (base.inStack)
        {
            base.LeaveStack();
        } else*/
        {
            Debug.Log("Action pressed and not in stack, possibly have this change player customization??");
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) { }
    protected override void OnCollisionExit2D(Collision2D collision) { }
    protected override void SwitchCollider(bool enabled) { }
}
