using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Collider2D hitboxCollider;
    [SerializeField] int damage = 10;
    [SerializeField] Animator anim;
    private bool isAttacking = false;

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

    public void Attack()
    {
        if (isAttacking) return; // 공격 중이면 무시
        isAttacking = true;

        int rand = Random.Range(0, 2);
        anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
    }

    // 공격 애니메이션의 마지막 프레임에 Animation Event로 이 함수를 호출해줘야 함
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    public void SetFacing(bool facingLeft)
    {
        Vector3 scale = hitboxCollider.transform.localScale;
        scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        hitboxCollider.transform.localScale = scale;
    }
}