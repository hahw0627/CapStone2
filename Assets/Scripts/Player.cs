using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public float speed = 50f;
    public bool isTouchRight, isTouchLeft;

    public int life, score;

    public float maxShotDelay, curShotDelay;

    public GameManager gameManager;

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

    private Renderer playerRenderer;
    public Material defaultMaterial, buffMaterial, hitMaterial;
    public float materialChangeDuration = 0.1f;
    public float hitEffectDuration = 0.2f;

    private float lastClickTimeLeft = -1f, lastClickTimeRight = -1f;
    private bool isDashing = false;

    private Rigidbody rb;
    private Vector3 inputDirection;

    private Coroutine materialCoroutine;
    private enum MaterialState { Default, Hit, Buff }
    private MaterialState currentMaterialState = MaterialState.Default;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        score = 0;
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
            defaultMaterial = playerRenderer.sharedMaterial;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        StartCoroutine(AutoFireWater());
    }

    void OnEnable()
    {
        StartCoroutine(AutoFireWater());
    }

    void Update()
    {
        GetInput();
        Reload();
        DetectDashInput();
    }

    void FixedUpdate()
    {
        Move();
        LockPosition();
        CheckAndPreventWallOverlap();
    }

    void GetInput()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            inputDirection = Vector3.zero;
            return;
        }

        float h = 0;

        if (Input.GetMouseButton(0))
        {
            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;
            h = inputPos.x < halfScreen ? -1 : 1;
        }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) h = 0;

        inputDirection = new Vector3(h, 0, 0f).normalized;
    }

    void Move()
    {
        if (!isDashing && inputDirection.sqrMagnitude > 0f)
        {
            Vector3 move = inputDirection * speed * Time.fixedDeltaTime;
            transform.position += new Vector3(move.x, 0f, 0f);
        }
    }

    void LockPosition()
    {
        Vector3 pos = transform.position;
        pos.y = fixedY;
        pos.z = fixedZ;
        transform.position = pos;
    }

    void CheckAndPreventWallOverlap()
    {
        float buffer = 0.05f;
        Vector3 pos = transform.position;

        Collider[] overlaps = Physics.OverlapBox(
            pos,
            new Vector3(0.5f, 1f, 0.5f),
            Quaternion.identity,
            LayerMask.GetMask("Border")
        );

        foreach (Collider col in overlaps)
        {
            Vector3 dir = pos - col.ClosestPoint(pos);
            dir.y = 0;
            dir.z = 0;

            if (dir.sqrMagnitude > 0.0001f)
                transform.position += dir.normalized * buffer;
        }
    }

    void DetectDashInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 inputPos = Input.mousePosition;
            float halfScreen = Screen.width / 2f;

            if (inputPos.x < halfScreen)
            {
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

        float dashCheckDistance = dashDistance + 0.1f;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, dashCheckDistance, LayerMask.GetMask("Border")))
            return;

        isDashing = true;
        transform.position += direction * dashDistance;
        Invoke(nameof(ResetDash), 0.2f);

        CheckAndPreventWallOverlap();
    }

    void ResetDash()
    {
        isDashing = false;
    }

    IEnumerator AutoFireWater()
    {
        while (true)
        {
            yield return new WaitForSeconds(waterDuration);
            StartCoroutine(FireWaterParticle());
        }
    }

    IEnumerator FireWaterParticle()
    {
        Vector3 spawnPos = transform.position + Vector3.forward * 2.25f;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
        GameObject particle = Instantiate(waterParticlePrefab, spawnPos, rotation);

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

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void UseSkillAttack()
    {
        Vector3 skillCenter = transform.position + Vector3.forward * (skillRange / 2f);

        Collider[] enemiesInRange = Physics.OverlapBox(
            skillCenter,
            new Vector3(skillWidth / 2f, 2f, skillRange / 2f),
            Quaternion.identity,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetType().GetMethod("OnHit",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.Invoke(enemy, new object[] { skillDamage });
            }
        }

        Debug.Log($"스킬 공격! {enemiesInRange.Length}명의 적에게 {skillDamage} 데미지!");
    }

    private void OnTriggerEnter(Collider other) => HandleCollisionOrTrigger(other);

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            switch (other.name)
            {
                case "Right": isTouchRight = false; break;
                case "Left": isTouchLeft = false; break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) => HandleCollisionOrTrigger(collision.collider);

    private void HandleCollisionOrTrigger(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            switch (other.name)
            {
                case "Right": isTouchRight = true; break;
                case "Left": isTouchLeft = true; break;
            }
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("FireArea"))
        {
            if (isHit) return;

            isHit = true;
            AudioManager.Instance.PlayerHitSound();
            TriggerHitEffect();

            life--;
            gameManager.UpdateLifeIcon(life);

            if (life == 0)
                StartCoroutine(DieAfterDelay());
            else
                StartCoroutine(RespawnAfterHit());

            if (other.CompareTag("Enemy"))
                Destroy(other.gameObject);
        }
        else if (other.CompareTag("Item"))
        {
            Debug.Log("플레이어가 아이템을 획득했습니다!");
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                item.UseItem(this);
                ChangeMaterialTemporarily();
            }

            Destroy(other.gameObject);
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
            playerRenderer.material = mat;
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

    IEnumerator DieAfterDelay()
    {
        yield return new WaitForSeconds(hitEffectDuration);
        SetMaterial(defaultMaterial);
        gameManager.GameOver();
        gameObject.SetActive(false);
    }

    IEnumerator RespawnAfterHit()
    {
        yield return new WaitForSeconds(hitEffectDuration);
        SetMaterial(defaultMaterial);
        gameManager.RespawnPlayer();
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Vector3 skillCenter = transform.position + Vector3.forward * (skillRange / 2f);
        Gizmos.DrawWireCube(skillCenter, new Vector3(skillWidth, 4f, skillRange));
    }
}