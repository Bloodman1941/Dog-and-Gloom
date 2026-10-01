using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParamteres_", menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandWalkData : ScriptableObject
{
    public int interations = 10, walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
