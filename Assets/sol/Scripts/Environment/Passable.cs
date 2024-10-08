using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Passable : MonoBehaviour
{
    private new Collider2D collider;
    private Collider2D playerCollider;
    private float col;
    [SerializeField] private float offset = 0f;
    [SerializeField] private float bounds = 0.1f;
    private Vector3 Offset { get { return new Vector3(0, offset, 0); } }

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            Destroy(this);
        col = Mathf.Abs(collider.bounds.max.y / transform.localScale.y);

        playerCollider = PlayerManager.Instance.gameObject.GetComponentInChildren<BoxCollider2D>();
        offset += collider.offset.y;
    }

    private void FixedUpdate()
    {
        if (!collider.enabled)
        {
            float max = (playerCollider.bounds.min.y / PlayerManager.Instance.gameObject.transform.localScale.y) - col + offset + bounds;
            //Debug.Log($"Passable: {name} collider disabled: {max} ({playerCollider.bounds.min.y} - {collider.bounds.max.y})");
            if (max > 0)
                collider.enabled = true;
        } else
        {
            float min = (playerCollider.bounds.min.y / PlayerManager.Instance.gameObject.transform.localScale.y) - col + offset - bounds;
            //Debug.Log($"Passable: {name} collider enabled: {min} ({playerCollider.bounds.min.y} - {collider.bounds.min.y})");
            if (min < 0)
                collider.enabled = false;
        }
    }
}
