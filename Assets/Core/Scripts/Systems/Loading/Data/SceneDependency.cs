using System;
using Eflatun.SceneReference;
using UnityEngine;

namespace Core.Scripts.Systems.Loading.Data
{
    [Serializable]
    public struct SceneDependency
    {
        [Header("Scene")] 
        [SerializeField] private SceneReference sceneReference;
        
        [Header("Dependencies")]
        [SerializeField] private SceneDependency[] dependencies;
        
        public SceneReference SceneReference => sceneReference;
        public SceneDependency[] Dependencies => dependencies;
    }
}