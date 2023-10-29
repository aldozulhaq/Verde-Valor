using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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
    public GameObject greenLandObject;

    public List<GameObject> treeList = new List<GameObject>();
    public List<GameObject> waterEnviObjs = new List<GameObject>();
    public GameObject well;

    public GameManager gm;

    public void GenerateLevel()
    {
        HashSet<Vector2Int> floorPos = RunRandomWalk();
        HashSet<Vector2Int> greenLandPos = RunRandomWalkGreenland();
        HashSet<Vector2Int> lakePos = RunRandomWalk(floorPos);
        HashSet<Vector2Int> lakeEdges = FindLakeEdgesInDirections(lakePos, Direction2D.cardinalDirList);
        HashSet<Vector2Int> groundEnvi = RunRandomWalkGroundEnvi(floorPos, lakePos);
        HashSet<Vector2Int> waterEnvi = RunRandomWalkGroundEnvi(lakePos);
        floorPos.UnionWith(lakeEdges);

        ClearLevel();
        GenerateGreenLand(greenLandPos);
        GenerateFloors(floorPos, lakePos, greenLandPos);
        GenerateTrees(groundEnvi);
        GenerateLake(lakePos);
        GenerateWaterEnvi(waterEnvi);
        GenerateWell(floorPos, lakePos, groundEnvi);
        GenerateCollider(floorPos, lakePos);
        
        GetComponent<NavMeshSurface>().BuildNavMesh();
        gm.SetHashsets(floorPos, greenLandPos, lakePos);
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

    protected HashSet<Vector2Int> RunRandomWalkGroundEnvi(HashSet<Vector2Int> lakePos)
    {
        HashSet<Vector2Int> groundEnviPos = new HashSet<Vector2Int>();
        for (int i = 0; i < 25; i++)
        {
            var currentPos = lakePos.ElementAt(Random.Range(0, lakePos.Count));
            var path = ProcGenAlgo.SimpleRandomWalk(currentPos, 0);
            groundEnviPos.UnionWith(path);
        }
        return groundEnviPos;
    }

    protected HashSet<Vector2Int> RunRandomWalkGroundEnvi(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> lakePos)
    {
        HashSet<Vector2Int> groundPos = new HashSet<Vector2Int>();

        groundPos.UnionWith(floorPos);
        groundPos.ExceptWith(lakePos);

        HashSet<Vector2Int> groundEnviPos = new HashSet<Vector2Int>();
        for (int i = 0; i < 25; i++)
        {
            var currentPos = groundPos.ElementAt(Random.Range(0, groundPos.Count));
            var path = ProcGenAlgo.SimpleRandomWalk(currentPos, 0);
            groundEnviPos.UnionWith(path);
        }
        return groundEnviPos;
    }

    protected HashSet<Vector2Int> RunRandomWalkGreenland()
    {
        var currentPos = startPos;
        HashSet<Vector2Int> greenLandPos = new HashSet<Vector2Int>();
        for (int i = 0; i < 5; i++)
        {
            var path = ProcGenAlgo.SimpleRandomWalk(currentPos, 5);
            greenLandPos.UnionWith(path);
            currentPos = greenLandPos.ElementAt(Random.Range(0, greenLandPos.Count));
        }
        return greenLandPos;
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
    
    protected void GenerateWell(HashSet<Vector2Int> floor, HashSet<Vector2Int> lakePos, HashSet<Vector2Int> trees)
    {
        HashSet<Vector2Int> pos = new HashSet<Vector2Int>();
        pos.UnionWith(floor);
        pos.ExceptWith(lakePos);
        pos.ExceptWith(trees);

        Vector2 loc = pos.ElementAt(Random.Range(100, 200));
        var wellParent = new GameObject("Well");
        wellParent.transform.parent = transform;
        var wellTile = Instantiate(well, new Vector3(loc.x, 0.6f, loc.y), Quaternion.Euler(new Vector3(90,0,0)));
        wellTile.transform.parent = wellParent.transform;
    }

    protected void GenerateTrees(HashSet<Vector2Int> vector2Ints)
    {
        var treeParent = new GameObject("Trees");
        treeParent.transform.parent = transform;
        foreach (var pos in vector2Ints)
        {
            int randomTreeIndex = Random.Range(0, treeList.Count);
            var tree = Instantiate(treeList[randomTreeIndex], new Vector3(pos.x, treeList[randomTreeIndex].transform.position.y, pos.y), Quaternion.identity);
            tree.transform.parent = treeParent.transform;
        }
    }

    private void GenerateWaterEnvi(HashSet<Vector2Int> waterEnvi)
    {
        var waterEnviParent = new GameObject("WaterEnvi");
        waterEnviParent.transform.parent = transform;

        for (int i = 0; i < 2; i++)
        {
            var tentacleTile = Instantiate(waterEnviObjs[1], new Vector3(waterEnvi.ElementAt(i).x, 0f, waterEnvi.ElementAt(i).y), Quaternion.identity);
            tentacleTile.transform.parent = waterEnviParent.transform;
        }

        foreach (var pos in waterEnvi)
        {
            var lotusTile = Instantiate(waterEnviObjs[0], new Vector3(pos.x, 0.06f, pos.y), Quaternion.Euler(new Vector3(90,Random.Range(0,360),0)));
            lotusTile.transform.parent = waterEnviParent.transform;
        }
    }

    protected void GenerateLake(HashSet<Vector2Int> lakePos)
    {
        var lakeParent = new GameObject("Lake");
        lakeParent.transform.parent = transform;
        foreach (var pos in lakePos)
        {
            var lakeTile = Instantiate(lakeObject, new Vector3(pos.x, -0.45f, pos.y), Quaternion.identity);
            lakeTile.transform.parent = lakeParent.transform;
        }
    }

    protected void GenerateGreenLand(HashSet<Vector2Int> greenLandPos)
    {
        var greenLandParent = new GameObject("GreenLand");
        greenLandParent.transform.parent = transform;
        foreach (var pos in greenLandPos)
        {
            var greenLandTile = Instantiate(greenLandObject, new Vector3(pos.x, -0.4f, pos.y), Quaternion.identity);
            greenLandTile.transform.parent = greenLandParent.transform;
        }
    }

    protected void GenerateFloors(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> lakePos, HashSet<Vector2Int> greenlandPos)
    {
        floorPos.ExceptWith(lakePos);
        var floorParent = new GameObject("Floor");
        floorParent.transform.parent = transform;
        foreach (var pos in floorPos)
        {
            if (greenlandPos.Contains(pos))
            {
                continue;
            }
            var floorTile = Instantiate(floorObject, new Vector3(pos.x, -0.4f, pos.y), Quaternion.identity);
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
