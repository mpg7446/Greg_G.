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

    public override void CollisionEnter(Collision2D collision) { }
    public override void CollisionExit(Collision2D collision) { }
    protected override void SwitchCollider(bool enabled) { }
}
