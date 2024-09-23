using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public TMP_Text counterText;
    public GameObject player;
    private PlayerInventory inventory;
    public int Score;

    public void Init(GameObject player)
    {
        this.player = player;
        if (counterText == null)
            counterText = GetComponent<TMP_Text>();
        if (inventory == null)
            inventory = this.player.GetComponent<PlayerInventory>();
    }
    private void FixedUpdate()
    {
        Score = inventory.itemCount;
        counterText.text = $"{player.GetComponent<PlayerID>().GetID()}'s Score: {Score}";
    }
}
