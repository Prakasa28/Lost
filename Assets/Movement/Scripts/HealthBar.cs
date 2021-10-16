using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private bool withGradient = true;

    void Awake()
    {
        slider = GetComponent<Slider>();
        fill = GetComponentInChildren<Image>();
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        if (withGradient)
        {
            fill.color = gradient.Evaluate(1f);
        }
    }

    public void WithGradient()
    {
        withGradient = true;
    }

    public void SetHealth (int health)
    {
        slider.value = health;

        if (withGradient)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
