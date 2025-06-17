using UnityEngine;

public class WaterParticle : MonoBehaviour
{
    public int damage = 1;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collided with: " + other.name + " Tag: " + other.tag);

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit!");
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetType().GetMethod("OnHit",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.Invoke(enemy, new object[] { damage });

                Debug.Log("Enemy damage applied, destroying particle");
                Destroy(gameObject); // 전체 파티클 오브젝트 제거
            }
        }

        if (other.CompareTag("BorderBullet"))
        {
            Debug.Log("Particle hit border, destroying particle");
            Destroy(gameObject);
        }
    }
}
