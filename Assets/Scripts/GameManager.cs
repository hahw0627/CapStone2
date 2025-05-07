using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPos;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreTxt;
    public Image[] lifeImage;
    public GameObject gameOverSet;

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        Player playerLogic = player.GetComponent<Player>();
        scoreTxt.text = string.Format("{0:n0}", playerLogic.score);
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

    public void UpdateLifeIcon(int life)
    {
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }
        for (int index=0; index<life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = new Vector3(0, 1, -2.72f);
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene("DevScene");
    }

    public void AddScore(int amount)
    {
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.score += amount;
        scoreTxt.text = string.Format("{0:n0}", playerLogic.score);
    }
}
