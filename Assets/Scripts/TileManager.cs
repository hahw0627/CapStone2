using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    public GameObject[] tilePrefabs;
    public Dictionary<int, Queue<GameObject>> TilePool;

    public float tileLength = 40.06f; // 타일 길이
    public int numberOfTiles = 5;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        TilePool = new Dictionary<int, Queue<GameObject>>();
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int j = 0; j < 5; j++)
            {
                GameObject go = Instantiate(tilePrefabs[i], transform);
                go.SetActive(false);
                queue.Enqueue(go);
            }
            int key = tilePrefabs[i].GetComponent<Tile>().TileId;
            TilePool.Add(key, queue);
        }
    }

    public GameObject Get(int id)
    {
        return TilePool[id].Dequeue();
    }

    public void Release(int id, GameObject go)
    {
        TilePool[id].Enqueue(go);
    }


    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            int idx = Random.Range(0, tilePrefabs.Length);
            GameObject go = Get(idx);
            go.transform.position = transform.forward * (i * tileLength);

            go.SetActive(true);
        }
    }

    public void SpawnTile()
    {
        int idx = Random.Range(0, tilePrefabs.Length);

        GameObject go = Get(idx);
        go.transform.position = transform.forward * (tileLength * 2);

        go.SetActive(true);
        Debug.Log("SpawnTile");
    }
}
