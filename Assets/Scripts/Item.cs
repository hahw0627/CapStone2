using UnityEngine;

public class Item : MonoBehaviour
{
    public int speed = 5;
    public ItemType itemType;

    void Start()
    {
        // 아이템도 몬스터처럼 플레이어 방향으로 이동
        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.linearVelocity = new Vector3(0, 0, -speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            UseItem(player);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("BorderBullet"))
        {
            // 화면 밖으로 나가면 삭제
            Destroy(gameObject);
        }
    }

    public void UseItem(Player player)
    {
        switch (itemType)
        {
            case ItemType.SkillAttack:
                player.UseSkillAttack();
                break;
        }
    }
}

public enum ItemType
{
    SkillAttack
}
