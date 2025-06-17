using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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

    [Header("Skill Attack")]
    public float skillRange = 10f;
    public float skillWidth = 8f;
    public int skillDamage = 5;

    [Header("Water Attack")]
    public GameObject waterParticlePrefab;
    public Transform firePoint;
    public float waterDuration = 1f;

    // 머티리얼 관련
    private Renderer playerRenderer;
    public Material defaultMaterial;
    public Material buffMaterial;
    public float materialChangeDuration = 0.1f;
    public Material hitMaterial;
    public float hitEffectDuration = 0.2f;

    private float lastClickTimeLeft = -1f;
    private float lastClickTimeRight = -1f;
    private bool isDashing = false;
    private int leftTouchCount = 0;

    private Coroutine materialCoroutine;
    private enum MaterialState { Default, Hit, Buff }
    private MaterialState currentMaterialState = MaterialState.Default;

    IEnumerator FireWaterParticle()
    {
        // 플레이어 전방에서 약간 앞쪽 위치 + Y축 오프셋
        Vector3 spawnPos = transform.position + transform.forward * 1f;
        spawnPos.z += 2.25f;

        // 전방을 향한 회전값
        Quaternion rotation = Quaternion.LookRotation(transform.forward);

        GameObject particle = Instantiate(waterParticlePrefab, spawnPos, rotation);

        // 파티클 설정 (루프 X)
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.loop = false;
        }

        AudioManager.Instance.PlayAttackSound();

        Destroy(particle, waterDuration);
        yield return null;
    }

    void Start()
    {
        score = 0;
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            defaultMaterial = playerRenderer.sharedMaterial;
        }
    }

    void Update()
    {
        Move();
        Reload();
        DetectDashInput();
        HandleWaterAttack();
    }

    void Move()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }


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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;

            if (inputPos.x < halfScreen)
            {
                // 왼쪽 클릭
                if (Time.time - lastClickTimeLeft < doubleClickTime)
                {
                    Dash(Vector3.left);
                    lastClickTimeLeft = -1f;
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

    void HandleWaterAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Space 키 입력 (변경 가능)
        {
            StartCoroutine(FireWaterParticle());
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void UseSkillAttack()
    {
        // 플레이어 전방 범위에 있는 모든 적에게 데미지
        Vector3 skillCenter = transform.position + transform.forward * (skillRange / 2f);

        // 스킬 이펙트 생성 (선택사항)
        /* if (skillEffectPrefab != null)
        {
            GameObject effect = Instantiate(skillEffectPrefab, skillCenter, transform.rotation);
            Destroy(effect, 2f); // 2초 후 이펙트 삭제
        }*/

        // 스킬 범위 내의 모든 적 탐지
        Collider[] enemiesInRange = Physics.OverlapBox(
            skillCenter,
            new Vector3(skillWidth / 2f, 2f, skillRange / 2f),
            transform.rotation,
            LayerMask.GetMask("Enemy") // Enemy 레이어에 있는 오브젝트만 탐지
        );

        // 탐지된 적들에게 데미지 적용
        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Enemy 클래스의 OnHit 메서드를 호출하기 위해 리플렉션 사용
                enemy.GetType().GetMethod("OnHit",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.Invoke(enemy, new object[] { skillDamage });
            }
        }

        // 스킬 사용 사운드 재생 (선택사항)
       // AudioManager.Instance.PlayAttackSound(); // 또는 별도의 스킬 사운드

        Debug.Log($"스킬 공격! {enemiesInRange.Length}명의 적에게 {skillDamage} 데미지!");
    }

    IEnumerator DieAfterDelay()
    {
        yield return new WaitForSeconds(hitEffectDuration); // 머티리얼 효과 볼 시간

        SetMaterial(defaultMaterial); // 복구
        gameManager.GameOver();
        gameObject.SetActive(false);
    }

    IEnumerator RespawnAfterHit()
    {
        yield return new WaitForSeconds(hitEffectDuration); // 피격 연출

        SetMaterial(defaultMaterial); // 복구
        gameManager.RespawnPlayer();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top": isTouchTop = true; break;
                case "Bottom": isTouchBottom = true; break;
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
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("FireArea"))
        {
            if (isHit) return;

            isHit = true;
            AudioManager.Instance.PlayerHitSound();
            TriggerHitEffect(); // 빨간색으로 변경

            life--;
            gameManager.UpdateLifeIcon(life);

            if (life == 0)
            {
                StartCoroutine(DieAfterDelay()); // 잠깐 대기 후 죽음 처리
            }
            else
            {
                StartCoroutine(RespawnAfterHit());
            }

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("FireArea")) // 불장판 처리 추가
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
            // 불장판은 파괴하지 않음 (시간이 지나면 자동으로 사라짐)
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("플레이어가 아이템을 획득했습니다!");
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                item.UseItem(this);
                ChangeMaterialTemporarily();
                Destroy(collision.gameObject);
            }
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
                    if (leftTouchCount <= 0) isTouchRight = false;
                    break;
                case "Left":
                    leftTouchCount--;
                    if (leftTouchCount <= 0) isTouchLeft = false;
                    break;
            }
        }
    }
    IEnumerator FlashHitMaterial()
    {
        SetMaterial(hitMaterial);
        currentMaterialState = MaterialState.Hit;

        yield return new WaitForSeconds(hitEffectDuration);

        if (currentMaterialState == MaterialState.Hit)
        {
            SetMaterial(defaultMaterial);
            currentMaterialState = MaterialState.Default;
        }

        materialCoroutine = null;
    }

    void SetMaterial(Material mat)
    {
        if (playerRenderer != null && mat != null)
        {
            playerRenderer.material = mat;
        }
    }

    void ChangeMaterialTemporarily()
    {
        if (materialCoroutine != null)
            StopCoroutine(materialCoroutine);

        SetMaterial(buffMaterial);
        currentMaterialState = MaterialState.Buff;

        Invoke(nameof(ReturnToDefaultMaterial), materialChangeDuration);
    }

    void ReturnToDefaultMaterial()
    {
        if (currentMaterialState == MaterialState.Buff)
        {
            SetMaterial(defaultMaterial);
            currentMaterialState = MaterialState.Default;
        }
    }

    void TriggerHitEffect()
    {
        if (materialCoroutine != null)
            StopCoroutine(materialCoroutine);

        materialCoroutine = StartCoroutine(FlashHitMaterial());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 스킬 범위 시각화
        Gizmos.color = Color.blue;
        Vector3 skillCenter = transform.position + transform.forward * (skillRange / 2f);
        Gizmos.DrawWireCube(skillCenter, new Vector3(skillWidth, 4f, skillRange));
    }
}