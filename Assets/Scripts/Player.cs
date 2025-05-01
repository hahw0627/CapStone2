using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public float maxShotDelay;
    public float curShotDelay;

    public GameManager gameManager;

    public GameObject bulletObj;

    // 자동 공격 시, 총알에 가할 힘의 크기
    public float bulletForce = 10f;
    // 자동 공격 시, 탐색 범위 (원하는 경우 사용)
    public float detectionRadius = 15f;

    void Update()
    {
        Move();
        AutoFire();
        Reload();
    }

    void Move()
    {
        // 원래 왼쪽, 오른쪽 입력에 따라 이동하는 기존 코드 유지
        float h = 0;
        float v = Input.GetAxisRaw("Vertical");

        // 예제: 화면 반 나누기 입력 처리 (이전 예제 코드와 같이 수정 가능)
        if (Input.GetMouseButton(0))
        {
            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;
            if (inputPos.x < halfScreen)
            {
                h = -1;
            }
            else if (inputPos.x >= halfScreen)
            {
                h = 1;
            }
        }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
        {
            h = 0;
        }
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void AutoFire()
    {
        // 공격 딜레이가 채워지지 않았다면 발사하지 않음
        if (curShotDelay < maxShotDelay)
        {
            return;
        }

        // 1. 간단하게 씬 내에 "Enemy" 태그가 붙은 오브젝트가 있는지 찾는 방법
        // GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        // if (enemy == null) return;

        // 2. 또는 특정 범위 내에 적이 있는지 확인 (OverlapCircle 사용)
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));
        if (enemyCollider == null)
        {
            return;
        }

        // 적이 존재하면 적 방향으로 총알 발사
        // (여기서는 탐지된 enemyCollider의 위치를 목표로 하여 발사)
        Vector3 enemyPos = enemyCollider.transform.position;
        Vector3 direction = (enemyPos - transform.position).normalized;

        // 총알 인스턴스 생성
        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        // 적 방향으로 힘을 가함
        rigid.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        // 공격 딜레이 초기화
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
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
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            gameManager.RespawnPlayer();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }

    // (옵션) 디버깅용으로 화면에 탐지 영역을 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
