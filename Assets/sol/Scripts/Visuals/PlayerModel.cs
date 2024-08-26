using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

[CreateAssetMenu(menuName = "Player Model", fileName = "New Player Model")]
public class PlayerModel : ScriptableObject
{
    public PlayerVisual playerVisual;
    public Sprite sprite;
    public Vector3 offset = new Vector3(0, 0);
    public Vector3 scale = new Vector3(1, 1);
}
