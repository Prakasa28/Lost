using System;
using UnityEngine;
using СOF.Tools.Constants;

namespace СOF.Tools
{
    // All methods return float/int value
    public static class Converter
    {
        // Conversion of value (dragging) according to aspect ratio of game window
        public static float ConvertByAspectRatio(float value)
        {
            return value * Screen.height / Screen.width;
        }

        // Conversion of float slider value to float field
        public static float ConvertSliderValueToField(float currentSliderValue, float maxFieldValue, float minFieldValue = Limits.MinSliderValue,
                                                     float minSliderValue = Limits.MinSliderValue, float maxSliderValue = Limits.MaxSliderValue)
        {
            return minFieldValue + (maxFieldValue - minFieldValue) * (currentSliderValue - minSliderValue) / (maxSliderValue - minSliderValue);
        }

        // Conversion of initial float field to float slider value
        public static float ConvertFieldToSliderValue(float currentFieldValue, float maxFieldValue, float minFieldValue = Limits.MinSliderValue,
                                                     float minSliderValue = Limits.MinSliderValue, float maxSliderValue = Limits.MaxSliderValue)
        {
            return (float)Math.Round(minSliderValue + (maxSliderValue - minSliderValue) * (currentFieldValue - minFieldValue) / (maxFieldValue - minFieldValue), 2);
        }

        public static float ConvertInitialDeltaToResult(float initialDelta, float controllingSpeed, float? deltaTime = null)
        {
            if (deltaTime == null)
            {
                deltaTime = 1;
            }

            return initialDelta * controllingSpeed * deltaTime.Value;
        }
    }
}
