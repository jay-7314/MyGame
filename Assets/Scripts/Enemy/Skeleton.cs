using UnityEngine;
using DG.Tweening;

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

    //순찰 (DOTween 이용. 기존엔 Raycast로 절벽을 감지했는데, 캐릭터가 절벽 경계선에 걸치면
    //반전->이동->다시반전 이 반복되면서 발판 끝에서 떠는 버그가 있어서 절벽감지 자체를 없애고
    //시작위치 기준 좌우 거리만큼만 왕복하는 방식으로 바꿈. 발판 폭보다 작게 잡아두면 절벽 문제 자체가 안생김.
    [SerializeField] float patrolDistance = 2f;                             //시작위치 기준 좌우로 얼마나 왕복할지
    [SerializeField] float patrolSpeed = 2f;                                     //순찰 이동속도
    [SerializeField] float patrolWaitTime = 0.5f;                             //왕복 지점 도착하고 잠깐 멈추는 시간

    //애니메이션 및 공격관련
    [SerializeField] Animator anim;                                          //애니메이션 가져오기
    [SerializeField] float attackCooldown = 1.5f;                       //공격 쿨다운
    [SerializeField] float dieDestroyDeley = 1f;                         //죽는 시간

    // ===== 디버그용 =====
    [Header("Debug")]
    [SerializeField] bool debugLog = true;                                    //디버그 로그 on/off
    [SerializeField] float debugLogInterval = 0.5f;                     //매 프레임 찍으면 콘솔 터지니까 간격 두고 찍음
    float lastDebugLogTime = -999f;
    // ====================

    AIState currentState = AIState.Patrol;                                  //현재 상태는 추적상태
    AIState previousState = AIState.Patrol;                               //상태가 바뀌는 순간을 잡아내려고 이전 프레임 상태를 저장해둠
    float lastAttackTime = -999f;                                               //마지막 공격하고 나서 쿨타임을 주기 위함
    bool isDead = false;                                                             //죽은상태 확인
    Rigidbody2D rb;

    Tween patrolTween;                                                              //순찰중인 트윈을 들고있어야 Chase전환시 죽일수있음
    float basePosX;                                                                        //순찰 왕복의 기준이 되는 시작 x좌표
    float patrolTargetX;                                                                 //지금 이동중인 목표 x좌표

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

    void Start()
    {
        basePosX = transform.position.x;                                    //스폰된 위치를 순찰 기준점으로 저장
        patrolTargetX = basePosX + patrolDistance;                //일단 오른쪽 끝을 목표로 시작
    }

    //몬스터 상태 변경
    void UpdateState()
    {
        if (player == null)              //플레이어가 없다면
        {
            currentState = AIState.Patrol;          //기본상태인 순찰상태로 돌아간다.

            if (debugLog && Time.time >= lastDebugLogTime + debugLogInterval)
            {
                lastDebugLogTime = Time.time;
                Debug.Log($"[Skeleton:{name}] player == null -> State: Patrol");
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);             //플레이어와 몬스터와의 거리
        AIState newState;

        if (dist <= attackRange)                                                                              //공격 범위보다 dist가 작다면
        {
            newState = AIState.Attack;                                                              //상태값은 공격으로 바꾼다.
        }
        else if (dist <= chaseRange)                                                                         //추적상태의 범위보다 dist가 작다면
        {
            newState = AIState.Chase;                                                                   //추적상태로 바꾼다.
        }
        else
        {
            newState = AIState.Patrol;                                                                          //아무상태가 아니라면 순찰상태로 바꾼다.
        }

        currentState = newState;

        // ===== 디버그 로그 =====
        if (debugLog && Time.time >= lastDebugLogTime + debugLogInterval)
        {
            lastDebugLogTime = Time.time;
            Debug.Log($"[Skeleton:{name}] dist={dist:F2} | chaseRange={chaseRange} | attackRange={attackRange} | State={currentState} | pos={transform.position.x:F2} | playerPos={player.position.x:F2} | velocity={(rb != null ? rb.linearVelocity : Vector2.zero)}");
        }
        // ======================
    }

    // 스프라이트를 좌우 반전시켜 이동 방향을 시각적으로 표현한다.
    void FlipVisual()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //이동 방향 보고 필요할때만 반전시키는 함수. Chase랑 Patrol 둘다 써서 따로 뺌
    void FaceDirection(float moveDir)
    {
        bool facingRight = transform.localScale.x > 0f;                                                                         //x의 방향이 0보다 크면
        if ((moveDir > 0f && !facingRight) || (moveDir < 0f && facingRight))                                           //왼쪽, 오른쪽 구분
        {
            FlipVisual();                                                                                                                       //구분해서 스프라이트 전환시킨다.
        }
    }

    //움직임
    protected override void Move()
    {
        if (isDead) return;                     //죽으면 return시킨다.
        UpdateState();                          //상태를 변화시킨다.

        //상태가 바뀐 프레임에만 트윈 정지/재시작 처리. 매프레임 체크하면 낭비라 이렇게함
        if (currentState != previousState)
        {
            if (debugLog)
            {
                Debug.Log($"[Skeleton:{name}] ★상태전환★ {previousState} -> {currentState}");
            }

            OnStateChanged(previousState, currentState);
            previousState = currentState;
        }

        if (currentState == AIState.Attack)                 //상태값이 공격이라면
        {
            anim.SetFloat(speedParam, 0f);                  //공격중에는 움직이면 안된다.
            return;
        }

        if (currentState == AIState.Chase && player != null)                    //상태값이 추적상태이고, 플레이어가 있다면
        {
            ChaseMove();                                                                                     //추적은 기존처럼 Rigidbody velocity로 이동
            return;
        }

        //Patrol인데 트윈이 죽어있으면 다시 시작 (Chase갔다 돌아왔을때 대비한 안전장치)
        if (currentState == AIState.Patrol && (patrolTween == null || !patrolTween.IsActive()))
        {
            StartPatrolTween();
        }
    }

    //Patrol <-> Chase/Attack 전환될때 트윈을 죽이거나 다시 살리는 처리
    void OnStateChanged(AIState from, AIState to)
    {
        if (to != AIState.Patrol)
        {
            KillPatrolTween();                                                                                     //추적/공격 들어가면 순찰트윈은 바로 죽여야함
        }
        else if (from != AIState.Patrol)
        {
            //Patrol로 복귀했을때 지금 위치에서 더 가까운 왕복 끝점을 다음 목표로 잡음
            //안그러면 복귀하자마자 반대편 끝까지 순간이동하듯 튕겨나가는 느낌이 남
            float leftEnd = basePosX - patrolDistance;
            float rightEnd = basePosX + patrolDistance;
            float distToLeft = Mathf.Abs(transform.position.x - leftEnd);
            float distToRight = Mathf.Abs(transform.position.x - rightEnd);
            patrolTargetX = distToLeft <= distToRight ? leftEnd : rightEnd;

            StartPatrolTween();
        }
    }

    //추적 이동. 기존 Move()에 있던 velocity 이동 로직 그대로 가져옴
    void ChaseMove()
    {
        float moveDir = player.position.x > transform.position.x ? 1f : -1f;                    // 플레이어가 오른쪽이면 1, 왼쪽이면 -1로 추적 방향 결정
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);                             //플레이어 방향으로 이동하는 로직
        anim.SetFloat(speedParam, speed);                                                                                       //속도를 내라
        FaceDirection(moveDir);

        if (debugLog && Time.time >= lastDebugLogTime + debugLogInterval)
        {
            // UpdateState에서 이미 이번 프레임 로그를 찍었을 수 있어서 시간 갱신은 안 하고 별도로 찍음
            Debug.Log($"[Skeleton:{name}] ChaseMove 실행중 | moveDir={moveDir} | speed={speed} | 결과 velocity={rb.linearVelocity}");
        }
    }

    //순찰 트윈 시작. patrolTargetX까지 DOMoveX로 이동시키고 도착하면 반대편으로 돌림
    void StartPatrolTween()
    {
        KillPatrolTween();                                                                                             //혹시 남아있는 트윈있으면 정리하고 새로 시작

        float dist = Mathf.Abs(patrolTargetX - transform.position.x);
        float duration = patrolSpeed > 0f ? dist / patrolSpeed : 0f;                        //거리/속도로 걸리는 시간 계산해서 속도감 일정하게 유지

        float moveDir = patrolTargetX > transform.position.x ? 1f : -1f;
        FaceDirection(moveDir);
        anim.SetFloat(speedParam, patrolSpeed);                                                        //속도를 내라

        patrolTween = transform.DOMoveX(patrolTargetX, duration)
            .SetEase(Ease.Linear)                                                                                   //등속으로 움직여야 자연스러움
            .OnComplete(OnPatrolPointReached);
    }

    //왕복 끝점에 도착했을때. 잠깐 대기하고 반대편으로 목표를 바꿔서 다시 출발
    void OnPatrolPointReached()
    {
        anim.SetFloat(speedParam, 0f);                                                                             //도착하면 잠깐 멈춰야 자연스러움

        float leftEnd = basePosX - patrolDistance;
        float rightEnd = basePosX + patrolDistance;
        patrolTargetX = Mathf.Approximately(patrolTargetX, rightEnd) ? leftEnd : rightEnd;               //다음 목표는 반대쪽 끝

        patrolTween = DOVirtual.DelayedCall(patrolWaitTime, () =>
        {
            if (currentState == AIState.Patrol)                                                                     //대기하는 동안 Chase로 바뀌었으면 다시 출발하면 안됨
            {
                StartPatrolTween();
            }
        });
    }

    //순찰 트윈 정리. Chase전환/사망시 반드시 호출해줘야 메모리에 안남음
    void KillPatrolTween()
    {
        if (patrolTween != null && patrolTween.IsActive())
        {
            patrolTween.Kill();
        }
        patrolTween = null;
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

        if (debugLog)
        {
            Debug.Log($"[Skeleton:{name}] Attack() 실행됨 (Trigger 발동)");
        }
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

        KillPatrolTween();                                                                              //죽을때 순찰 트윈 살아있으면 안되니까 정리

        anim.SetBool(IsDeadParam, true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Invoke(nameof(FinishDeath), dieDestroyDeley);
    }

    void FinishDeath()
    {
        base.Die();
    }

    void OnDestroy()
    {
        KillPatrolTween();                                                                              //오브젝트 파괴될때 트윈 남아있으면 에러나니까 정리
    }
}