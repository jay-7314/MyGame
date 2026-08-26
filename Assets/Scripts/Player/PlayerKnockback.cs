using UnityEngine;
public class PlayerKnockback : MonoBehaviour
{
    Rigidbody2D rigid;

    [Header("Knockback")]
    [SerializeField] string monsterTag = "Enemy";
    [SerializeField] float knockbackDistance = 1.2f;      // 좌우로 밀려나는 거리 기준
    [SerializeField] float knockbackDuration = 0.12f;      // 넉백 중 조작 불가 시간

    [Header("Knockback - 위에서 부딪혔을 때")]
    [SerializeField] float topXThreshold = 0.3f;               // 이 값보다 X좌표 차이가 작으면 "머리 위" 후보로 판정
    [SerializeField] float topYThreshold = 0.1f;               // 플레이어가 몬스터보다 이만큼 이상 위에 있어야 "머리 위"로 인정
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
        float yDiff = transform.position.y - collision.transform.position.y;

        IsKnockback = true;
        knockbackTimer = knockbackDuration;

        if (Mathf.Abs(xDiff) < topXThreshold && yDiff > topYThreshold)
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