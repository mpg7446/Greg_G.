using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance;

    public List<GameObject> counters = new List<GameObject>();
    public GameObject counter;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;

        CreateCounters();

        //AddCounters();
        //SpawnCounters();
    }

    private void CreateCounters()
    {
        counters = new List<GameObject>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (counter != null)
            {
                GameObject newCounter = Instantiate(counter, gameObject.transform);
                newCounter.GetComponent<Counter>().player = player;
                newCounter.name = player.name + "_Counter";
                counters.Add(newCounter);
            }
        }
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
