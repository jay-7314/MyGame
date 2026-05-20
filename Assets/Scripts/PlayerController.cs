using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    BoxCollider2D boxcollider;
    public Image leftBtn, rightBtn;
    bool moveLeft;
    bool moveRight;
    bool isGround;
    bool isJump;
    bool isTouched;


    float horizontalMove;
    public LayerMask layerMask;
    public float movespeed;
    public float jumpForce;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
        moveLeft = false;
        moveRight = false;
        isJump = false;
        isTouched = false;
    }

    #region 이동 및 버튼 색상 변경 구현
    public void PushLeftBtn()
    {
        isTouched = true;
        moveRight = false;
        moveLeft = true;
        sprite.flipX = true;
        isTouched = true;
        moveRight = false;
        anim.SetBool("isRun", true);
        SetButtonAlpha(rightBtn, 0.3f);
        SetButtonAlpha(leftBtn, 1f);
    }
    public void UnPushLeftBtn() 
    {
        isTouched = false;
        StopMove();
    }

    public void ExitLeftBtn()
    {
        StopMove();
    }

    public void EnterLeftBtn()
    {
        if (!isTouched) return;
        PushLeftBtn();
    }
    public void PushRightBtn()
    {
        isTouched = true;
        moveLeft = false;
        moveRight = true;
        sprite.flipX = false;
        isTouched = true;
        moveLeft = false;
        SetButtonAlpha(rightBtn, 1f);
        SetButtonAlpha(leftBtn, 0.3f);
    }
    public void UnPushRightBtn() 
    {
       isTouched=false;
        StopMove();
    }

    public void ExitRightBtn()
    {
        StopMove();
    }
    public void EnterRightBtn()
    {
        if (!isTouched) return;
        PushRightBtn();
    }

    void StopMove()
    {
        anim.SetBool("isRun", false);
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
        if (isGround && !isJump)
        {
            isJump = true;
            anim.SetInteger("isJump", 1);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    #endregion
    private void Update()
    {
        MovementPlayer();
    }
    void MovementPlayer()
    {
        if (moveLeft && !moveRight)
        {
            horizontalMove = -movespeed;
        }
        else if (moveRight && !moveLeft)
        {
            horizontalMove = movespeed;
        }
        else
        {
            horizontalMove = 0;
        }
    }
    private void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(horizontalMove, rigid.linearVelocity.y);

        bool wasGround = isGround; // 추가
        isGround = Physics2D.Raycast(transform.position, Vector2.down, boxcollider.bounds.extents.y + 0.1f, layerMask);

        if (isJump) // isJump일 때만 애니메이션 변경
        {
            if (rigid.linearVelocity.y > 0)
            {
                anim.SetInteger("isJump", 1);
            }
            else if (rigid.linearVelocity.y < 0)
            {
                anim.SetInteger("isJump", 2);
            }

            if (!wasGround && isGround) // 공중→착지 순간만 감지
            {
                isJump = false;
                anim.SetInteger("isJump", 0);
            }
        }
    }
}