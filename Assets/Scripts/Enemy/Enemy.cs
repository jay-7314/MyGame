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
    public event Action<float, float> OnHealthChanged;
    public float healthRatio => currentHealth / maxHealth;                     //체력 비율을 계산


    protected virtual void Awake()
    {
        currentHealth = maxHealth;   // 시작할 때 현재 체력을 최대 체력으로 초기화
    }
    protected virtual void Start()
    {
        // 체력바가 시작할 때부터 꽉 찬 상태로 보이도록 최초 1회 알림
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected virtual void Update()
    {
        if (currentHealth <= 0) return;

        // player가 없으면 다시 찾는다.
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        Move();
        Attack();
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);   // 음수로 안 내려가게 보정

        OnHealthChanged?.Invoke(currentHealth, maxHealth);   // 체력 변화 알림

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
