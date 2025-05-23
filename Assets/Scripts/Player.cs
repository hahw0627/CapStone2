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

    public float dashDistance = 5f;
    public float doubleClickTime = 0.3f;

    private float lastClickTimeLeft = -1f;
    private float lastClickTimeRight = -1f;
    private bool isDashing = false;
    private int leftTouchCount = 0;


    void Start()
    {
        score = 0;
    }

    void Update()
    {
        Move();
        AutoFire();
        Reload();
        DetectDashInput();
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

    void DetectDashInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;

            if (inputPos.x < halfScreen)
            {
                // 왼쪽 클릭
                if (Time.time - lastClickTimeLeft < doubleClickTime)
                {
                    Dash(Vector3.left);
                    lastClickTimeLeft = -1f; // 리셋
                }
                else
                {
                    lastClickTimeLeft = Time.time;
                }
            }
            else
            {
                // 오른쪽 클릭
                if (Time.time - lastClickTimeRight < doubleClickTime)
                {
                    Dash(Vector3.right);
                    lastClickTimeRight = -1f;
                }
                else
                {
                    lastClickTimeRight = Time.time;
                }
            }
        }
    }

    void Dash(Vector3 direction)
    {
        if (isDashing) return;

        // 대시 거리 + 여유 거리만큼 Ray를 쏴서 벽이 있는지 확인
        float dashCheckDistance = dashDistance + 0.1f;
        Ray ray = new Ray(transform.position, direction);

        // "Border" 레이어만 검사
        if (Physics.Raycast(ray, dashCheckDistance, LayerMask.GetMask("Border")))
        {
            // 벽이 있으므로 대시 취소
            return;
        }

        // 문제 없으면 대시
        isDashing = true;
        transform.position += direction * dashDistance;
        Invoke(nameof(ResetDash), 0.2f);
    }

    void ResetDash()
    {
        isDashing = false;
    }

    void AutoFire()
    {
        if (curShotDelay < maxShotDelay) return;

        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody rigid = bullet.GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * bulletForce, ForceMode.Impulse);

        AudioManager.Instance.PlayAttackSound();

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
                case "Top": 
                    isTouchTop = true; 
                    break;
                case "Bottom": 
                    isTouchBottom = true; 
                    break;
                case "Right":
                    leftTouchCount++;
                    isTouchRight = true; 
                    break;
                case "Left": 
                    leftTouchCount++; 
                    isTouchLeft = true; 
                    break;
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isHit)
            {
                return;
            }
            isHit = true;
            AudioManager.Instance.PlayerHitSound();
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
                case "Right":
                    leftTouchCount--;
                    if (leftTouchCount <= 0)
                    {
                        isTouchRight = false;
                    }  
                    break;
                case "Left":
                    leftTouchCount--;
                    if(leftTouchCount <= 0)
                    {
                        isTouchLeft = false;
                    }
                    break;
                    
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
