using System;
using UnityEngine;


/// <summary>
/// 해당코드는 Enemy의 추상클래스이며 상속의 부모가 되는 스크립트임.
/// </summary>
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 1f;                                //체력 비율
    [SerializeField] protected float speed = 1f;                                       //속도 비율
    [SerializeField] protected float attackPower;                                   //공격력
    [SerializeField] protected string enemyTalk;                                    //적이 말하는 의성어 글자

    protected float currentHealth;                                                          //현재 체력
    protected Transform player;                                                             //플레이어 위치 파악

    public event Action<Enemy> OnDeath;                                             //죽은 상태를 다른 컴포넌트도 알수있게 조치
    public float healthRatio => currentHealth / maxHealth;                     //체력 비율을 계산
    protected virtual void Awake()
    {
        currentHealth = maxHealth;                                                          //태어났을때는 현재체력은 최대채력임
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()     //움직이는 것과 공격하는건 업데이트에서 진행
    {
        if (currentHealth <= 0) return;                 //죽으면 이동/공격 안함.
        Move();
        Attack();
    }

    public virtual void TakeDamage(float damage)                //공격받는거
    {
        if (currentHealth <= 0) return;                                     //체력이 0이면 공격못받음
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke(this);                                              //죽은거를 다 알려줌
        Destroy(gameObject);                                               //죽어서 게임오브젝트 삭제함
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)         //닿으면 공격됨
    {
        if (!collision.gameObject.CompareTag("Player")) return;               //적끼리 부딛히는건 상관 없음
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(CalculateContactDamage());
    }


    //아래 부분은 상속을 하긴 하지만, 몬스터별로 특징이 있어서 정리함
    protected abstract float CalculateContactDamage();      //몬스터별로 공격 계산이 다름
    protected abstract void Move();                             //몬스터별로 이동 스타일이 다름
    protected abstract void Attack();                           //몬스터별로 공격 스타일이 다름
}
