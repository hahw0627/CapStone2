using UnityEngine;

public class WaterParticle : MonoBehaviour
{
    public int damage = 1;

    private void OnParticleCollision(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // OnHit 호출 (리플렉션 방식 그대로 유지)
                enemy.GetType().GetMethod("OnHit",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.Invoke(enemy, new object[] { damage });

                Debug.Log($"Enemy '{enemy.name}' hit by particle. Damage: {damage}");

                // 파티클 전체 제거 or 선택 제거
                Destroy(gameObject); // 또는 필요시 파티클 시스템에서 개별 제거 로직
            }
        }
        else if (other.CompareTag("BorderBullet"))
        {
            Debug.Log("Particle hit border, destroying particle");
            Destroy(gameObject);
        }
    }
}