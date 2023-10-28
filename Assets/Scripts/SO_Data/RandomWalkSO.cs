using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters_", menuName = "PCG/RandomWalkData")]
public class RandomWalkSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomEachIteration = true;

    public int lakeIterations = 10;
    public int lakeWalkLength = 10;
}
