using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelGen lg;

    HashSet<Vector2Int> corruptedFloor = new HashSet<Vector2Int>();
    HashSet<Vector2Int> GreenLand = new HashSet<Vector2Int>();
    HashSet<Vector2Int> trueCorruptfloor = new HashSet<Vector2Int>();

    public float corruptLandAmount;

    public GameObject[] landEnemy;
    public GameObject[] lakeEnemy;

    public List<GameObject> gaming = new List<GameObject>();
    public List<GameObject> menuObjs = new List<GameObject>();
    public List<GameObject> gameoverObjs = new List<GameObject>();

    public GameObject player;

    public GameObject greenlandObject;
    public GameObject greenlandParent;

    void Start()
    {
        lg.GenerateLevel();
        corruptLandAmount = trueCorruptfloor.Count;
        //find greendland parent by name
        greenlandParent = GameObject.Find("GreenLand");
    }

    public void PlayAgain()
    {
        //destroy everything in enemy parent
        foreach (Transform child in GameObject.Find("EnemyParent").transform)
        {
            Destroy(child.gameObject);
        }

        Start();
        StartGame();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        foreach (GameObject obj in gameoverObjs)
        {
            obj.SetActive(true);
        }
        player.GetComponent<PlayerManager>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;

        foreach (GameObject obj in gaming)
        {
            obj.SetActive(false);
        }
    }

    public void StartGame()
    {
        FindObjectOfType<AudioManager>().PlayBGMCombat();

        //create enemy parent
        GameObject enemyParent = new GameObject("EnemyParent");

        //spawn 10 enemy at random on corrupted floor
        for (int i = 0; i < 8; i++)
        {
            int randomIndex = Random.Range(0, trueCorruptfloor.Count);
            Vector2Int randomPos = trueCorruptfloor.ElementAt(randomIndex);
            Vector3 spawnPos = new Vector3(randomPos.x, 0, randomPos.y);
            Instantiate(landEnemy[Random.Range(0, landEnemy.Length)], spawnPos, Quaternion.identity, enemyParent.transform);
        }

        foreach (GameObject obj in menuObjs)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in gaming)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in gameoverObjs)
        {
            obj.SetActive(false);
        }

        player.GetComponent<PlayerManager>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;

    }

    public void SetHashsets(HashSet<Vector2Int> floor, HashSet<Vector2Int> greenland, HashSet<Vector2Int> lake)
    {
        corruptedFloor = floor;
        GreenLand = greenland;

        trueCorruptfloor = corruptedFloor.Except(GreenLand).ToHashSet();
    }

    public void ReplaceCorruptWithGreenland(int amount)
    {
        if (amount > trueCorruptfloor.Count)
            amount = trueCorruptfloor.Count;

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, trueCorruptfloor.Count);
            Vector2Int randomPos = trueCorruptfloor.ElementAt(randomIndex);
            Vector3 spawnPos = new Vector3(randomPos.x, -0.4f, randomPos.y);
            //delete corrupted floor through hashset to replace with greenland
            //find the object with the same position as the randomPos
            foreach (Transform child in GameObject.Find("Floor").transform)
            {
                if (child.position == spawnPos)
                {
                    Destroy(child.gameObject);
                }
            }
            
            Instantiate(greenlandObject, spawnPos, Quaternion.identity, greenlandParent.transform);
            trueCorruptfloor.Remove(randomPos);
            GreenLand.Add(randomPos);
        }
    }
}
