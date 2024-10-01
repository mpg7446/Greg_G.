using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance;

    public List<Counter> counters = new List<Counter>();
    public GameObject counter;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;

        //CreateCounters();

        //AddCounters();
        //SpawnCounters();
    }

    //private void CreateCounters()
    //{
    //    counters = new List<GameObject>();
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //    foreach (GameObject player in players)
    //    {
    //        if (counter != null)
    //        {
    //            GameObject newCounter = Instantiate(counter, gameObject.transform);
    //            newCounter.GetComponent<Counter>().player = player;
    //            newCounter.name = player.GetComponent<PlayerID>().GetID() + "_Counter";
    //            counters.Add(newCounter);
    //        }
    //    }
    //}

    public void AddCounter(GameObject obj)
    {
        Counter newCounter = Instantiate(counter, gameObject.transform).GetComponent<Counter>();
        newCounter.Init(obj);
        counters.Add(newCounter);
    }

    public void IncreaseCounter(GameObject obj, int amount = 1)
    {
        foreach(Counter counter in counters)
        {
            if (counter.player == obj)
                counter.IncreaseScore(amount);
        }

        //ReOrder();
        counters.Sort();

        //get the children
        //set their index to their counter index
        for (int i = 0; i < counters.Count; i++)
        {
            counters[i].transform.SetSiblingIndex(i);
        }
        Debug.Log("SCORE COUNT ER SORTED");
    }

    /*private void ReOrder()
    {
        List<GameObject> newCounters = new List<GameObject>();

        foreach(GameObject counter in counters)
        {
            int index = 0;
            foreach(GameObject newCounter in newCounters)
            {
                if (newCounter.GetComponent<Counter>().Score < counter.GetComponent<Counter>().Score)
                    index++;
            }

            newCounters.Insert(index, counter);
        }

        Debug.Log("ScoreCounter: Scores Reordered!");
        counters = newCounters;
    }*/

    public void ClearCounters()
    {
        foreach (Counter counter in counters)
        {
            Destroy(counter.gameObject);
        }
        counters = new List<Counter>();
    }

    //public void AddCounters()
    //{
    //    counters = new Dictionary<PlayerInventory, int>();
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject player in players)
    //        counters.Add(player.GetComponent<PlayerInventory>(), counters.Count);
    //}

    //public void SpawnCounters()
    //{
    //    foreach (KeyValuePair<PlayerInventory, int> counter in counters)
    //    {
    //        Debug.Log(counter.Value);
    //    }
    //}
}
