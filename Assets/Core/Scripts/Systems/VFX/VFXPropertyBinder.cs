using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.VFX
{
    /// <summary>
    /// Defines what type of particle system property to bind.
    /// </summary>
    public enum VFXPropertyType
    {
        /// <summary>Sets a float property on the material via MaterialPropertyBlock.</summary>
        MaterialFloat,
        /// <summary>Sets a vector property on the material via MaterialPropertyBlock.</summary>
        MaterialVector,
        /// <summary>Sets a color property on the material via MaterialPropertyBlock.</summary>
        MaterialColor,
        /// <summary>Sets the radius of a particle system's shape module (for circle/sphere shapes).</summary>
        ShapeRadius,
        /// <summary>Sets the emission rate over time of a particle system.</summary>
        EmissionRateOverTime,
        /// <summary>Sets the emission burst count of a particle system.</summary>
        EmissionBurstCount,
        /// <summary>Sets the start size of a particle system.</summary>
        StartSize,
        /// <summary>Sets the start speed of a particle system.</summary>
        StartSpeed,
        /// <summary>Sets the start lifetime of a particle system.</summary>
        StartLifetime
    }

    /// <summary>
    /// Defines a single property binding that maps a named parameter to a particle system property.
    /// </summary>
    [Serializable]
    public class VFXPropertyBinding
    {
        [Tooltip("The name of the parameter (used when calling SetFloat, SetVector, etc.)")]
        public string ParameterName;

        [Tooltip("The type of property to modify")]
        public VFXPropertyType PropertyType;

        [Tooltip("The target particle system. If null, applies to the root particle system.")]
        public ParticleSystem TargetParticleSystem;

        [Tooltip("Optional multiplier applied to the input value")]
        public float Multiplier = 1f;

        [Tooltip("Optional offset added after multiplier")]
        public float Offset = 0f;

        [Tooltip("For material properties: the shader property name (e.g., '_Radius'). Leave empty to use ParameterName.")]
        public string ShaderPropertyName;

        /// <summary>
        /// Gets the effective shader property name.
        /// </summary>
        public string EffectiveShaderPropertyName =>
            string.IsNullOrEmpty(ShaderPropertyName) ? ParameterName : ShaderPropertyName;
    }

    /// <summary>
    /// Component that binds named parameters to specific VFX properties.
    /// Add this to VFX prefabs to configure how SetFloat/SetVector calls affect child particle systems.
    /// </summary>
    public class VFXPropertyBinder : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of property bindings that map parameter names to particle system properties")]
        private List<VFXPropertyBinding> _bindings = new List<VFXPropertyBinding>();

        private Dictionary<string, List<VFXPropertyBinding>> _bindingLookup;
        private ParticleSystem _rootParticleSystem;
        private ParticleSystemRenderer[] _allRenderers;
        private MaterialPropertyBlock _propertyBlock;

        /// <summary>
        /// Gets all configured bindings.
        /// </summary>
        public IReadOnlyList<VFXPropertyBinding> Bindings => _bindings;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the binder, building the lookup dictionary.
        /// Can be called manually if needed before Awake.
        /// </summary>
        public void Initialize()
        {
            if (_bindingLookup != null)
                return;

            _rootParticleSystem = GetComponent<ParticleSystem>();
            _allRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
            _bindingLookup = new Dictionary<string, List<VFXPropertyBinding>>();

            foreach (var binding in _bindings)
            {
                if (string.IsNullOrEmpty(binding.ParameterName))
                    continue;

                if (!_bindingLookup.TryGetValue(binding.ParameterName, out var list))
                {
                    list = new List<VFXPropertyBinding>();
                    _bindingLookup[binding.ParameterName] = list;
                }

                list.Add(binding);
            }
        }

        /// <summary>
        /// Applies a float value to all bindings with the given parameter name.
        /// </summary>
        /// <param name="parameterName">The parameter name to look up.</param>
        /// <param name="value">The value to apply.</param>
        /// <returns>True if any bindings were found and applied.</returns>
        public bool ApplyFloat(string parameterName, float value)
        {
            Initialize();

            if (!_bindingLookup.TryGetValue(parameterName, out var bindings))
                return false;

            foreach (var binding in bindings)
            {
                float adjustedValue = value * binding.Multiplier + binding.Offset;
                ApplyFloatToBinding(binding, adjustedValue);
            }

            return true;
        }

        /// <summary>
        /// Applies a vector value to all bindings with the given parameter name.
        /// </summary>
        /// <param name="parameterName">The parameter name to look up.</param>
        /// <param name="value">The value to apply.</param>
        /// <returns>True if any bindings were found and applied.</returns>
        public bool ApplyVector(string parameterName, Vector4 value)
        {
            Initialize();

            if (!_bindingLookup.TryGetValue(parameterName, out var bindings))
                return false;

            foreach (var binding in bindings)
            {
                if (binding.PropertyType == VFXPropertyType.MaterialVector)
                {
                    ApplyMaterialVector(binding, value);
                }
                else
                {
                    // For non-vector properties, use the x component
                    float adjustedValue = value.x * binding.Multiplier + binding.Offset;
                    ApplyFloatToBinding(binding, adjustedValue);
                }
            }

            return true;
        }

        /// <summary>
        /// Applies a color value to all bindings with the given parameter name.
        /// </summary>
        /// <param name="parameterName">The parameter name to look up.</param>
        /// <param name="value">The value to apply.</param>
        /// <returns>True if any bindings were found and applied.</returns>
        public bool ApplyColor(string parameterName, Color value)
        {
            Initialize();

            if (!_bindingLookup.TryGetValue(parameterName, out var bindings))
                return false;

            foreach (var binding in bindings)
            {
                if (binding.PropertyType == VFXPropertyType.MaterialColor)
                {
                    ApplyMaterialColor(binding, value);
                }
            }

            return true;
        }

        private void ApplyFloatToBinding(VFXPropertyBinding binding, float value)
        {
            var targetPS = binding.TargetParticleSystem != null ? binding.TargetParticleSystem : _rootParticleSystem;

            switch (binding.PropertyType)
            {
                case VFXPropertyType.MaterialFloat:
                    ApplyMaterialFloat(binding, value);
                    break;

                case VFXPropertyType.ShapeRadius:
                    if (targetPS != null)
                    {
                        var shape = targetPS.shape;
                        shape.radius = value;
                    }
                    break;

                case VFXPropertyType.EmissionRateOverTime:
                    if (targetPS != null)
                    {
                        var emission = targetPS.emission;
                        emission.rateOverTime = value;
                    }
                    break;

                case VFXPropertyType.EmissionBurstCount:
                    if (targetPS != null)
                    {
                        var emission = targetPS.emission;
                        
                        for (int i = 0; i < emission.burstCount; i++)
                        {
                            var burst = emission.GetBurst(i);
                            burst.count = burst.count.constant * value;
                            emission.SetBurst(i, burst);
                        }
                    }
                    break;

                case VFXPropertyType.StartSize:
                    if (targetPS != null)
                    {
                        var main = targetPS.main;
                        main.startSize = value;
                    }
                    break;

                case VFXPropertyType.StartSpeed:
                    if (targetPS != null)
                    {
                        var main = targetPS.main;
                        main.startSpeed = value;
                    }
                    break;

                case VFXPropertyType.StartLifetime:
                    if (targetPS != null)
                    {
                        var main = targetPS.main;
                        main.startLifetime = value;
                    }
                    break;
            }
        }

        private void ApplyMaterialFloat(VFXPropertyBinding binding, float value)
        {
            var targetPS = binding.TargetParticleSystem != null ? binding.TargetParticleSystem : _rootParticleSystem;
            string propertyName = binding.EffectiveShaderPropertyName;

            if (targetPS != null)
            {
                var renderer = targetPS.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetFloat(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
            else
            {
                // Apply to all renderers if no specific target
                foreach (var renderer in _allRenderers)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetFloat(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
        }

        private void ApplyMaterialVector(VFXPropertyBinding binding, Vector4 value)
        {
            var targetPS = binding.TargetParticleSystem != null ? binding.TargetParticleSystem : _rootParticleSystem;
            string propertyName = binding.EffectiveShaderPropertyName;

            if (targetPS != null)
            {
                var renderer = targetPS.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetVector(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
            else
            {
                foreach (var renderer in _allRenderers)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetVector(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
        }

        private void ApplyMaterialColor(VFXPropertyBinding binding, Color value)
        {
            var targetPS = binding.TargetParticleSystem != null ? binding.TargetParticleSystem : _rootParticleSystem;
            string propertyName = binding.EffectiveShaderPropertyName;

            if (targetPS != null)
            {
                var renderer = targetPS.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
            else
            {
                foreach (var renderer in _allRenderers)
                {
                    renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(propertyName, value);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }
        }
    }
}
