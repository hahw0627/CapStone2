using UnityEngine;

public class FireArea : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 러너 게임 이동 속도 (몬스터와 비슷하게)
    private Rigidbody rigid;

    [Header("Fire Effect")]
    public GameObject fireEffectPrefab; // 불이펙트 프리팹
    private GameObject fireEffectInstance; // 인스턴스화된 이펙트

    void Start()
    {
        Debug.Log("불장판 생성됨!");

        rigid = GetComponent<Rigidbody>();
        if (rigid == null)
        {
            rigid = gameObject.AddComponent<Rigidbody>();
        }

        // 중력 끄기 (바닥에 붙어있게)
        rigid.useGravity = false;

        // 회전 고정 (불장판이 회전하지 않도록)
        rigid.freezeRotation = true;

        // 뒤쪽으로 이동 (러너 게임 효과)
        rigid.linearVelocity = new Vector3(0, 0, -moveSpeed);

        // 이펙트 생성 및 재생
        if (fireEffectPrefab != null)
        {
            // 이펙트를 현재 위치에 생성하고 불장판을 따라가도록 부모로 설정
            fireEffectInstance = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
            fireEffectInstance.transform.SetParent(transform); // 부모-자식 관계 설정

            fireEffectInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            fireEffectInstance.transform.localPosition = new Vector3(0f, -0.004f, 0f);

            // 파티클 재생
            var ps = fireEffectInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
    }

    // 플레이어 데미지는 Player 스크립트의 OnTriggerEnter에서 처리
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"불장판이 {other.gameObject.name} (태그: {other.gameObject.tag})와 충돌!");

        // 경계선에 닿으면 파괴 (몬스터와 동일)
        if (other.gameObject.CompareTag("BorderBullet"))
        {
            Debug.Log("불장판이 경계선에 닿아 파괴됨");
            Destroy(gameObject);
        }
        // 아이템과는 충돌하지 않도록 (아이템을 파괴하지 않음)
        else if (other.gameObject.CompareTag("Item"))
        {
            Debug.Log("불장판이 아이템과 충돌했지만 무시함");
            // 아무것도 하지 않음 - 아이템과 불장판이 공존할 수 있도록
        }
    }

    // 불장판이 파괴될 때 로그 출력 및 이펙트도 제거
    void OnDestroy()
    {
        Debug.Log("불장판 파괴됨");

        // 생성된 이펙트도 함께 제거
        if (fireEffectInstance != null)
        {
            Destroy(fireEffectInstance);
        }
    }
}