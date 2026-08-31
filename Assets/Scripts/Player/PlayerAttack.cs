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
        if (!anim.enabled) return; 

        isAttacking = true;
        int rand = Random.Range(0, 2);
        anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        DisableHitbox();
    }

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