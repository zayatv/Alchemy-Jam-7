using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Systems.UI.Common.Button
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class ButtonVisualElement
    {
        [HideLabel]
        public string DisplaySummary => GetDisplaySummary();

        [BoxGroup("Setup")]
        [LabelText("Target")]
        [Tooltip("The visual component to animate (Image, CanvasGroup, RectTransform, etc.)")]
        public Component target;

        [BoxGroup("Setup")]
        [LabelText("Type")]
        [Tooltip("What property to animate on this element")]
        public VisualAnimationType animationType = VisualAnimationType.None;

        [BoxGroup("Setup")]
        [LabelText("Duration")]
        [Tooltip("Duration of the transition animation")]
        [SuffixLabel("s", true)]
        public float transitionDuration = 0.2f;

        [BoxGroup("Setup")]
        [LabelText("Ease")]
        [Tooltip("Easing curve for the animation")]
        public Ease transitionEase = Ease.OutQuad;

        [ShowIf(nameof(ShowColorSettings))]
        [HideLabel]
        public ColorStateConfig colorConfig = new ColorStateConfig();

        [ShowIf(nameof(ShowScaleSettings))]
        [HideLabel]
        public ScaleStateConfig scaleConfig = new ScaleStateConfig();

        [ShowIf(nameof(ShowPositionSettings))]
        [HideLabel]
        public PositionStateConfig positionConfig = new PositionStateConfig();

        [ShowIf(nameof(ShowRotationSettings))]
        [HideLabel]
        public RotationStateConfig rotationConfig = new RotationStateConfig();

        [ShowIf(nameof(ShowAlphaSettings))]
        [HideLabel]
        public AlphaStateConfig alphaConfig = new AlphaStateConfig();

        private bool ShowColorSettings() => animationType == VisualAnimationType.Color || animationType == VisualAnimationType.All;
        private bool ShowScaleSettings() => animationType == VisualAnimationType.Scale || animationType == VisualAnimationType.All;
        private bool ShowPositionSettings() => animationType == VisualAnimationType.Position || animationType == VisualAnimationType.All;
        private bool ShowRotationSettings() => animationType == VisualAnimationType.Rotation || animationType == VisualAnimationType.All;
        private bool ShowAlphaSettings() => animationType == VisualAnimationType.Alpha || animationType == VisualAnimationType.All;

        private string GetDisplaySummary()
        {
            string targetName = target != null ? target.name : "None";
            string typeStr = animationType.ToString();
            string durationStr = $"{transitionDuration:F2}s";

            return $"[{typeStr}] {targetName} • {durationStr} • {transitionEase}";
        }
    }

    public enum VisualAnimationType
    {
        None,
        Color,
        Scale,
        Position,
        Rotation,
        Alpha,
        All
    }

    public enum ScaleMode
    {
        Multiplier,
        Pixels
    }

    [Serializable]
    public class ColorStateConfig
    {
        [FoldoutGroup("Colors", expanded: true)]
        [LabelText("Normal")]
        public Color normalColor = Color.white;

        [FoldoutGroup("Colors")]
        [LabelText("Highlighted")]
        public Color highlightedColor = new Color(1f, 0.9f, 0.7f);

        [FoldoutGroup("Colors")]
        [LabelText("Pressed")]
        public Color pressedColor = new Color(0.8f, 0.8f, 0.8f);

        [FoldoutGroup("Colors")]
        [LabelText("Selected")]
        public Color selectedColor = new Color(1f, 0.85f, 0.5f);

        [FoldoutGroup("Colors")]
        [LabelText("Disabled")]
        public Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

        public Color GetColorForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalColor,
                ButtonState.Highlighted => highlightedColor,
                ButtonState.Pressed => pressedColor,
                ButtonState.Selected => selectedColor,
                ButtonState.Disabled => disabledColor,
                _ => normalColor
            };
        }
    }

    [Serializable]
    public class ScaleStateConfig
    {
        [FoldoutGroup("Scale", expanded: true)]
        [LabelText("Mode")]
        [Tooltip("Multiplier: Scale by factor (1.05 = 5% larger). Pixels: Expand by pixel amount (good for stretched UI)")]
        public ScaleMode scaleMode = ScaleMode.Multiplier;

        [FoldoutGroup("Scale")]
        [LabelText("Normal")]
        [ShowIf(nameof(scaleMode), ScaleMode.Multiplier)]
        public Vector3 normalScale = Vector3.one;

        [FoldoutGroup("Scale")]
        [LabelText("Highlighted")]
        [ShowIf(nameof(scaleMode), ScaleMode.Multiplier)]
        public Vector3 highlightedScale = new Vector3(1.05f, 1.05f, 1f);

        [FoldoutGroup("Scale")]
        [LabelText("Pressed")]
        [ShowIf(nameof(scaleMode), ScaleMode.Multiplier)]
        public Vector3 pressedScale = new Vector3(0.95f, 0.95f, 1f);

        [FoldoutGroup("Scale")]
        [LabelText("Selected")]
        [ShowIf(nameof(scaleMode), ScaleMode.Multiplier)]
        public Vector3 selectedScale = Vector3.one;

        [FoldoutGroup("Scale")]
        [LabelText("Disabled")]
        [ShowIf(nameof(scaleMode), ScaleMode.Multiplier)]
        public Vector3 disabledScale = Vector3.one;

        [FoldoutGroup("Scale")]
        [LabelText("Normal (Pixels)")]
        [ShowIf(nameof(scaleMode), ScaleMode.Pixels)]
        [Tooltip("Pixel expansion on each side (10 = 20px larger in each dimension)")]
        public Vector2 normalPixels = Vector2.zero;

        [FoldoutGroup("Scale")]
        [LabelText("Highlighted (Pixels)")]
        [ShowIf(nameof(scaleMode), ScaleMode.Pixels)]
        [Tooltip("Pixel expansion on each side")]
        public Vector2 highlightedPixels = new Vector2(10f, 10f);

        [FoldoutGroup("Scale")]
        [LabelText("Pressed (Pixels)")]
        [ShowIf(nameof(scaleMode), ScaleMode.Pixels)]
        [Tooltip("Pixel expansion on each side (negative = shrink)")]
        public Vector2 pressedPixels = new Vector2(-5f, -5f);

        [FoldoutGroup("Scale")]
        [LabelText("Selected (Pixels)")]
        [ShowIf(nameof(scaleMode), ScaleMode.Pixels)]
        [Tooltip("Pixel expansion on each side")]
        public Vector2 selectedPixels = Vector2.zero;

        [FoldoutGroup("Scale")]
        [LabelText("Disabled (Pixels)")]
        [ShowIf(nameof(scaleMode), ScaleMode.Pixels)]
        [Tooltip("Pixel expansion on each side")]
        public Vector2 disabledPixels = Vector2.zero;

        public Vector3 GetScaleForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalScale,
                ButtonState.Highlighted => highlightedScale,
                ButtonState.Pressed => pressedScale,
                ButtonState.Selected => selectedScale,
                ButtonState.Disabled => disabledScale,
                _ => normalScale
            };
        }

        public Vector2 GetPixelsForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalPixels,
                ButtonState.Highlighted => highlightedPixels,
                ButtonState.Pressed => pressedPixels,
                ButtonState.Selected => selectedPixels,
                ButtonState.Disabled => disabledPixels,
                _ => normalPixels
            };
        }
    }

    [Serializable]
    public class PositionStateConfig
    {
        [FoldoutGroup("Position", expanded: true)]
        [LabelText("Normal")]
        public Vector3 normalPosition = Vector3.zero;

        [FoldoutGroup("Position")]
        [LabelText("Highlighted")]
        public Vector3 highlightedPosition = Vector3.zero;

        [FoldoutGroup("Position")]
        [LabelText("Pressed")]
        public Vector3 pressedPosition = Vector3.zero;

        [FoldoutGroup("Position")]
        [LabelText("Selected")]
        public Vector3 selectedPosition = Vector3.zero;

        [FoldoutGroup("Position")]
        [LabelText("Disabled")]
        public Vector3 disabledPosition = Vector3.zero;

        public Vector3 GetPositionForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalPosition,
                ButtonState.Highlighted => highlightedPosition,
                ButtonState.Pressed => pressedPosition,
                ButtonState.Selected => selectedPosition,
                ButtonState.Disabled => disabledPosition,
                _ => normalPosition
            };
        }
    }

    [Serializable]
    public class RotationStateConfig
    {
        [FoldoutGroup("Rotation", expanded: true)]
        [LabelText("Normal")]
        public Vector3 normalRotation = Vector3.zero;

        [FoldoutGroup("Rotation")]
        [LabelText("Highlighted")]
        public Vector3 highlightedRotation = Vector3.zero;

        [FoldoutGroup("Rotation")]
        [LabelText("Pressed")]
        public Vector3 pressedRotation = Vector3.zero;

        [FoldoutGroup("Rotation")]
        [LabelText("Selected")]
        public Vector3 selectedRotation = Vector3.zero;

        [FoldoutGroup("Rotation")]
        [LabelText("Disabled")]
        public Vector3 disabledRotation = Vector3.zero;

        public Vector3 GetRotationForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalRotation,
                ButtonState.Highlighted => highlightedRotation,
                ButtonState.Pressed => pressedRotation,
                ButtonState.Selected => selectedRotation,
                ButtonState.Disabled => disabledRotation,
                _ => normalRotation
            };
        }
    }

    [Serializable]
    public class AlphaStateConfig
    {
        [FoldoutGroup("Alpha", expanded: true)]
        [LabelText("Normal")]
        [Range(0f, 1f)]
        public float normalAlpha = 1f;

        [FoldoutGroup("Alpha")]
        [LabelText("Highlighted")]
        [Range(0f, 1f)]
        public float highlightedAlpha = 1f;

        [FoldoutGroup("Alpha")]
        [LabelText("Pressed")]
        [Range(0f, 1f)]
        public float pressedAlpha = 1f;

        [FoldoutGroup("Alpha")]
        [LabelText("Selected")]
        [Range(0f, 1f)]
        public float selectedAlpha = 1f;

        [FoldoutGroup("Alpha")]
        [LabelText("Disabled")]
        [Range(0f, 1f)]
        public float disabledAlpha = 0.5f;

        public float GetAlphaForState(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalAlpha,
                ButtonState.Highlighted => highlightedAlpha,
                ButtonState.Pressed => pressedAlpha,
                ButtonState.Selected => selectedAlpha,
                ButtonState.Disabled => disabledAlpha,
                _ => normalAlpha
            };
        }
    }
}
