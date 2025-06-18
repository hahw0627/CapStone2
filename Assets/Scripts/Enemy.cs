using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 20f;
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
    private Rigidbody rb;

    private Vector3 moveDirection = Vector3.back;
    private bool isDead = false;

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        if (rb != null && !isDead)
        {
            Vector3 movement = moveDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    protected virtual void OnHit(int dmg)
    {
        if (isDead) return;
        health -= dmg;
        objRenderer.material = materials[1];
        Invoke("ReturnMaterial", 0.1f);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

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
        if (!isDead)
        {
            objRenderer.material = materials[0];
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("BorderBullet"))
        {
            Die();
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                OnHit(bullet.dmg);
            }
            Destroy(other.gameObject);
        }
    }
}