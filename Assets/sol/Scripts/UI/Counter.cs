using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour, IComparable<Counter>
{
    public TMP_Text counterText;
    public GameObject player;
    public int playerID {  get; private set; }
    private PlayerInventory inventory;

    public int Score { get; private set; }
    private new string name; // Name is set up for Photon Nicknames if we decide to use it

    public void Init(GameObject player)
    {
        this.player = player;
        if (counterText == null)
            counterText = GetComponent<TMP_Text>();
        if (inventory == null)
            inventory = this.player.GetComponent<PlayerInventory>();

        playerID = player.GetComponent<PlayerID>().GetID();
        name = $"Player {playerID}";
        Score = 0;
        counterText.text = $"{name}'s Score: {Score}";
    }
    private void FixedUpdate()
    {
        //Score = inventory.itemCount;
        //counterText.text = $"{name}'s Score: {Score}";
    }

    public void IncreaseScore(int amount = 1)
    {
        Score += amount;
        counterText.text = $"{name}'s Score: {Score}";
    }

    public int CompareTo(Counter obj)
    {
        return obj.Score - Score;
    }
}