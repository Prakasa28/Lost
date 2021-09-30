using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using СOF.GUI;
using СOF.Tools.Constants;

namespace COF.Editors
{
    [CustomEditor(typeof(CameraObstacleFader))]
    [CanEditMultipleObjects]
    public class FaderEditor : CommonEditor
    {
        #region Fields
        #region Collections
        private static readonly List<Tuple<string, string>> ToolbarData = new List<Tuple<string, string>>() {
            new Tuple<string, string> (Target, "Settings of camera Target and main Fading settings"),
            new Tuple<string, string> (Obstacle, "Settings of Camera obstacle Fading") 
        };

        private static readonly Dictionary<string, Tuple<string, string>> PropertiesData = new Dictionary<string, Tuple<string, string>>() {
            { Fade + Radius, new Tuple<string, string>(null, "Radius of fade camera view") },
            //{ "FadeResolution", new Tuple<string, string>(null, "Number of rays in row/column") },
            { Fade + LayerMask, new Tuple<string, string>("Layer Mask", "Layers to fade") },
            { "RenderingMode", new Tuple<string, string>(null, "Mode of Fading Obstacles") }, // TODO
            { Fade + Opacity, new Tuple<string, string>(null, "Obstacle opacity after fade") },
            { Distance + Offset, new Tuple<string, string>(null, "Distance between rays` ends and camera target") },
            { Target, new Tuple<string, string>(Object, "Transform of target object") },
            { Position + Offset, new Tuple<string, string>(null, "Actual center of camera target") },
            { FadeOut + Speed, new Tuple<string, string>(null, null) },
            { FadeIn + Speed, new Tuple<string, string>(null, null) } 
        };
        #endregion
        #region Constants
        private const int MaxLogoHeight = 50, LogoPadding = 1, GroupPadding = 10;
        private const float MinWidth = 250, MaxWidth = 450;
        private const string LogoImageName = "COF Logo";
        #endregion
        #region Static and Readonly
        private static int _currentToolbarIndex, _previousToolbarIndex;
        private static Texture _logo;
        private static GUIStyle _logoStyle, _groupStyle;
        private static GUILayoutOption _maxWidthOption, _minWidthOption;
        #endregion
        #region Debug
#if DEBUG
        private SerializedProperty _testProperty;
#endif
        #endregion
        #endregion

        #region Methods
        public override void OnInspectorGUI()
        {
            if (_groupStyle == null)
            {
                _groupStyle = UnityEngine.GUI.skin.box;
                _groupStyle.padding = new RectOffset(GroupPadding, GroupPadding, GroupPadding, GroupPadding);
            }

            if (_minWidthOption == null)
            {
                _minWidthOption = GUILayout.MinWidth(MinWidth);
            }
            if (_maxWidthOption == null)
            {
                _maxWidthOption = GUILayout.MaxWidth(MaxWidth);
            }

            ShowImage(_logo, _logoStyle, GUILayout.MaxHeight(MaxLogoHeight), _maxWidthOption, _minWidthOption);
            ShowToolbar(ref _currentToolbarIndex, ToolbarData.ToArray(), _maxWidthOption);

            BeginGroup(_groupStyle, _maxWidthOption, _minWidthOption);
            switch (_currentToolbarIndex)
            {
                case 0:
                    ShowPropertyField(PropertiesDictionary[Target], EmptySpace);
                    ShowSlider(PropertiesDictionary[Fade + Radius], LargeSpace);
                    //ShowSlider(PropertiesDictionary["FadeResolution"], SmallSpace);
                    ShowSlider(PropertiesDictionary[Distance + Offset], MediumSpace, -Limits.MaxSliderValue / 2, Limits.MaxSliderValue / 2);
                    ShowPropertyField(PropertiesDictionary[Position + Offset]);

                    #region Debug
#if DEBUG
                    //ShowPropertyField(_testProperty, "TestProperty", EmptySpace);
#endif
                    #endregion
                    break;
                case 1:
                    //ShowLabel(LabelsData[Environment], true);
                    var fadeOutProperty = PropertiesDictionary[FadeOut + Speed];
                    var fadeInProperty = PropertiesDictionary[FadeIn + Speed];
                    
                    ShowPropertyField(PropertiesDictionary[Fade + LayerMask], EmptySpace);
                    //ShowPropertyField(PropertiesDictionary["RenderingMode"]);
                    ShowSlider(fadeOutProperty, LargeSpace);
                    if (IsPropertyValuePositive(fadeOutProperty.Item1))
                    {
                        ShowSlider(PropertiesDictionary[FadeIn + Speed]);
                        ShowSlider(PropertiesDictionary[Fade + Opacity], MediumSpace);
                    }
                    break;
            }

            EndGroup();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (PropertiesDictionary.Count == 0)
            {
                foreach (var propertyData in PropertiesData)
                {
                    PropertiesDictionary[propertyData.Key] = new Tuple<SerializedProperty, string, string>
                        (GetProperty(propertyData.Key), propertyData.Value?.Item1, propertyData.Value?.Item2);
                }
            }

            if (_logo == null)
            {
                _logo = (Texture)Resources.Load(LogoImageName);
            }

            if (_logoStyle == null)
            {
                _logoStyle = new GUIStyle { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, LogoPadding, 0) };
            }

            #region Debug
#if DEBUG
            //SetProperty(ref _testProperty, "TestProperty");
#endif
            #endregion
        }

        protected override void EndGroup()
        {
            base.EndGroup();

            ResetFocusControl();
            _previousToolbarIndex = _currentToolbarIndex;
        }

        private void ResetFocusControl()
        {
            if (_currentToolbarIndex != _previousToolbarIndex)
            {
                UnityEngine.GUI.FocusControl(null);
            }
        }
        #endregion
    }
}

//private static readonly Dictionary<string, Tuple<string, string>> LabelsData = new Dictionary<string, Tuple<string, string>>() {
//    //{ Camera_, new Tuple<string, string> (Camera_ + Position, "Camera position") }
//};