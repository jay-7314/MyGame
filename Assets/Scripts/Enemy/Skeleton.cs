using UnityEngine;

public class Skeleton : Enemy
{
    enum AIState
    {
        Patrol,
        Chase,
        Attack
    }

    //플레이어 추적
    [SerializeField] float chaseRange = 5f;                                 //플레이어 감지반경
    [SerializeField] float attackRange = 1f;                                //공격 사거리

    //절벽감지
    [SerializeField] LayerMask groundLayer;                            //바닥 레이어
    [SerializeField] Transform groundCheckPoint;                    //바닥 체크하는 Transform
    [SerializeField] float groundCheckDistance = 0.5f;              //바닥체크하는 거리

    //애니메이션 및 공격관련
    [SerializeField] Animator anim;                                          //애니메이션 가져오기
    [SerializeField] float attackCooldown = 1.5f;                       //공격 쿨다운
    [SerializeField] float dieDestroyDeley = 1f;                         //죽는 시간

    AIState currentState = AIState.Patrol;                                  //현재 상태는 추적상태
    int patroDir = 1;                                                                   //방향전환. 약간 flip으로 생각하면 됨
    float lastAttackTime = -999f;                                               //마지막 공격하고 나서 쿨타임을 주기 위함
    bool isDead = false;                                                             //죽은상태 확인
    Rigidbody2D rb; 

    //아래는 애니메이션 코드를 작성할때 오타가 날수 있어서 정리한 부분
    static readonly int speedParam = Animator.StringToHash("Speed");
    static readonly int attackTriggerParam = Animator.StringToHash("AttackTrigger");
    static readonly int AttackIndexParam = Animator.StringToHash("AttackIndex");
    static readonly int HitTriggerParam = Animator.StringToHash("HitTrigger");
    static readonly int IsDeadParam = Animator.StringToHash("isDead");

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D를 가져온다.
    }

    //몬스터 상태 변경
    void UpdateState()
    {
        if (player == null)              //플레이어가 없다면
        {
            currentState = AIState.Patrol;          //기본상태인 순찰상태로 돌아간다.
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);             //플레이어와 몬스터와의 거리
        if (dist <= attackRange)                                                                              //공격 범위보다 dist가 작다면
        {
            currentState = AIState.Attack;                                                              //상태값은 공격으로 바꾼다.
        }
        else if (dist <= chaseRange)                                                                         //추적상태의 범위보다 dist가 작다면
        {
            currentState = AIState.Chase;                                                                   //추적상태로 바꾼다.
        }
        else
        {
            currentState = AIState.Patrol;                                                                          //아무상태가 아니라면 순찰상태로 바꾼다.
        }
    }

    //지정한 방향 앞에 절벽이 있는지 확인한다.
    bool IsClif(int dir)
    {
        if (groundCheckPoint == null) return false;                                                                                                              //땅에 있지 않다면 false

        Vector2 origin = groundCheckPoint.position;                                                                                                           //레이캐스트 시작 지점 설정
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);                        //땅아래에 광선을 쏜다.
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, hit.collider != null ? Color.green : Color.red);          //그 광선을 표시한다.
        return hit.collider == null;                                                                                                                                        // 바닥이 감지되지 않으면 절벽으로 판단
    }

    // 스프라이트를 좌우 반전시켜 이동 방향을 시각적으로 표현한다.
    void FlipVisual()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //움직임
    protected override void Move()
    {
        if (isDead) return;                     //죽으면 return시킨다.
        UpdateState();                          //상태를 변화시킨다.

        if (currentState == AIState.Attack)                 //상태값이 공격이라면
        {
            anim.SetFloat(speedParam, 0f);                  //공격중에는 움직이면 안된다.
            return;
        }

        int moveDir = patroDir;                                 // 기본 이동 방향은 순찰 방향으로 설정

        if (currentState == AIState.Chase && player != null)                    //상태값이 추적상태이고, 플레이어가 있다면
        {
            moveDir = player.position.x > transform.position.x ? 1 : -1;                    // 플레이어가 오른쪽이면 1, 왼쪽이면 -1로 추적 방향 결정, 플레이어 방향으로 움직인다.
        }

        if (IsClif(moveDir))
        {
            // 순찰, 추적 둘 다 절벽 앞에서는 멈춘다.
            if (currentState == AIState.Patrol)
            {
                patroDir *= -1;
                FlipVisual();
            }
            else if (currentState == AIState.Chase)
            {
                // 추적 중 절벽을 만나면 상태를 순찰로 되돌린다.
                currentState = AIState.Patrol;
            }
            anim.SetFloat(speedParam, 0f);
            return;
        }

        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);                             //오른쪽으로 이동하는 로직
        anim.SetFloat(speedParam, speed);                                                                                       //속도를 내라

        bool facingRight = transform.localScale.x > 0f;                                                                         //x의 방향이 0보다 크면
        if ((moveDir == 1 && !facingRight) || (moveDir == -1 && facingRight))                                           //왼쪽, 오른쪽 구분
        {
            FlipVisual();                                                                                                                       //구분해서 스프라이트 전환시킨다.
        }
    }

    //공격
    protected override void Attack()
    {
        if (isDead) return;                                             //죽으면 끝
        if (currentState != AIState.Attack) return;                         //현재 상태가 공격이 아니면 끝
        if (Time.time < lastAttackTime + attackCooldown) return;                                        //공격 쿨타임이 해당되지 않으면 끝

        lastAttackTime = Time.time;                                                                     // 마지막 공격 시간을 현재 시간으로 갱신한다.
        anim.SetInteger(AttackIndexParam, Random.Range(0, 2));                      // 공격 애니메이션을 랜덤으로 선택한다.
        anim.SetTrigger(attackTriggerParam);                                                //공격에 트리거를 준다.
    }

    //실제 데미지 처리함수
    public void DealDamage()
    {
        if (isDead) return;
        if (player == null) return;
        if (Vector2.Distance(transform.position, player.position) > attackRange) return;                        // 애니메이션이 재생되는 동안 플레이어가 공격 범위를 벗어났으면 데미지를 주지 않는다.

        IDamageable damageable = player.GetComponent<IDamageable>();                                            // 플레이어가 데미지를 받을 수 있는지 확인한다.
        damageable?.TakeDamage(attackPower);                            // 데미지를 입힌다.
    }

    protected override float CalculateContactDamage()
    {
        return attackPower;
    }

    public override void TakeDamage(float damage)
    {
        if (isDead || currentHealth <= 0) return;
        bool willDie = currentHealth - damage <= 0;
        base.TakeDamage(damage);

        if (!willDie)
        {
            anim.SetTrigger(HitTriggerParam);
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetBool(IsDeadParam, true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Invoke(nameof(FinishDeath), dieDestroyDeley);
    }

    void FinishDeath()
    {
        base.Die();
    }
}
