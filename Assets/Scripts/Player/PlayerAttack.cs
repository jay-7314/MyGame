using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator anim;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            anim.SetTrigger("Attack");
        }
    }
}
