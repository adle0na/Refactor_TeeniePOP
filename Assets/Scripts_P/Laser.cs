using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private BoxCollider2D _collider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tping"))
        {
            Tping other = collision.gameObject.GetComponent<Tping>();

            if (!LevelManager.Instance._erasePings.Contains(other))
            {
                LevelManager.Instance._erasePings.Add(other);
                other.gameObject.SetActive(false);
            }
        }
    }
}


