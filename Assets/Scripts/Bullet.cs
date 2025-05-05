using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(gameObject);
        }
    }
}
