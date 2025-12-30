using Eflatun.SceneReference;
using UnityEngine;

namespace Core.Scripts.Systems.Loading.Data
{
    [CreateAssetMenu(fileName = "Scene Config", menuName = "Core/Loading/Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        [Header("Scene Dependencies")]
        [SerializeField] private SceneDependency[] sceneDependencies;
        
        [Header("Persistent Scenes")]
        [SerializeField] private SceneReference[] persistentScenes;
        
        public SceneDependency[] SceneDependencies => sceneDependencies;
        public SceneReference[] PersistentScenes => persistentScenes;
    }
}