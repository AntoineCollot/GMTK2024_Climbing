using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType { Stone, Wind }
[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{
    [Header("Settings")]
    public BiomeType type;
    public int minFloor = 0;
    public int maxFloor = 0;

    [Space(10)]
    [Header("Segments")]
    [SerializeField] TowerSegment[] towerSegments;

    public bool IsAvailable(int floor)
    {
        return floor >= minFloor && (maxFloor <= 0 || floor <= maxFloor);
    }

    public TowerSegment GetRandomSegment()
    {
        return towerSegments[Random.Range(0, towerSegments.Length)];
    }

    private void OnValidate()
    {
        foreach (var segment in towerSegments)
        {
            if (segment!= null && segment.biome != type)
                Debug.LogError($"Segment {segment.name} registered in biome {type} is set as biome {segment.biome}");
        }
    }
}
