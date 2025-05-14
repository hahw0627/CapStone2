using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int TileId;

    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    private void Update()
    {
        if (transform.position.z < -40f)
        {
            gameObject.SetActive(false);
            TileManager.Instance.Release(TileId, gameObject);
            TileManager.Instance.SpawnTile();
        }
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return null;
            transform.position -= Vector3.forward * 0.1f;
        }
    }
}
