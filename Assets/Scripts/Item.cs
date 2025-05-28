using UnityEngine;

public class Item : MonoBehaviour
{
    public int speed = 2;
    public ItemType itemType;

    void Start()
    {
        Debug.Log("아이템 생성됨!");

        // 아이템도 몬스터처럼 플레이어 방향으로 이동
        Rigidbody rigid = GetComponent<Rigidbody>();
        if (rigid == null)
        {
            rigid = gameObject.AddComponent<Rigidbody>();
        }

        rigid.useGravity = false;
        rigid.freezeRotation = true;
        rigid.linearVelocity = new Vector3(0, 0, -speed);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"아이템이 {other.gameObject.name} (태그: {other.gameObject.tag})와 충돌!");

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌! 아이템 사용!");
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                UseItem(player);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Player 컴포넌트를 찾을 수 없습니다!");
            }
        }
        else if (other.gameObject.CompareTag("BorderBullet"))
        {
            Debug.Log("아이템이 경계선에 닿아 파괴됨");
            Destroy(gameObject);
        }
        // 불장판과는 충돌하지 않도록 (서로 파괴하지 않음)
        else if (other.gameObject.CompareTag("FireArea"))
        {
            Debug.Log("아이템이 불장판과 충돌했지만 무시함");
            // 아무것도 하지 않음 - 아이템과 불장판이 공존할 수 있도록
        }
    }

    public void UseItem(Player player)
    {
        Debug.Log($"아이템 사용! 타입: {itemType}");
        switch (itemType)
        {
            case ItemType.SkillAttack:
                player.UseSkillAttack();
                break;
        }
    }

    // 아이템이 파괴될 때 로그 출력
    void OnDestroy()
    {
        Debug.Log("아이템 파괴됨");
    }
}

public enum ItemType
{
    SkillAttack
}
