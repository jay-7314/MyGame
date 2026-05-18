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


    float horizontalMove;
    float prevY;
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
    }

    #region 이동 및 버튼 색상 변경 구현
    public void PushLeftBtn()
    {
        moveLeft = true;
        sprite.flipX = true;
        anim.SetBool("isRun", true);

        SetButtonAlpha(rightBtn, 0.3f);
        SetButtonAlpha(leftBtn, 1f);
    }

    public void UnPushLeftBtn()
    {
        moveLeft = false;
        sprite.flipX = true;
        anim.SetBool("isRun", false);

        SetButtonAlpha(rightBtn, 1f);
        SetButtonAlpha(leftBtn, 1f);
    }

    public void PushRightBtn()
    {
        anim.SetBool("isRun", true);
        moveRight = true;
        SetButtonAlpha(rightBtn, 1f);
        SetButtonAlpha(leftBtn, 0.3f);
        sprite.flipX = false;
    }

    public void UnPushRightBtn()
    {
        anim.SetBool("isRun", false);
        moveRight = false;
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
        if (moveLeft)
        {
            horizontalMove = -movespeed;
        }
        else if (moveRight)
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

        prevY = rigid.linearVelocity.y;
    }
}
