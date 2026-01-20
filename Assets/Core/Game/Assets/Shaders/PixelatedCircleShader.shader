Shader "Custom/PixelatedCircleAdvanced"
{
    Properties
    {
        [Header(Geometry)]
        _Radius ("Radius", Float) = 2.0
        _PixelSize ("Pixel Size (World Units)", Float) = 0.25
        _BorderWidth ("Border Width (in pixels)", Float) = 1.0
        
        [Header(Border)]
        [HDR] _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
        [HDR] _BorderColorSecondary ("Border Secondary Color", Color) = (0, 0.5, 1, 1)
        [Toggle] _AnimateBorder ("Animate Border", Float) = 1
        
        [Header(Fill)]
        [Toggle] _EnableFill ("Enable Fill", Float) = 0
        [HDR] _FillColor ("Fill Color", Color) = (0.2, 0.2, 0.8, 0.5)
        [HDR] _FillColorSecondary ("Fill Secondary Color", Color) = (0.1, 0.1, 0.4, 0.5)
        [Toggle] _AnimateFill ("Animate Fill", Float) = 1
        
        [Header(Inner Border)]
        [Toggle] _EnableInnerBorder ("Enable Inner Border", Float) = 0
        _InnerBorderWidth ("Inner Border Width (in pixels)", Float) = 1.0
        _InnerBorderOffset ("Inner Border Offset (in pixels)", Float) = 2.0
        [HDR] _InnerBorderColor ("Inner Border Color", Color) = (0.5, 0.5, 1, 0.5)
        [HDR] _InnerBorderColorSecondary ("Inner Border Secondary Color", Color) = (0.2, 0.2, 0.5, 0.5)
        [Toggle] _AnimateInnerBorder ("Animate Inner Border", Float) = 1
        
        [Header(Outer Glow)]
        [Toggle] _EnableOuterGlow ("Enable Outer Glow", Float) = 0
        _OuterGlowWidth ("Outer Glow Width (in pixels)", Float) = 2.0
        [HDR] _OuterGlowColor ("Outer Glow Color", Color) = (0, 0.5, 1, 0.3)
        [Toggle] _AnimateOuterGlow ("Animate Outer Glow", Float) = 1
        
        [Header(Animation)]
        [KeywordEnum(None, Wave, Random, Radial, Checkerboard, Spiral, Segments)] 
        _AnimMode ("Animation Mode", Float) = 1
        _AnimSpeed ("Animation Speed", Float) = 2.0
        _AnimScale ("Animation Scale", Float) = 1.0
        _AnimOffset ("Animation Offset", Float) = 0.0
        _SegmentCount ("Segment Count (for Segments mode)", Float) = 8
        
        [Header(Particle Integration)]
        [Toggle] _UseParticleColor ("Use Particle Color (multiplies all)", Float) = 1
        [Toggle] _UseParticleAlpha ("Use Particle Alpha", Float) = 1
        [Toggle(_USE_CUSTOM_DATA)] _UseCustomData ("Use Custom Data (X=AnimOffset, Y=Radius)", Float) = 0
        _CustomData ("Custom Data (Internal)", Vector) = (0,0,0,0)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            Name "PixelatedCircleAdvanced"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            
            #pragma shader_feature_local _ANIMMODE_NONE _ANIMMODE_WAVE _ANIMMODE_RANDOM _ANIMMODE_RADIAL _ANIMMODE_CHECKERBOARD _ANIMMODE_SPIRAL _ANIMMODE_SEGMENTS
            #pragma shader_feature_local _USE_CUSTOM_DATA
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct ParticleInstanceData
            {
                float3x4 transform;
                #if !defined(UNITY_PARTICLE_INSTANCE_DATA_NO_COLOR)
                uint color;
                #endif
                #if !defined(UNITY_PARTICLE_INSTANCE_DATA_NO_ANIM_FRAME)
                float animFrame;
                #endif
                #if defined(_USE_CUSTOM_DATA)
                float4 customData;
                #endif
            };
            #define UNITY_PARTICLE_INSTANCE_DATA ParticleInstanceData
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ParticlesInstancing.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 customData : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 centerWS : TEXCOORD1;
                float4 color : COLOR;
                float4 customData : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            CBUFFER_START(UnityPerMaterial)
                // Geometry
                float _Radius;
                float _PixelSize;
                float _BorderWidth;
                
                // Border
                float4 _BorderColor;
                float4 _BorderColorSecondary;
                float _AnimateBorder;
                
                // Fill
                float _EnableFill;
                float4 _FillColor;
                float4 _FillColorSecondary;
                float _AnimateFill;
                
                // Inner Border
                float _EnableInnerBorder;
                float _InnerBorderWidth;
                float _InnerBorderOffset;
                float4 _InnerBorderColor;
                float4 _InnerBorderColorSecondary;
                float _AnimateInnerBorder;
                
                // Outer Glow
                float _EnableOuterGlow;
                float _OuterGlowWidth;
                float4 _OuterGlowColor;
                float _AnimateOuterGlow;
                
                // Animation
                float _AnimSpeed;
                float _AnimScale;
                float _AnimOffset;
                float _SegmentCount;
                
                // Particle
                float _UseParticleColor;
                float _UseParticleAlpha;
                float _UseCustomData;
                float4 _CustomData;
            CBUFFER_END
            
            // Hash functions
            float hash21(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }
            
            float noise21(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // Animation calculation - returns 0-1 value
            float CalculateAnimation(float2 pixelCoord, float2 quantizedOffset, float dist, float time)
            {
                float animValue = 0.0;
                
                #if defined(_ANIMMODE_WAVE)
                    float wavePhase = dist * _AnimScale - time;
                    animValue = sin(wavePhase) * 0.5 + 0.5;
                    
                #elif defined(_ANIMMODE_RANDOM)
                    float pixelRandom = hash21(pixelCoord);
                    float flickerSpeed = lerp(0.5, 2.0, pixelRandom);
                    animValue = sin(time * flickerSpeed + pixelRandom * 6.28) * 0.5 + 0.5;
                    animValue *= noise21(pixelCoord * 0.5 + time * 0.3);
                    animValue = saturate(animValue * 2.0);
                    
                #elif defined(_ANIMMODE_RADIAL)
                    animValue = sin(dist * _AnimScale * 3.14159 - time) * 0.5 + 0.5;
                    
                #elif defined(_ANIMMODE_CHECKERBOARD)
                    float2 checker = floor(pixelCoord + time);
                    animValue = fmod(checker.x + checker.y, 2.0);
                    
                #elif defined(_ANIMMODE_SPIRAL)
                    float angle = atan2(quantizedOffset.y, quantizedOffset.x);
                    animValue = sin(angle * 3.0 + dist * _AnimScale - time) * 0.5 + 0.5;
                    
                #elif defined(_ANIMMODE_SEGMENTS)
                    float angle = atan2(quantizedOffset.y, quantizedOffset.x);
                    float segmentAngle = 6.28318 / _SegmentCount;
                    float segment = floor((angle + 3.14159) / segmentAngle);
                    float segmentPhase = segment / _SegmentCount;
                    animValue = sin(segmentPhase * 6.28318 + time) * 0.5 + 0.5;
                    
                #endif
                
                return animValue;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.centerWS = TransformObjectToWorld(float3(0, 0, 0));
                
                #if defined(UNITY_PARTICLE_INSTANCING_ENABLED)
                    UNITY_PARTICLE_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];
                    output.color = lerp(half4(1.0, 1.0, 1.0, 1.0), input.color, unity_ParticleUseMeshColors);
                    #if !defined(UNITY_PARTICLE_INSTANCE_DATA_NO_COLOR)
                        output.color *= UnpackFromR8G8B8A8(data.color);
                    #endif
                    
                    #if defined(_USE_CUSTOM_DATA)
                        output.customData = data.customData;
                    #else
                        output.customData = input.customData;
                    #endif
                #else
                    output.color = input.color;
                    output.customData = input.customData;
                #endif
                
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                // Custom data overrides
                float radius = _Radius;
                float animOffset = _AnimOffset;
                
                #if defined(_USE_CUSTOM_DATA)
                animOffset += input.customData.x;
                if (input.customData.y > 0.001)
                {
                    radius = input.customData.y;
                }
                #endif
                
                // Also support non-keyword property block overrides
                if (_UseCustomData > 0.5)
                {
                    animOffset += _CustomData.x;
                    if (_CustomData.y > 0.001)
                    {
                        radius = _CustomData.y;
                    }
                }
                
                float time = _Time.y * _AnimSpeed + animOffset;
                
                // Calculate offset from center in world space (XZ plane)
                float2 offsetWS = input.positionWS.xz - input.centerWS.xz;
                
                // Quantize position to pixel grid
                float2 pixelCoord = floor(offsetWS / _PixelSize + 0.5);
                float2 quantizedOffset = pixelCoord * _PixelSize;
                float dist = length(quantizedOffset);
                
                // Calculate animation value
                float animValue = CalculateAnimation(pixelCoord, quantizedOffset, dist, time);
                
                // Define all the radii
                float outerGlowOuter = radius + (_OuterGlowWidth * _PixelSize);
                float outerGlowInner = radius;
                float borderOuter = radius;
                float borderInner = radius - (_BorderWidth * _PixelSize);
                float innerBorderOuter = radius - (_InnerBorderOffset * _PixelSize);
                float innerBorderInner = innerBorderOuter - (_InnerBorderWidth * _PixelSize);
                
                // Calculate masks for each zone
                float inOuterGlow = step(dist, outerGlowOuter) * (1.0 - step(dist, outerGlowInner)) * _EnableOuterGlow;
                float inBorder = step(dist, borderOuter) * (1.0 - step(dist, borderInner));
                float inInnerBorder = step(dist, innerBorderOuter) * (1.0 - step(dist, innerBorderInner)) * _EnableInnerBorder;
                float inFill = step(dist, borderInner) * _EnableFill * (1.0 - inInnerBorder);
                
                // Check if we're in any zone
                float inAnyZone = saturate(inOuterGlow + inBorder + inInnerBorder + inFill);
                clip(inAnyZone - 0.001);
                
                // Calculate colors for each zone
                float4 outerGlowFinal = _OuterGlowColor;
                if (_AnimateOuterGlow > 0.5)
                {
                    // Fade based on distance from border
                    float glowDist = (dist - radius) / (_OuterGlowWidth * _PixelSize);
                    outerGlowFinal.a *= (1.0 - glowDist) * (0.5 + 0.5 * animValue);
                }
                
                float4 borderFinal = lerp(_BorderColor, _BorderColorSecondary, 
                    _AnimateBorder > 0.5 ? animValue : 0.0);
                
                float4 innerBorderFinal = lerp(_InnerBorderColor, _InnerBorderColorSecondary, 
                    _AnimateInnerBorder > 0.5 ? animValue : 0.0);
                
                float4 fillFinal = lerp(_FillColor, _FillColorSecondary, 
                    _AnimateFill > 0.5 ? animValue : 0.0);
                
                // Composite all layers (back to front: fill -> inner border -> border -> outer glow)
                float4 finalColor = float4(0, 0, 0, 0);
                
                // Fill (base layer)
                finalColor = lerp(finalColor, fillFinal, inFill * fillFinal.a);
                
                // Inner border
                finalColor = lerp(finalColor, innerBorderFinal, inInnerBorder * innerBorderFinal.a);
                
                // Main border
                finalColor = lerp(finalColor, borderFinal, inBorder * borderFinal.a);
                
                // Outer glow (additive-ish blend)
                finalColor.rgb += outerGlowFinal.rgb * inOuterGlow * outerGlowFinal.a;
                finalColor.a = saturate(finalColor.a + inOuterGlow * outerGlowFinal.a);
                
                // Apply particle color
                if (_UseParticleColor > 0.5)
                {
                    finalColor.rgb *= input.color.rgb;
                }
                
                if (_UseParticleAlpha > 0.5)
                {
                    finalColor.a *= input.color.a;
                }
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    CustomEditor "UnityEditor.ShaderGraphLiteGUI"
}