using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tping : MonoBehaviour
{
    public int ID;

    public GameObject SelectSprite;
    public bool IsSelect { get; set; }

    public GameObject clearEffect;

    public Animator clearAnim;

    private bool effectCheck = false;

    private Rigidbody2D      _rigid;
    private CircleCollider2D _circle;

    public bool isBoom;
    private void Awake()
    {
        _rigid  = GetComponent<Rigidbody2D>();
        _circle = GetComponent<CircleCollider2D>();

        GetComponent<SpriteRenderer>();
        GetComponent<Animator>();
    }
    
    private void OnMouseDown()
    {
        LevelManager.Instance.PingDown(this);

        if (LevelManager.Instance.PopUsingCheck)
        {
            LevelManager.Instance.PopUse(this);
        }
    }

    private void OnMouseEnter()
    {
        //LevelManager.Instance.PingEnter(this);
    }

    private void OnMouseUp()
    {
        LevelManager.Instance.PingUp();
    }

    public void SetIsSelect(bool isSelect)
    {
        IsSelect = isSelect;
        SelectSprite.SetActive(isSelect);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsSelect)
        {
            if (collision.CompareTag("Tping"))
            {
                Tping other = collision.gameObject.GetComponent<Tping>();

                if (other.ID == LevelManager.Instance._selectedID &&
                    !LevelManager.Instance._selectPings.Contains(other))
                {
                    LevelManager.Instance._selectPings.Add(other);
                    other.SetIsSelect(true);
                }
            }
        }
    }
}
