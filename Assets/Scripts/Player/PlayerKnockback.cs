using UnityEngine;

// 이 스크립트는 반드시 실제 콜라이더/Rigidbody2D가 붙어있는
// 플레이어 캐릭터 오브젝트(Aren(Clone))에 직접 붙여야 합니다.
public class PlayerKnockback : MonoBehaviour
{
    Rigidbody2D rigid;

    [Header("Knockback")]
    [SerializeField] string monsterTag = "Enemy";
    [SerializeField] float knockbackDistance = 1.2f;      // 좌우로 밀려나는 거리 기준
    [SerializeField] float knockbackDuration = 0.12f;      // 넉백 중 조작 불가 시간

    [Header("Knockback - 위에서 부딪혔을 때")]
    [SerializeField] float topXThreshold = 0.3f;               // 이 값보다 X좌표 차이가 작으면 "머리 위"로 판정
    [SerializeField] float topPopForce = 4f;                       // 위로 튕겨내는 힘

    public bool IsKnockback { get; private set; }
    float knockbackTimer;
    float knockbackSpeed;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        knockbackSpeed = knockbackDistance / knockbackDuration;
    }

    void FixedUpdate()
    {
        if (!IsKnockback) return;

        knockbackTimer -= Time.fixedDeltaTime;
        if (knockbackTimer <= 0f)
        {
            IsKnockback = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryKnockback(collision);
    }

    void TryKnockback(Collision2D collision)
    {
        if (IsKnockback) return;
        if (!collision.collider.CompareTag(monsterTag)) return;

        float xDiff = transform.position.x - collision.transform.position.x;

        IsKnockback = true;
        knockbackTimer = knockbackDuration;

        // 몬스터 머리 위쪽(X좌표 차이가 거의 없음)에서 부딪힌 경우엔
        // 좌우로 미는 대신 살짝 위로 튕겨서 자연스럽게 떨어지게 함
        if (Mathf.Abs(xDiff) < topXThreshold)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, topPopForce);
        }
        else
        {
            float dir = xDiff > 0f ? 1f : -1f;
            rigid.linearVelocity = new Vector2(dir * knockbackSpeed, rigid.linearVelocity.y);
        }
    }
}