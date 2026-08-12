using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Collider2D hitboxCollider;
    [SerializeField] int damage = 10;
    [SerializeField] Animator anim;

    private void Awake()
    {
        hitboxCollider.enabled = false;
    }

    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
        Debug.Log($"[Frame {Time.frameCount}] EnableHitbox 호출됨");
    }

    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
        Debug.Log($"[Frame {Time.frameCount}] DisableHitbox 호출됨");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Frame {Time.frameCount}] Trigger 발생, hitbox enabled: {hitboxCollider.enabled}, " +
                   $"충돌 오브젝트: {collision.gameObject.name}, InstanceID: {collision.GetInstanceID()}, " +
                   $"부모: {(collision.transform.parent != null ? collision.transform.parent.name : "없음")}");

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"{collision.name}에게 데미지{damage}를 입혔습니다.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int rand = Random.Range(0, 2);
            anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
        }
    }

    public void SetFacing(bool facingLeft)
    {
        // 공격 중(hitbox 켜진 상태)에는 방향 전환으로 인한 콜라이더 재계산을 막음
        if (hitboxCollider.enabled) return;

        if (facingLeft)
        {
            hitboxCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            hitboxCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}