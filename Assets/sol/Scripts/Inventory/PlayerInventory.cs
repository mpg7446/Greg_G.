using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int itemCount = 0;

    public void PickupItem(int amount = 1)
    {
        itemCount += amount;
    }
}
