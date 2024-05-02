using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] float lifeAnimationDuration = 0.15f;
    private void Start()
    {
        Color customColor = new Color(0.458f, 0.819f, 0, 1);
        hpBar.color = customColor;
    }
    public void SetNormalizedValue(float newValue, float newPercentage)
    {
        hpBar.DOFillAmount(newValue, lifeAnimationDuration);
        if (newPercentage > 75)
        {
            Color customColor = new Color(0.458f, 0.819f, 0, 1);
            hpBar.color = customColor;

        }
        else if (newPercentage > 50)
        {
            Color customColor = new Color(1, 0.882f, 0, 1);
            hpBar.color = customColor;
        }
        else if (newPercentage > 25)
        {
            Color customColor = new Color(1, 0.462f, 0, 1);
            hpBar.color = customColor;
        }
        else
        {
            Color customColor = new Color(1, 0, 0, 1);
            hpBar.color = customColor;
        }
    }
}