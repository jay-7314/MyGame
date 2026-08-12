using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator anim;
    private bool isAttacking = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isAttacking)
        {
            isAttacking = true;
            int rand = Random.Range(0, 2);
            anim.SetTrigger(rand == 0 ? "Attack" : "Attack1");
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
}