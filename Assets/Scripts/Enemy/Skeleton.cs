using UnityEngine;
using DG.Tweening;

public class Skeleton : Enemy
{
    #region 매개변수
    enum AIState
    {
        Patrol,
        Chase,
        Attack
    }

    [SerializeField] float chaseRange = 5f;                                 
    [SerializeField] float attackRange = 1f;                             

    [SerializeField] float patrolDistance = 2f;                            
    [SerializeField] float patrolSpeed = 2f;                                   
    [SerializeField] float patrolWaitTime = 0.5f;                           

    [SerializeField] Animator anim;                                       
    [SerializeField] Transform healthBarTransform;
    [SerializeField] float attackCooldown = 1.5f;                       
    [SerializeField] float dieDestroyDeley = 1f;                       
    [SerializeField] Collider2D attackHitbox;

    [SerializeField] DialogueData deathDialogue;  

    [SerializeField] bool debugLog = true;                                    
    [SerializeField] float debugLogInterval = 0.5f;                  
    float lastDebugLogTime = -999f;

    AIState currentState = AIState.Patrol;                                  
    AIState previousState = AIState.Patrol;                              
    float lastAttackTime = -999f;                                              
    bool isDead = false;                                                        
    Rigidbody2D rb;

    Tween patrolTween;                                                           
    float basePosX;                                                                        
    float patrolTargetX;                                                                 
    bool isAttackingAnim = false;
    static readonly int speedParam = Animator.StringToHash("Speed");
    static readonly int attackTriggerParam = Animator.StringToHash("AttackTrigger");
    static readonly int AttackIndexParam = Animator.StringToHash("AttackIndex");
    static readonly int HitTriggerParam = Animator.StringToHash("HitTrigger");
    static readonly int IsDeadParam = Animator.StringToHash("isDead");
    static readonly int IsAttackingParam = Animator.StringToHash("isAttacking");

    #endregion

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>(); 

    }

    protected override void Start()
    {
        base.Start();  

        basePosX = transform.position.x;
        patrolTargetX = basePosX + patrolDistance;
    }

    public void EnableAttackHitbox()
    {
        if (attackHitbox != null) attackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null) attackHitbox.enabled = false;
    }

    void UpdateState()
    {
        if (player == null)            
        {
            currentState = AIState.Patrol;        

            if (debugLog && Time.time >= lastDebugLogTime + debugLogInterval)
            {
                lastDebugLogTime = Time.time;
            }
            return;
        }
        if (isAttackingAnim)
        {
            currentState = AIState.Attack;

            if (debugLog && Time.time >= lastDebugLogTime + debugLogInterval)
            {
                lastDebugLogTime = Time.time;
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);            
        AIState newState;

        if (dist <= attackRange)                                                                            
        {
            newState = AIState.Attack;                                                             
        }
        else if (dist <= chaseRange)                                                                         
        {
            newState = AIState.Chase;                                                                 
        }
        else
        {
            newState = AIState.Patrol;                                                                        
        }

        currentState = newState;
    }

    void FlipVisual()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (healthBarTransform != null)
        {
            Vector3 hbScale = healthBarTransform.localScale;
            hbScale.x *= -1;
            healthBarTransform.localScale = hbScale;
        }
    }

    void FaceDirection(float moveDir)
    {
        bool facingRight = transform.localScale.x > 0f;                                                          
        if ((moveDir > 0f && !facingRight) || (moveDir < 0f && facingRight))                               
        {
            FlipVisual();                                                                                                          
        }
    }

    //움직임
    protected override void Move()
    {
        if (isDead) return;                     
        UpdateState();                        

        if (currentState != previousState)
        {
            OnStateChanged(previousState, currentState);
            previousState = currentState;
        }

        if (currentState == AIState.Attack)                 
        {
            anim.SetFloat(speedParam, 0f);                  

 
            if (!isAttackingAnim && player != null)
            {
                float moveDir = player.position.x > transform.position.x ? 1f : -1f;
                FaceDirection(moveDir);
            }
            return;
        }

        if (currentState == AIState.Chase && player != null)                  
        {
            ChaseMove();                                                                                   
            return;
        }

        if (currentState == AIState.Patrol && (patrolTween == null || !patrolTween.IsActive()))
        {
            StartPatrolTween();
        }
    }

    void OnStateChanged(AIState from, AIState to)
    {
        if (to != AIState.Patrol)
        {
            KillPatrolTween();                                                                                     
        }
        else if (from != AIState.Patrol)
        {
            float leftEnd = basePosX - patrolDistance;
            float rightEnd = basePosX + patrolDistance;
            float distToLeft = Mathf.Abs(transform.position.x - leftEnd);
            float distToRight = Mathf.Abs(transform.position.x - rightEnd);
            patrolTargetX = distToLeft <= distToRight ? leftEnd : rightEnd;

            StartPatrolTween();
        }
    }

    void ChaseMove()
    {
        float moveDir = player.position.x > transform.position.x ? 1f : -1f;                  
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);                            
        anim.SetFloat(speedParam, speed);                                                                                     
        FaceDirection(moveDir);
    }

    void StartPatrolTween()
    {
        KillPatrolTween();                                                                                             

        float dist = Mathf.Abs(patrolTargetX - transform.position.x);
        float duration = patrolSpeed > 0f ? dist / patrolSpeed : 0f;                      

        float moveDir = patrolTargetX > transform.position.x ? 1f : -1f;
        FaceDirection(moveDir);
        anim.SetFloat(speedParam, patrolSpeed);                                                       

        patrolTween = transform.DOMoveX(patrolTargetX, duration)
            .SetEase(Ease.Linear)                                                                                   
            .OnComplete(OnPatrolPointReached);
    }

    void OnPatrolPointReached()
    {
        anim.SetFloat(speedParam, 0f);                                                                             

        float leftEnd = basePosX - patrolDistance;
        float rightEnd = basePosX + patrolDistance;
        patrolTargetX = Mathf.Approximately(patrolTargetX, rightEnd) ? leftEnd : rightEnd;              

        patrolTween = DOVirtual.DelayedCall(patrolWaitTime, () =>
        {
            if (currentState == AIState.Patrol)                                                                   
            {
                StartPatrolTween();
            }
        });
    }

    void KillPatrolTween()
    {
        if (patrolTween != null && patrolTween.IsActive())
        {
            patrolTween.Kill();
        }
        patrolTween = null;
    }

    public void OnAttackAnimationEnd()
    {
        isAttackingAnim = false;
        anim.SetBool(IsAttackingParam, false);
    }

    //공격
    protected override void Attack()
    {
        if (isDead) return;
        if (currentState != AIState.Attack) return;
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (isAttackingAnim) return;

        lastAttackTime = Time.time;
        isAttackingAnim = true;
        anim.SetBool(IsAttackingParam, true);

        anim.SetInteger(AttackIndexParam, Random.Range(0, 2));
        anim.SetTrigger(attackTriggerParam);
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

        KillPatrolTween();

        anim.SetBool(IsDeadParam, true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Invoke(nameof(FinishDeath), dieDestroyDeley);
    }

    void FinishDeath()
    {
        if (deathDialogue != null && DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(deathDialogue, OnDeathDialogueEnd);
        }
        else
        {
            base.Die();
        }
    }

    void OnDeathDialogueEnd()
    {
        base.Die(); 
    }

    void OnDestroy()
    {
        KillPatrolTween();                                                                              
    }
}