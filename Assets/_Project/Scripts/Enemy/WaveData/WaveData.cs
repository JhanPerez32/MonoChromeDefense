using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WaveData", menuName = "ScriptableObjects/Combat/WaveData")]
public class WaveData : ScriptableObject
{
    public List<Wave> waves;
}
