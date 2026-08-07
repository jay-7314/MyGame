using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    CircleCollider2D circleCollider;
    public Image leftBtn, rightBtn;
    bool moveLeft;
    bool moveRight;
    bool isGround;
    bool isJump;
    bool leftHeld;
    bool rightHeld;

    float horizontalMove;
    public LayerMask layerMask;
    public float movespeed;
    public float jumpForce;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float landingBuffer = 0f;
    private float landingBufferTime = 0.1f;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        rigid = player.GetComponent<Rigidbody2D>();
        sprite = player.GetComponent<SpriteRenderer>();
        anim = player.GetComponent<Animator>();
        player.GetComponent<Animator>().SetBool("isRun", false);
        circleCollider = player.GetComponent<CircleCollider2D>();
        moveLeft = false;
        moveRight = false;
        isJump = false;
        leftHeld = false;
        rightHeld = false;
    }

    #region 이동 및 버튼 색상 변경 구현
    public void PushLeftBtn()
    {
        leftHeld = true;
        ApplyMoveState();
    }
    public void UnPushLeftBtn()
    {
        leftHeld = false;
        ApplyMoveState();
    }

    public void ExitLeftBtn()
    {
        leftHeld = false;
        ApplyMoveState();
    }

    public void EnterLeftBtn()
    {
        if (!leftHeld) return;
        ApplyMoveState();
    }
    public void PushRightBtn()
    {
        rightHeld = true;
        ApplyMoveState();
    }
    public void UnPushRightBtn()
    {
        rightHeld = false;
        ApplyMoveState();
    }

    public void ExitRightBtn()
    {
        rightHeld = false;
        ApplyMoveState();
    }
    public void EnterRightBtn()
    {
        if (!rightHeld) return;
        ApplyMoveState();
    }

    void ApplyMoveState()
    {
        if (leftHeld && !rightHeld)
        {
            moveLeft = true;
            moveRight = false;
            sprite.flipX = true;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            anim.SetBool("isRun", true);
            SetButtonAlpha(rightBtn, 0.3f);
            SetButtonAlpha(leftBtn, 1f);
        }
        else if (rightHeld && !leftHeld)
        {
            moveLeft = false;
            moveRight = true;
            sprite.flipX = false;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            anim.SetBool("isRun", true);
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
        anim.SetBool("isRun", false);
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
        Debug.Log($"점프 누름 - coyoteTimeCounter: {coyoteTimeCounter}");
        if (coyoteTimeCounter > 0f && !isJump)
        {
            isJump = true;
            coyoteTimeCounter = 0f;
            anim.SetInteger("isJump", 1);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            PushLeftBtn();
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            UnPushLeftBtn();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            PushRightBtn();
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            UnPushRightBtn();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }
#endif

    void MovementPlayer()
    {
        if (moveLeft && !moveRight)
            horizontalMove = -movespeed;
        else if (moveRight && !moveLeft)
            horizontalMove = movespeed;
        else
            horizontalMove = 0;
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(horizontalMove, rigid.linearVelocity.y);

        // 바닥 감지
        Vector2 boxSize = new Vector2(circleCollider.bounds.size.x * 0.9f, 0.1f);
        Vector2 boxOrigin = new Vector2(circleCollider.bounds.center.x, circleCollider.bounds.min.y);
        float castDistance = 0.1f;

        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, castDistance, layerMask);
        isGround = hit.collider != null;

        // 코요테 타임 갱신
        if (isGround)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        // 착지 버퍼 갱신 — 하강 중이거나 정지 중일 때만 채움 (아래에서 위로 통과 시 오인식 방지)
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

            // 착지 버퍼가 남아있고 상승 중이 아닐 때만 착지로 인정
            if (landingBuffer > 0f && rigid.linearVelocity.y <= 0.1f)
            {
                isJump = false;
                landingBuffer = 0f;
                anim.SetInteger("isJump", 0);

                if (!moveLeft && !moveRight)
                    anim.SetBool("isRun", false);
            }
        }

        Debug.Log($"isGround: {isGround}, isJump: {isJump}, landingBuffer: {landingBuffer}, animParam: {anim.GetInteger("isJump")}, hitCollider: {hit.collider?.name}, velY: {rigid.linearVelocity.y}");
    }
}