using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int speed;
    public int health;
    public int enemyScore;
    public Material[] materials; // materials[0]: 기본, materials[1]: 피격 시
    public GameObject player;

    [Header("Item Drop")]
    public GameObject[] itemPrefabs; // 드롭할 아이템 프리팹들
    [Range(0f, 1f)]
    public float itemDropChance = 0.3f; // 아이템 드롭 확률 (30%)

    protected Renderer objRenderer; // private에서 protected로 변경

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
    }

    // virtual로 만들어서 상속받은 클래스에서 오버라이드 가능하게
    protected virtual void OnHit(int dmg)
    {
        health -= dmg;
        objRenderer.material = materials[1]; // 피격 시 머티리얼 교체
        Invoke("ReturnMaterial", 0.1f);

        if (health <= 0)
        {
            // 아이템 드롭 체크
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

    void DropItem()
    {
        // 확률적으로 아이템 드롭
        if (itemPrefabs.Length > 0 && Random.value < itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject droppedItem = Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);

            // 아이템에 약간의 랜덤한 위치 오프셋 추가 (선택사항)
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            );
            droppedItem.transform.position += randomOffset;
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
