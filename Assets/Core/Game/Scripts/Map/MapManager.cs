using System;
using Core.Game.Scripts.Map;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    
    private MapGenerator _mapGenerator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mapGenerator = GetComponent<MapGenerator>();
    }

    private int i = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (i++ % 100 == 0)
        {
            float f = Time.realtimeSinceStartup;
            _mapGenerator.GenerateMap();
            Debug.Log((Time.realtimeSinceStartup - f).ToString("F3") + "s");
        }
    }
}
