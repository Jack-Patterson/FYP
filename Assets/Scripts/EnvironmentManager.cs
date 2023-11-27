using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;
    
    [SerializeField] private Renderer ground;
    [SerializeField] private Texture victoryTexture;
    [SerializeField] private Texture failureTexture;

    private void Awake()
    {
        Instance = this;
    }

    internal void AddTexture(bool succeeded)
    {
        ground.material.mainTexture = succeeded ? victoryTexture : failureTexture;
    }
}