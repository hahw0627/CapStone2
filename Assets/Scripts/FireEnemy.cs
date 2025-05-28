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
            // 불장판 생성 (아이템 드롭보다 먼저)
            CreateFireArea();

            // 아이템 드롭 (불장판과 다른 위치에)
            DropItem();

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

    // 아이템 드롭을 오버라이드해서 불장판과 다른 위치에 생성
    void DropItem()
    {
        // 확률적으로 아이템 드롭
        if (itemPrefabs.Length > 0 && Random.value < itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);

            // 불장판과 겹치지 않도록 위치 조정
            Vector3 itemPosition = transform.position;
            //itemPosition.y += 1f; // 불장판보다 위에 생성

            // 좌우로 약간 오프셋 추가
            itemPosition.x += Random.Range(-1f, 1f);

            GameObject droppedItem = Instantiate(itemPrefabs[randomIndex], itemPosition, Quaternion.identity);

            // 아이템이 불장판과 충돌하지 않도록 레이어 설정이나 태그 확인
            Debug.Log($"FireEnemy: 아이템 드롭! 위치: {itemPosition}");
        }
    }
}
