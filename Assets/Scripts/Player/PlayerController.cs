using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    CircleCollider2D circleCollider;
    PlayerAttack playerAttack;
    public Image leftBtn, rightBtn;
    bool moveLeft;
    bool moveRight;
    bool isGround;
    bool isJump;
    bool leftHeld;
    bool rightHeld;
    bool isDashing = false;
    bool facingLeft = false;

    float horizontalMove;
    public LayerMask layerMask;
    public float movespeed;
    public float jumpForce;
    public float coyoteTime = 0.2f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    private float dashTimer;
    private float dashCooldownTimer;

    [SerializeField] Sprite dashSpriteImage; // Project 창에서 직접 드래그 연결

    private float coyoteTimeCounter;
    private float landingBuffer = 0f;
    private float landingBufferTime = 0.1f;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        rigid = player.GetComponent<Rigidbody2D>();
        sprite = player.GetComponent<SpriteRenderer>();
        anim = player.GetComponent<Animator>();
        anim.SetBool("isRun", false);
        playerAttack = player.GetComponent<PlayerAttack>();
        circleCollider = player.GetComponent<CircleCollider2D>();
        moveLeft = false;
        moveRight = false;
        isJump = false;
        leftHeld = false;
        rightHeld = false;
    }

    #region 이동 및 버튼 색상 변경 구현
    public void PushLeftBtn() { leftHeld = true; ApplyMoveState(); }
    public void UnPushLeftBtn() { leftHeld = false; ApplyMoveState(); }
    public void ExitLeftBtn() { leftHeld = false; ApplyMoveState(); }
    public void EnterLeftBtn() { leftHeld = true; ApplyMoveState(); }
    public void PushRightBtn() { rightHeld = true; ApplyMoveState(); }
    public void UnPushRightBtn() { rightHeld = false; ApplyMoveState(); }
    public void ExitRightBtn() { rightHeld = false; ApplyMoveState(); }
    public void EnterRightBtn() { rightHeld = true; ApplyMoveState(); }

    void ApplyMoveState()
    {
        if (leftHeld && !rightHeld)
        {
            moveLeft = true;
            moveRight = false;
            facingLeft = true;
            sprite.flipX = true;
            playerAttack.SetFacing(true);
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!isDashing) anim.SetBool("isRun", true);
            SetButtonAlpha(rightBtn, 0.3f);
            SetButtonAlpha(leftBtn, 1f);
        }
        else if (rightHeld && !leftHeld)
        {
            moveLeft = false;
            moveRight = true;
            facingLeft = false;
            sprite.flipX = false;
            playerAttack.SetFacing(false);
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!isDashing) anim.SetBool("isRun", true);
            SetButtonAlpha(rightBtn, 1f);
            SetButtonAlpha(leftBtn, 0.3f);
        }
        else
        {
            StopMove();
        }
    }

    void StopMove()
    {
        if (!isDashing) anim.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        moveRight = false;
        moveLeft = false;
        SetButtonAlpha(rightBtn, 1f);
        SetButtonAlpha(leftBtn, 1f);
    }
    void SetButtonAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
    #endregion

    #region 점프
    public void Jump()
    {
        if (coyoteTimeCounter > 0f && !isJump)
        {
            isJump = true;
            coyoteTimeCounter = 0f;
            anim.SetInteger("isJump", 1);
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    #endregion

    #region 대쉬
    public void Dash()
    {
        if (isDashing || dashCooldownTimer > 0f) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Animator가 매 프레임 sprite를 덮어쓰지 못하게 잠깐 꺼둠
        anim.enabled = false;

        if (dashSpriteImage != null)
        {
            sprite.sprite = dashSpriteImage;
        }
    }

    void UpdateDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        float dir = facingLeft ? -1f : 1f;
        rigid.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        if (dashTimer <= 0f)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        isDashing = false;

        // Animator 다시 켜서 원래 스프라이트 제어권 복구
        anim.enabled = true;

        // 대시 끝난 시점의 현재 상태(이동중/정지)에 맞게 애니메이션 복원
        if (moveLeft || moveRight)
            anim.SetBool("isRun", true);
        else
            anim.SetBool("isRun", false);
    }
    #endregion

    private void Update()
    {
        MovementPlayer();
#if UNITY_EDITOR
        HandleKeyboardInput();
#endif
    }

#if UNITY_EDITOR
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) PushLeftBtn();
        if (Input.GetKeyUp(KeyCode.LeftArrow)) UnPushLeftBtn();
        if (Input.GetKeyDown(KeyCode.RightArrow)) PushRightBtn();
        if (Input.GetKeyUp(KeyCode.RightArrow)) UnPushRightBtn();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.S)) Dash();
    }
#endif

    void MovementPlayer()
    {
        if (isDashing) return;

        if (moveLeft && !moveRight) horizontalMove = -movespeed;
        else if (moveRight && !moveLeft) horizontalMove = movespeed;
        else horizontalMove = 0;
    }

    private void FixedUpdate()
    {
        UpdateDash();

        if (!isDashing)
        {
            rigid.linearVelocity = new Vector2(horizontalMove, rigid.linearVelocity.y);
        }

        // 바닥 감지
        Vector2 boxSize = new Vector2(circleCollider.bounds.size.x * 0.9f, 0.5f);
        Vector2 boxOrigin = new Vector2(circleCollider.bounds.center.x, circleCollider.bounds.min.y);
        float castDistance = 0.1f;

        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, castDistance, layerMask);
        isGround = hit.collider != null;

        // 코요테 타임 갱신
        if (isGround)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        // 착지 버퍼 갱신
        if (isGround && rigid.linearVelocity.y <= 0f)
            landingBuffer = landingBufferTime;
        else
            landingBuffer -= Time.fixedDeltaTime;

        Debug.DrawRay(boxOrigin + Vector2.left * boxSize.x / 2, Vector2.down * castDistance, Color.red);
        Debug.DrawRay(boxOrigin + Vector2.right * boxSize.x / 2, Vector2.down * castDistance, Color.red);

        // 점프 상태 처리
        if (isJump)
        {
            if (rigid.linearVelocity.y > 0.1f)
                anim.SetInteger("isJump", 1);
            else if (rigid.linearVelocity.y < -0.1f)
                anim.SetInteger("isJump", 2);

            if (landingBuffer > 0f && rigid.linearVelocity.y <= 0.1f)
            {
                isJump = false;
                landingBuffer = 0f;
                anim.SetInteger("isJump", 0);

                if (!moveLeft && !moveRight)
                    anim.SetBool("isRun", false);
            }
        }
    }
}