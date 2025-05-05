using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPos;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, enemyObjs.Length);
        int ranPos = Random.Range(0, spawnPos.Length);

        GameObject enemy = Instantiate(
            enemyObjs[ranEnemy],
            spawnPos[ranPos].position,
            spawnPos[ranPos].rotation
        );

        Rigidbody rigid = enemy.GetComponent<Rigidbody>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        if (ranPos == 5 || ranPos == 6) // Right Spawn
        {
            enemy.transform.Rotate(Vector3.up * 90);
            rigid.linearVelocity = new Vector3(-enemyLogic.speed, 0, -1);
        }
        else if (ranPos == 7 || ranPos == 8) // Left Spawn
        {
            enemy.transform.Rotate(Vector3.up * -90);
            rigid.linearVelocity = new Vector3(enemyLogic.speed, 0, -1);
        }
        else // Front Spawn
        {
            rigid.linearVelocity = new Vector3(0, 0, -enemyLogic.speed);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = new Vector3(0, 0, -3.5f);
        player.SetActive(true);
    }
}
