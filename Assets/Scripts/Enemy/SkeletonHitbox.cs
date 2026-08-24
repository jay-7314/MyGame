using UnityEngine;
public class SkeletonHitbox : MonoBehaviour
{
    [SerializeField] int damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log($"{collision.name}에게 데미지 {damage}를 입혔습니다.");
            }
        }
    }
}