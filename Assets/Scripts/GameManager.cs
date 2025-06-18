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


    void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.mainBGM);
    }
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

        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        Vector3 moveDir;

        if (ranPos == 5 || ranPos == 6) // Right Spawn
        {
            enemy.transform.Rotate(Vector3.up * 90);
            moveDir = new Vector3(-1, 0, -1).normalized;
        }
        else if (ranPos == 7 || ranPos == 8) // Left Spawn
        {
            enemy.transform.Rotate(Vector3.up * -90);
            moveDir = new Vector3(1, 0, -1).normalized;
        }
        else // Front Spawn
        {
            moveDir = new Vector3(0, 0, -1);
        }

        enemyLogic.SetMoveDirection(moveDir);
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
        player.transform.position = new Vector3(0, 1.52f, -16.2f);
        player.SetActive(true);
        AudioManager.Instance.PlayerRevivalSound();
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
