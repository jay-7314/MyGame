using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Collider2D hitboxCollider;
    [SerializeField] int damage = 10;
    [SerializeField] Animator anim;

    private bool isAttacking = false; // 공격 중 중복 입력 방지 (선택)

    private void Awake()
    {
        hitboxCollider.enabled = false;
    }

    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            Attack();
        }
#endif
    }

    // 버튼(OnClick)이나 키보드 입력 양쪽에서 호출 가능한 공용 메서드
    public void Attack()
    {
        if (isAttacking) return; // 공격 중이면 무시 (선택 사항)
        int rand = Random.Range(0, 2);
        anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
    }

    public void SetFacing(bool facingLeft)
    {
        Vector3 scale = hitboxCollider.transform.localScale;
        scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        hitboxCollider.transform.localScale = scale;
    }
}