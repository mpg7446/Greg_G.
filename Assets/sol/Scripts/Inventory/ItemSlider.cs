using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlider : MonoBehaviour
{
    public GameObject cursor;
    public float cursorHeight;
    public float cursorWidth;
    private Vector3 cursorOrigin;

    public float cursorSway;

    public float value;
    public float maxValue;

    public void Start()
    {
        cursorOrigin = cursor.transform.localPosition;
    }

    public void Update()
    {
        Vector3 newloc = new Vector3(cursorOrigin.x + ((value * cursorWidth) - (maxValue/2 * cursorWidth)), (Mathf.Sin(value / (maxValue / Mathf.PI)) * cursorHeight) + cursorOrigin.y, 0);
        cursor.transform.SetLocalPositionAndRotation(newloc, Quaternion.Euler(0, 0, -(value - (maxValue/2)) * cursorSway));
    }
}
