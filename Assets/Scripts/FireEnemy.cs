using UnityEngine;

public class FireEnemy : Enemy
{
    [Header("Fire Area Settings")]
    public GameObject fireAreaPrefab; // 불장판 프리팹
    public float fireAreaDuration = 5f; // 불장판 지속시간 (옵션)

    // Enemy의 OnHit을 오버라이드해서 죽을 때 불장판 생성
    protected override void OnHit(int dmg)
    {
        health -= dmg;
        objRenderer.material = materials[1]; // 피격 시 머티리얼 교체
        Invoke("ReturnMaterial", 0.1f);

        if (health <= 0)
        {
            // 불장판 생성
            CreateFireArea();

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(enemyScore);
            }
            AudioManager.Instance.MonsterDeadSound();
            Destroy(gameObject);
        }
    }

    void CreateFireArea()
    {
        if (fireAreaPrefab != null)
        {
            Vector3 firePosition = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
            GameObject fireArea = Instantiate(fireAreaPrefab, firePosition, Quaternion.identity);

            // 불장판 지속시간 설정
            if (fireAreaDuration > 0)
            {
                Destroy(fireArea, fireAreaDuration);
            }
        }
    }
}
