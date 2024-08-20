using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

[CreateAssetMenu(menuName = "Player Model", fileName = "New Player Model")]
public class PlayerModel : MonoBehaviour
{
    public PlayerVisual playerVisual;

    // Objects
    private GameObject cardboard;
    private GameObject stickers;

    public bool Squished {  get; private set; }

    public void Start()
    {
        if (stickers == null)
        {
            stickers = GameObject.Find("Stickers");
        }
    }

    public void Squish()
    {
        stickers.transform.localScale = new Vector3(1, 0.5f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y / 4), 0);
        Squished = true;
    }
    public void UnSquish()
    {
        stickers.transform.localScale = Vector3.one;
        Squished = false;
    }
}
