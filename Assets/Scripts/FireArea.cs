using UnityEngine;

public class FireArea : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 러너 게임 이동 속도 (몬스터와 비슷하게)

    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        if (rigid == null)
        {
            rigid = gameObject.AddComponent<Rigidbody>();
        }

        // 중력 끄기 (바닥에 붙어있게)
        rigid.useGravity = false;

        // 뒤쪽으로 이동 (러너 게임 효과)
        rigid.linearVelocity = new Vector3(0, 0, -moveSpeed);
    }

    // 플레이어 데미지는 Player 스크립트의 OnTriggerEnter에서 처리

    private void OnTriggerEnter(Collider other)
    {
        // 경계선에 닿으면 파괴 (몬스터와 동일)
        if (other.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(gameObject);
        }
    }
}
