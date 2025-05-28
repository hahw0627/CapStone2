using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int speed;
    public int health;
    public int enemyScore;
    public Material[] materials;
    public GameObject player;

    [Header("Item Drop")]
    public GameObject[] itemPrefabs;
    [Range(0f, 1f)]
    public float itemDropChance = 0.3f;

    [Header("Effect")]
    public GameObject deathEffectPrefab;

    protected Renderer objRenderer;

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
    }

    protected virtual void OnHit(int dmg)
    {
        health -= dmg;
        objRenderer.material = materials[1];
        Invoke("ReturnMaterial", 0.1f);

        if (health <= 0)
        {
            PlayDeathEffect();

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

    void PlayDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void DropItem()
    {
        if (itemPrefabs.Length > 0 && Random.value < itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject droppedItem = Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            droppedItem.transform.position += randomOffset;
        }
    }

    void ReturnMaterial()
    {
        objRenderer.material = materials[0];
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