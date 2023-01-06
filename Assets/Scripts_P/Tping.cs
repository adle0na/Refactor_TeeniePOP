using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tping : MonoBehaviour
{
    public int ID;

    public GameObject SelectSprite;
    public bool IsSelect { get; private set; }

    public GameObject clearEffect;

    public Animator clearAnim;

    private bool effectCheck = false;

    private Rigidbody2D      _rigid;
    private CircleCollider2D _circle;

    [SerializeField]
    private CircleCollider2D _activeRange;
    
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

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (IsSelect)
        {
            List<Tping> _SelectedPings = new List<Tping>();
            
            Tping other = collision.gameObject.GetComponent<Tping>();

            if (LevelManager.Instance._selectPings.Count < 20)
            {
                _SelectedPings.Add(other);
            }
            else
            {
                return;
            }
            
            for (int i = 0; i < _SelectedPings.Count; i++)
            {
                if (_SelectedPings[i].ID == LevelManager.Instance._selectPings[0].ID && !_SelectedPings[i].IsSelect)
                {
                    LevelManager.Instance._selectPings.Add(_SelectedPings[i]);
                    _SelectedPings[i].IsSelect = false;
                }
            }
        }

    }
}
