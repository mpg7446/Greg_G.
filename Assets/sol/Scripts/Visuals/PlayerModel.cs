using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

[CreateAssetMenu(menuName = "Player Model", fileName = "New Player Model")]
public class PlayerModel : ScriptableObject
{
    public PlayerVisual playerVisual;
    public Sprite sprite;
    public Vector2 offset;
}
