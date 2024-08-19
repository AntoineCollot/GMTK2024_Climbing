using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] Biome[] biomeSources;
    Dictionary<BiomeType, Biome> biomes;
    List<TowerSegment> spawnedSegments = new List<TowerSegment>();

    //Current
    BiomeType currentBiome;
    int floorsInCurrentBiome;
    int totalFloors;

  

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < biomeSources.Length; i++)
        {
            biomes.Add(biomeSources[i].type, biomeSources[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        GenerateInfinite();
        DisableFarSegments();
    }

    void DisableFarSegments()
    {
        int playerFloor = GameManager.Instance.PlayerCurrentFloor;
        foreach (TowerSegment segment in spawnedSegments)
        {
            //Activate only the nearby floors
            segment.gameObject.SetActive(Mathf.Abs(segment.floor - playerFloor) <= 1);
        }
    }

    void GenerateInfinite()
    {
        if (GameManager.Instance.PlayerCurrentFloor > (totalFloors - 1))
        {
            TowerSegment segment = GetSegmentPossibilities(currentBiome, totalFloors, floorsInCurrentBiome, 1)[0];
            SpawnSegment(segment);
        }
    }

    void SpawnSegment(TowerSegment segment)
    {
        //If stay in biome
        if (segment.biome == currentBiome)
        {
            floorsInCurrentBiome++;
        }
        //if change biome
        else
        {
            currentBiome = segment.biome;
            floorsInCurrentBiome = 1;
        }

        //Instantiate
        TowerSegment newSegment = Instantiate(segment, transform);
        newSegment.transform.position = Tower.GetTowerCenter(totalFloors * Tower.FLOOR_HEIGHT);
        newSegment.Init(totalFloors);
        spawnedSegments.Add(newSegment);

        totalFloors++;
    }

    public List<TowerSegment> GetSegmentPossibilities(BiomeType currentBiomeType, int totalFloors, int floorsInCurrentBiome, int choices = 2)
    {
        List<TowerSegment> segments = new();
        for (int i = 0; i < choices; i++)
        {
            int tryCount = 0;
            TowerSegment segment;
            do
            {
                BiomeType biomeType = GetNextBiome(currentBiomeType, totalFloors, floorsInCurrentBiome);
                segment = biomes[biomeType].GetRandomSegment();
                tryCount++;
                if (tryCount > 10)
                {
                    Debug.LogError("Stuck on 10 tries to find a different atlernative, breaking out");
                    break;
                }
            } while (segments.Contains(segment));

            segments.Add(segment);
        }
        return segments;
    }

    BiomeType GetNextBiome(BiomeType currentBiomeType, int totalFloors, int floorsInCurrentBiome)
    {
        List<BiomeType> availableBiomes = new();
        foreach (var kv in biomes)
        {
            if (kv.Value.IsAvailable(totalFloors) && kv.Key != currentBiomeType)
                availableBiomes.Add(kv.Key);
        }

        float switchBiomeProbability = 0;
        switch (floorsInCurrentBiome)
        {
            case 0:
                switchBiomeProbability = 0;
                break;
            case 1:
                switchBiomeProbability = 0.1f;
                break;
            case 2:
                switchBiomeProbability = 0.2f;
                break;
            case 3:
                switchBiomeProbability = 0.33f;
                break;
            case 4:
                switchBiomeProbability = 0.5f;
                break;
            default:
                switchBiomeProbability = 0.75f;
                break;
        }

        if (Random.Range(0f, 1f) < switchBiomeProbability)
            return availableBiomes[Random.Range(0, availableBiomes.Count)];

        return currentBiomeType;
    }
}
