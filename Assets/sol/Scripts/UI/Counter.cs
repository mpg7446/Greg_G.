using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public TMP_Text counterText;
    public GameObject player;
    private PlayerInventory inventory;

    public int Score { get { return score; } }
    private int score;
    private string name; // Name is set up for Photon Nicknames if we decide to use it

    public void Init(GameObject player)
    {
        this.player = player;
        if (counterText == null)
            counterText = GetComponent<TMP_Text>();
        if (inventory == null)
            inventory = this.player.GetComponent<PlayerInventory>();

        name = "Player " + player.GetComponent<PlayerID>().GetID();
    }
    private void FixedUpdate()
    {
        score = inventory.itemCount;
        counterText.text = $"{name}'s Score: {score}";
    }

    public void IncreaseScore(int amount = 1)
    {
        score += amount;
    }
}