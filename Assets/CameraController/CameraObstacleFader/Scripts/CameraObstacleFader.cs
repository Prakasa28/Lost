using СOF.Tools.Constants;
using СOF.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace COF
{
    [RequireComponent(typeof(CameraObstacleFader))]
    /// <summary>
    /// Class that fades obstacles of camera
    /// </summary>
    [ExecuteInEditMode]
    public class CameraObstacleFader : MonoBehaviour
    {
        private const string ColorName = "_Color";   
        private Dictionary<int, Renderer> _fadedObstaclesRenderers = new Dictionary<int, Renderer>();
        private Dictionary<int, Material[]> _obstaclesInitialMaterials = new Dictionary<int, Material[]>();
        private GameObject _cameraTarget;
        private Transform _cameraTransform;
        private Transform _targetTransform;
        private float _fadeOutSpeed, _fadeInSpeed;
        private float _fadeRadius;
        private float _distanceOffset;
        private float _fadeResolution;
        private float _fadeOpacity;
        private float _scaledFadeResolution;
        //private float _previousFadeOpacity;

        public Vector3 PositionOffset = new Vector3(0, 0, 0);
        public Transform Target;
        public BlendMode RenderingMode;
        public LayerMask FadeLayerMask;
        public float FadeOutSpeed, FadeInSpeed;
        public float FadeRadius;
        public float FadeOpacity;
        public float DistanceOffset;

        private void Start()
        {
            Time.timeScale = 1.0f;

            _cameraTransform = Camera.main.transform;

            _cameraTarget = GameObject.Find(Names.CameraTarget);
            if (_cameraTarget == null)
            {
                _cameraTarget = new GameObject(Names.CameraTarget);
            }
            _targetTransform = _cameraTarget.transform;
            //if (Target == null)
            Target = GameObject.FindGameObjectWithTag(Names.Player).transform;

            // ===========================================================================================

            _cameraTransform.LookAt(_targetTransform);
        }

        // Call of all Methods each fixed frame
        private void FixedUpdate()
        {
            HashSet<int> hittedObstacles;
            Vector3[] raysEnds, raysDirections;
            float[] raysLengths;
            int raysNumber;

            ConvertSliderValuesToFields();

            _targetTransform.position = Target.position + PositionOffset;
            DefineRays(out raysDirections, out raysEnds, out raysLengths, out raysNumber, _cameraTransform, _fadeRadius, _distanceOffset, _fadeResolution);
            hittedObstacles = HitObstacles(raysNumber, _cameraTransform.position, raysDirections, raysEnds, raysLengths);
            FadeObstacles(ref _fadedObstaclesRenderers, ref _obstaclesInitialMaterials, hittedObstacles);

            ConvertFieldsToSliderValues();
        }

        private void Update()
        {
            // TODO
        }

        // Definition of fading Rays (position, direction, length) that goes from Camera through Obstacles
        private void DefineRays(out Vector3[] raysDirections, out Vector3[] raysEnds, out float[] raysLengths, out int raysNumber, Transform cameraTransform, float fadeRadius, float distanceOffset, float fadeResolution)
        {
            const float radiusError = 1.01f;
            Vector3 raysBegin = cameraTransform.position;
            Vector3 cameraTargetLevelDirection = _targetTransform.position - new Vector3(raysBegin.x, _targetTransform.position.y, raysBegin.z),
                    cameraTargetLevelPosition = new Vector3(raysBegin.x, _targetTransform.position.y, raysBegin.z),
                    cameraRightDirection = cameraTransform.right,
                    cameraUpDirection = cameraTransform.up,
                    cameraforwardDirection = cameraTransform.forward;
            Vector3 raysEndsStart = _targetTransform.position + fadeRadius * (cameraUpDirection - cameraRightDirection);
            int intFadeResolution = Mathf.RoundToInt(fadeResolution); intFadeResolution += intFadeResolution % 2 == 0 ? 1 : 0;
            float raySpace = fadeRadius * 2 / (intFadeResolution - 1);

            raysNumber = (int)Mathf.Pow(intFadeResolution, 2);
            raysEnds = new Vector3[raysNumber]; 
            raysDirections = new Vector3[raysNumber];
            raysLengths = new float[raysNumber];

            // Опредедение начал, направлений и концов лучей
            for (int i = 0; i < intFadeResolution; i++)
            {
                for (int j = 0; j < intFadeResolution; j++)
                {
                    int rayIndex = i * intFadeResolution + j;
                    float x = -fadeRadius + j * raySpace, y = fadeRadius - i * raySpace;

                    if (Mathf.Sqrt(x * x + y * y) <= fadeRadius * radiusError)
                    {
                        raysEnds[rayIndex] = raysEndsStart + j * cameraRightDirection * raySpace - i * cameraUpDirection * raySpace;
                        raysDirections[rayIndex] = (raysEnds[rayIndex] - raysBegin).normalized;
                        raysEnds[rayIndex] += raysDirections[rayIndex].normalized * distanceOffset;
                        raysLengths[rayIndex] = Vector3.Distance(raysBegin, raysEnds[rayIndex]);
                    }
                }
            }

            #region Debug
            //float angle = Vector3.Angle(cameraTargetLevelDirection, playerTransform.forward); //Debug.Log(angle);
            //Debug.DrawLine(cameraTargetLevelPosition, playerTransform.position, Color.red);
            //Debug.DrawRay(playerTransform.position, playerTransform.forward, Color.blue);
            #endregion
        }

        // Casting Rays from Camera to Target through Obstacles
        private HashSet<int> HitObstacles(int raysNumber, Vector3 raysBegin, Vector3[] raysDirections, Vector3[] raysEnds, float[] raysLengths)
        {
            HashSet<int> hittedObstacles = new HashSet<int>();
            RaycastHit[] rayHits;
            Color[] raysColors = new Color[raysNumber];
            Color rayNoHitColor = Color.yellow, rayHitColor = Color.red;

            // defition of obstacle objects that intersect rays
            for (int rayIndex = 0; rayIndex < raysNumber; rayIndex++)
            {
                rayHits = Physics.RaycastAll(raysBegin, raysDirections[rayIndex], raysLengths[rayIndex], FadeLayerMask);

                if (rayHits.Length > 0)
                {
                    raysColors[rayIndex] = rayHitColor;

                    foreach (var hit in rayHits)
                    {
                        var renderer = hit.collider.GetComponent<Renderer>();
                        var obstacleID = hit.collider.gameObject.GetInstanceID();

                        hittedObstacles.Add(obstacleID);
                        TryAddObstacleInitialMaterials(renderer.sharedMaterials, obstacleID, ref _obstaclesInitialMaterials);
                        TryAddObstacleRenderer(renderer, obstacleID, ref _fadedObstaclesRenderers);
                    }
                }
                else
                {
                    raysColors[rayIndex] = rayNoHitColor;
                }
            }

            #region Debug
            for (int i = 0; i < raysNumber; i++)
            {
                if (raysEnds[i] != Vector3.zero)
                {
                    Debug.DrawLine(raysBegin, raysEnds[i], raysColors[i]);
                    //Debug.DrawRay(raysBegin, raysDirections[i] * raysLengths[i], raysColors[i]);
                }
            }
            #endregion

            return hittedObstacles;
        }

        // Addition of new Obstacle Renderer to Dictionary if it is necessary
        private bool TryAddObstacleRenderer(Renderer renderer, int obstacleID, ref Dictionary<int, Renderer> renderers)
        {
            if (!renderers.ContainsKey(obstacleID))
            {
                var fadeRenderer = renderer;

                fadeRenderer.materials = CreateFadeMaterials(renderer.sharedMaterials);
                renderers.Add(obstacleID, fadeRenderer);

                return true;
            }

            return false;
        }

        // Addition of Initial Renderer Materials array to Dictionary if it is necessary
        private bool TryAddObstacleInitialMaterials(Material[] materials, int obstacleID, ref Dictionary<int, Material[]> initialMaterials)
        {
            if (!initialMaterials.ContainsKey(obstacleID))
            {
                initialMaterials.Add(obstacleID, new Material[materials.Length]);

                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    initialMaterials[obstacleID][materialIndex] = new Material(materials[materialIndex]);
                }

                return true;
            }

            return false;
        }

        // Fading in and out of camera obstacles
        private void FadeObstacles(ref Dictionary<int, Renderer> renderers, ref Dictionary<int, Material[]> initialMaterials, HashSet<int> hittedObstacles)
        {
            foreach (var obstacleID in renderers.Keys.ToList())
            {
                bool isObstacleHitted = hittedObstacles.Contains(obstacleID);
                Renderer renderer = renderers[obstacleID];
                Material[] fadedMaterials = null;

                if (isObstacleHitted)
                {
                    fadedMaterials = TryFadeOutObstacle(renderer.sharedMaterials, _fadeOutSpeed, _fadeOpacity);

                    if (fadedMaterials != null)
                    {
                        renderer.materials = fadedMaterials;
                    }
                }              
                else
                {
                    fadedMaterials = FadeInObstacle(renderer.sharedMaterials, initialMaterials[obstacleID], _fadeInSpeed);
                    
                    if (fadedMaterials != null)
                    {
                        renderer.materials = fadedMaterials;
                    }
                    else
                    {
                        renderer.materials = initialMaterials[obstacleID];
                        initialMaterials.Remove(obstacleID);
                        renderers.Remove(obstacleID);
                    }
                }
            }
        }
        
        // Fading Out of camera Obstacle if it is possible
        private Material[] TryFadeOutObstacle(Material[] rendererMaterials, float fadeOutSpeed, float fadeOpacity)
        {
            float deltaOpacity = -Time.fixedDeltaTime * fadeOutSpeed;
            int materialsNumber = rendererMaterials.Length;
            Material[] fadedMaterials = new Material[materialsNumber];
            bool isToFadeOut = false;

            for (int materialIndex = 0; materialIndex < materialsNumber; materialIndex++)
            {
                Color newColor = rendererMaterials[materialIndex].GetColor(ColorName);
                fadedMaterials[materialIndex] = new Material(rendererMaterials[materialIndex]);

                if (newColor.a > fadeOpacity)
                {
                    isToFadeOut = true;
                    newColor.a += deltaOpacity;

                    if (newColor.a < fadeOpacity)
                    {
                        newColor.a = fadeOpacity;
                    }

                    fadedMaterials[materialIndex].SetColor(ColorName, newColor);
                }
            }

            if (isToFadeOut)
            {
               //Debug.Log("FadeOutObstacle");
                return fadedMaterials;
            }

            return null;
        }

        // Fading In of camera Obstacle
        private Material[] FadeInObstacle(Material[] rendererMaterials, Material[] initialMaterials, float fadeInSpeed)
        {
            float deltaOpacity = Time.fixedDeltaTime * fadeInSpeed;
            int materialsNumber = rendererMaterials.Length;
            Material[] fadedMaterials = new Material[materialsNumber];
            bool isFadedIn = true;
            //Debug.Log("FadeInObstacle");
            for (int materialIndex = 0; materialIndex < materialsNumber; materialIndex++)
            {
                Color initialColor = initialMaterials[materialIndex].GetColor(ColorName);
                Color newColor = rendererMaterials[materialIndex].GetColor(ColorName);
                fadedMaterials[materialIndex] = new Material(rendererMaterials[materialIndex]);

                if (newColor.a < initialColor.a)
                {
                    newColor.a += deltaOpacity;

                    if (newColor.a > initialColor.a)
                    {
                        newColor.a = initialColor.a;
                    } 
                    else if (newColor.a < initialColor.a)
                    {
                        isFadedIn = false;
                    }

                    fadedMaterials[materialIndex].SetColor(ColorName, newColor);
                }
            }

            if (isFadedIn)
            {             
                return null;
            }

            return fadedMaterials;
        }

        // Creation of array of Materials that Can Be Faded from Initial Materials 
        private Material[] CreateFadeMaterials(Material[] initialMaterials)
        {
            Material[] fadeMaterials = new Material[initialMaterials.Length];

            for (int index = 0; index < fadeMaterials.Length; index++)
            {
                var fadeMaterial = new Material(initialMaterials[index]);

                fadeMaterial.SetFloat("_Mode", 2);
                fadeMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                fadeMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                fadeMaterial.SetInt("_ZWrite", 0);
                fadeMaterial.DisableKeyword("_ALPHATEST_ON");
                fadeMaterial.EnableKeyword("_ALPHABLEND_ON");
                fadeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                fadeMaterial.renderQueue = 3000;

                fadeMaterials[index] = fadeMaterial;
            }

            return fadeMaterials;
        }

        // Update and Conversion of int Slider Values (in editor) to float Fields
        private void ConvertSliderValuesToFields()
        {
            //_previousFadeOpacity = _fadeOpacity;
            _fadeOutSpeed = Converter.ConvertSliderValueToField(FadeOutSpeed, Limits.MaxFadeSpeed);
            _fadeInSpeed = Converter.ConvertSliderValueToField(FadeInSpeed, Limits.MaxFadeSpeed, Limits.MinFadeInSpeed);
            _fadeOpacity = Converter.ConvertSliderValueToField(FadeOpacity, Limits.MaxFadeOpacity);
            _fadeRadius = Converter.ConvertSliderValueToField(FadeRadius, Limits.MaxFadeRadius, Limits.MinFadeRadius);
            _fadeResolution = Converter.ConvertSliderValueToField(_scaledFadeResolution, Limits.MaxFadeResolution, Limits.MinFadeResolution);
            _distanceOffset = Converter.ConvertSliderValueToField(DistanceOffset, Limits.MaxDistanceOffset, Limits.MinDistanceOffset, -Limits.MaxSliderValue / 2, Limits.MaxSliderValue / 2);
        }
        
        // Update and conversion of float Fields according to int Values in editor Sliders
        private void ConvertFieldsToSliderValues()
        {
            FadeOutSpeed = Converter.ConvertFieldToSliderValue(_fadeOutSpeed, Limits.MaxFadeSpeed);
            FadeInSpeed = Converter.ConvertFieldToSliderValue(_fadeInSpeed, Limits.MaxFadeSpeed, Limits.MinFadeInSpeed);
            FadeOpacity = Converter.ConvertFieldToSliderValue(_fadeOpacity, Limits.MaxFadeOpacity);
            FadeRadius = Converter.ConvertFieldToSliderValue(_fadeRadius, Limits.MaxFadeRadius, Limits.MinFadeRadius);
            //_scaledFadeResolution = Converter.ConvertFieldToSliderValue(_fadeResolution, Limits.MaxFadeResolution, Limits.MinFadeResolution);
            _scaledFadeResolution = FadeRadius;
            DistanceOffset = Converter.ConvertFieldToSliderValue(_distanceOffset, Limits.MaxDistanceOffset, Limits.MinDistanceOffset, -Limits.MaxSliderValue / 2, Limits.MaxSliderValue / 2);
        }
    }
}

//private void ChangeMaterialOpacity(ref Material material, Material oldMaterial, float deltaOpacity, float fadeOpacity)
//{
//    var newMaterial = new Material(material);
//    Color currentColor = material.GetColor("_Color");
//    Color oldColor = oldMaterial.GetColor("_Color");
//    Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, currentColor.a + deltaOpacity);

//    if (deltaOpacity > 0 && newColor.a > oldColor.a)
//    {
//        newColor.a = oldColor.a;
//    }
//    if (deltaOpacity < 0 && newColor.a < fadeOpacity)
//    {
//        newColor.a = fadeOpacity;
//    }

//    newMaterial.SetColor("_Color", newColor);
//    material = newMaterial;
//}
