using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGen : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    [SerializeField]
    private RandomWalkSO randomWalkParam;

    public GameObject floorObject;
    public GameObject colliderObject;
    public GameObject lakeObject;


    public void GenerateLevel()
    {
        HashSet<Vector2Int> floorPos = RunRandomWalk();
        HashSet<Vector2Int> lakePos = RunRandomWalk(floorPos);
        HashSet<Vector2Int> lakeEdges = FindLakeEdgesInDirections(lakePos, Direction2D.cardinalDirList);
        floorPos.UnionWith(lakeEdges);

        ClearLevel();
        GenerateFloors(floorPos, lakePos);
        GenerateLake(lakePos);
        GenerateCollider(floorPos, lakePos);
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPos = startPos;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkParam.iterations; i++)
        {
            var path = ProcGenAlgo.SimpleRandomWalk(currentPos, randomWalkParam.walkLength);
            floorPos.UnionWith(path);
            if (randomWalkParam.startRandomEachIteration)
            {
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
            }
        }
        return floorPos;
    }

    protected HashSet<Vector2Int> RunRandomWalk(HashSet<Vector2Int> floorPos)
    {
        var currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
        HashSet<Vector2Int> lakePos = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkParam.lakeIterations; i++)
        {
            var path = ProcGenAlgo.SimpleRandomWalk(currentPos, randomWalkParam.lakeWalkLength);
            lakePos.UnionWith(path);
            if (randomWalkParam.startRandomEachIteration)
            {
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
            }
        }
        return lakePos;
    }
    
    protected void GenerateLake(HashSet<Vector2Int> lakePos)
    {
        var lakeParent = new GameObject("Lake");
        lakeParent.transform.parent = transform;
        foreach (var pos in lakePos)
        {
            var lakeTile = Instantiate(lakeObject, new Vector3(pos.x, -0.4f, pos.y), Quaternion.identity);
            lakeTile.transform.parent = lakeParent.transform;
        }
    }

    protected void GenerateFloors(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> lakePos)
    {
        floorPos.ExceptWith(lakePos);
        var floorParent = new GameObject("Floor");
        floorParent.transform.parent = transform;
        foreach (var pos in floorPos)
        {
            var floorTile = Instantiate(floorObject, new Vector3(pos.x, -0.35f, pos.y), Quaternion.identity);
            floorTile.transform.parent = floorParent.transform;
        }
    }

    protected void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenerateCollider(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> lakePos)
    {
        var ColliderPos = FindColliderInDirections(floorPos, lakePos, Direction2D.cardinalDirList);
        var colliderParent = new GameObject("Collider");
        colliderParent.transform.parent = transform;
        foreach (var pos in ColliderPos)
        {
            var colliderTile = Instantiate(colliderObject, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
            colliderTile.transform.parent = colliderParent.transform;
        }
    }

    //find lake edges
    private static HashSet<Vector2Int> FindLakeEdgesInDirections(HashSet<Vector2Int> lakePos, List<Vector2Int> dirList)
    {
        HashSet<Vector2Int> lakeEdgesPos = new HashSet<Vector2Int>();
        foreach (var pos in lakePos)
        {
            foreach (var dir in dirList)
            {
                var neighboursPos = pos + dir;
                if(lakePos.Contains(neighboursPos) == false)
                {
                    lakeEdgesPos.Add(neighboursPos);
                }
            }
        }
        return lakeEdgesPos;
    }

    private static HashSet<Vector2Int> FindColliderInDirections(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> lakePos, List<Vector2Int> dirList)
    {
        floorPos.UnionWith(lakePos);
        HashSet<Vector2Int> colliderPos = new HashSet<Vector2Int>();
        foreach (var pos in floorPos)
        {
            foreach (var dir in dirList)
            {
                var neighboursPos = pos + dir;
                if(floorPos.Contains(neighboursPos) == false)
                {
                    colliderPos.Add(neighboursPos);
                }
            }
        }
        return colliderPos;
    }
}
