using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class Bomb : MonoBehaviour
{
    [HideInInspector]
    // 특수 블록 크기
    public int bomb_num;
    
    [HideInInspector]
    // 특수블록 단계
    public int level;
    
    [HideInInspector]
    // 스테이트 체크
    public bool isMergy;
    
    // 싱글톤 사용
    public static Bomb BInstance { get; set; }
    
    // 기본 물리, 애니메이션
    private Rigidbody2D      _rigid;
    private CircleCollider2D _circle;
    
    [HideInInspector]
    public  Animator         _bombAnim;

    private void Awake()
    {
        BInstance = this;
        
        _rigid    = GetComponent<Rigidbody2D>();
        _circle   = GetComponent<CircleCollider2D>();
        _bombAnim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        _bombAnim.SetInteger("Level", level);
        _bombAnim.SetInteger("Bomb_num", bomb_num);
    }

    // 클릭 이벤트 체크
    private void OnMouseDown()
    {
        LevelManager.Instance.BombDown(this);
    }

    // 폭탄끼리 접촉 이벤트
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 태그 체크
        if (collision.gameObject.tag == "Bomb")
        {
            Bomb other = collision.gameObject.GetComponent<Bomb>();
            // 크기가 다르면 큰쪽으로 합쳐짐
            if (level == other.level && !isMergy && !other.isMergy && level < 4)
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                int meN    = bomb_num;
                int otherN = other.bomb_num;

                if (meN > otherN)
                {
                    other.Hide(transform.position);
                        
                    LevelUp();
                }
                else
                {
                    Hide(transform.position);
                    
                    other.LevelUp();
                }
            }
        }
    }

    #region 특수 블록 합체 관련 함수
    
    public void Hide(Vector3 targetPos)
    {
        isMergy = true;

        _rigid.simulated = false;
        _circle.enabled  = false;

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while (frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);
            yield return null;
        }

        isMergy = false;
        gameObject.SetActive(false);
    }
    
    void LevelUp()
    {
        isMergy = true;

        _rigid.velocity = Vector2.zero;
        _rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }
    
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        _bombAnim.SetInteger("Level", level + 1);

        yield return new WaitForSeconds(0.3f);
        level++;

        isMergy = false;
    }
    
    #endregion
}
