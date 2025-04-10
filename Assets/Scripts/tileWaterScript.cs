using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileWaterScript : MonoBehaviour
{
    public GameObject waterTilePrefab;
    public int gridSize = 3;
    public float tileSize = 10f;
    public Transform player;
    public float checkThreshold = 20f;

    private Vector3 lastPlayerPosition;
    private List<GameObject> waterTiles;

    private void Start()
    {
        lastPlayerPosition = player.position;
        waterTiles = new List<GameObject>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 spawnPosition = new Vector3(i * tileSize, 0f, j * tileSize);
                GameObject tile = Instantiate(waterTilePrefab, spawnPosition, Quaternion.identity);
                waterTiles.Add(tile);
            }
        }
    }

    private void Update()
    {
        if (Vector3.Distance(player.position, lastPlayerPosition) > checkThreshold)
        {
            lastPlayerPosition = player.position;
            RepositionTiles();
        }
    }

    private void RepositionTiles()
    {
        foreach (GameObject tile in waterTiles)
        {
            Vector3 tilePosition = tile.transform.position;

            if (Vector3.Distance(player.position, tilePosition) > (gridSize * tileSize))
            {
                Vector3 offset = new Vector3(
                    Mathf.Sign(player.position.x - tilePosition.x) * gridSize * tileSize,
                    0f,
                    Mathf.Sign(player.position.z - tilePosition.z) * gridSize * tileSize
                );

                tile.transform.position += offset;
            }
        }
    }
}
