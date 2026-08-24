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
        if (isAttacking) return;
        if (!anim.enabled) return; // 대시 중(Animator 꺼짐)에는 공격 무시

        isAttacking = true;
        int rand = Random.Range(0, 2);
        anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        DisableHitbox(); // 안전장치: 혹시 안 꺼져있으면 여기서도 확실히 끔
    }

    // 피격으로 공격이 강제로 끊겼을 때 외부에서 호출해서 플래그를 강제로 풀어줌
    public void ForceCancelAttack()
    {
        isAttacking = false;
        DisableHitbox();
    }

    public void SetFacing(bool facingLeft)
    {
        Vector3 scale = hitboxCollider.transform.localScale;
        scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        hitboxCollider.transform.localScale = scale;
    }
}