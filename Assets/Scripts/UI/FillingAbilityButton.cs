using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FillingAbilityButton : MonoBehaviour
{
    public Image fillingImage; // Картинка заполнения кнопки способности
    private Image buttonImage; // Картинка самой кнопки
    public float maxValue = 100f; // Максимальное значение заполнения
    public float curValue = 0f; // Текущее значение заполнения

    public Weapon weapon; // Оружие
    void Start()
    {
        buttonImage = GetComponent<Image>();
        UpdateFill();
    }

    public void SetValue(float value)
    {
        curValue = Mathf.Clamp(value, 0, maxValue);
        if(curValue == maxValue)
        {
            Color color = new Color(fillingImage.color.r, fillingImage.color.g, fillingImage.color.b, 1);
            buttonImage.color = Color.white;
            weapon.ClearColor(color);
            weapon.PlayParticles();
            Color[] gradient = new Color[2];
            gradient[0] = new Color(
                (color.r * 255 + 40 <= 255) ? color.r + (float)40 / 255 : 1,
                (color.g * 255 + 40 <= 255) ? color.g + (float)40 / 255 : 1,
                (color.b * 255 + 40 <= 255) ? color.b + (float)40 / 255 : 1, 1);
            gradient[1] = color;
            weapon.SetColorOfTrail(gradient);
        }
        UpdateFill();
    }

    private void UpdateFill()
    {
        fillingImage.fillAmount = curValue / maxValue;
    }

    public void IncreaseValue(float amount)
    {
        SetValue(curValue + amount);
    }

    public void ClearValue()
    {
        SetValue(0);
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, (float)150/255);
        weapon.ClearColor(Color.white);
        weapon.StopParticles();
        Color[] gradient = new Color[2];
        gradient[0] = new Color((float)158 / 255, (float)158 / 255, (float)175 / 255, 1);
        gradient[1] = new Color((float)115 / 255, (float)125 / 255, (float)139 / 255, 1);
        weapon.SetColorOfTrail(gradient);
    }
}
