using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int speed;
    public int health;
    public int enemyScore;
    public Material[] materials; // materials[0]: 기본, materials[1]: 피격 시

    public GameObject player;
    private Renderer objRenderer;

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
    }

    void OnHit(int dmg)
    {
        health -= dmg;
        objRenderer.material = materials[1]; // 피격 시 머티리얼 교체
        Invoke("ReturnMaterial", 0.1f);
        if (health <= 0)
        {
            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(enemyScore);
            }

            AudioManager.Instance.MonsterDeadSound();
            Destroy(gameObject);
        }
    }

    void ReturnMaterial()
    {
        objRenderer.material = materials[0]; // 원래 머티리얼로 복귀
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);
            Destroy(other.gameObject);
        }
    }
}
