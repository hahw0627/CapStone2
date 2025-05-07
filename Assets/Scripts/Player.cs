using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;

    public float maxShotDelay;
    public float curShotDelay;

    public GameManager gameManager;

    public GameObject bulletObj;

    public float bulletForce = 10f;
    public float detectionRadius = 15f;
    public bool isHit;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        Move();
        AutoFire();
        Reload();
    }

    void Move()
    {
        float h = 0;
        float v = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(0))
        {
            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;
            if (inputPos.x < halfScreen)
                h = -1;
            else
                h = 1;
        }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) v = 0;

        Vector3 nextPos = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.position += nextPos;
    }

    void AutoFire()
    {
        if (curShotDelay < maxShotDelay) return;

        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));
        if (enemyColliders.Length == 0) return;

        Vector3 enemyPos = enemyColliders[0].transform.position;
        Vector3 direction = (enemyPos - transform.position).normalized;

        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody rigid = bullet.GetComponent<Rigidbody>();
        rigid.AddForce(direction * bulletForce, ForceMode.Impulse);

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top": isTouchTop = true; break;
                case "Bottom": isTouchBottom = true; break;
                case "Right": isTouchRight = true; break;
                case "Left": isTouchLeft = true; break;
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isHit)
            {
                return;
            }
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);

            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top": isTouchTop = false; break;
                case "Bottom": isTouchBottom = false; break;
                case "Right": isTouchRight = false; break;
                case "Left": isTouchLeft = false; break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
