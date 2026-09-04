using UnityEngine;
public class PlayerKnockback : MonoBehaviour
{
    Rigidbody2D rigid;

    [Header("Knockback")]
    [SerializeField] string monsterTag = "Enemy";
    [SerializeField] float knockbackDistance = 1.2f;      
    [SerializeField] float knockbackDuration = 0.12f;     

    [Header("Knockback - 위에서 부딪혔을 때")]
    [SerializeField] float topXThreshold = 0.3f;              
    [SerializeField] float topYThreshold = 0.1f;              
    [SerializeField] float topPopForce = 4f;                     

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