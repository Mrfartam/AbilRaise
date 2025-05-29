using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButtonBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerUpHandler
{
    public RectTransform targetPos; // Координаты центра кнопки атаки
    public float activationDistance = 30f; // Радиус активации до центра кнопки атаки (в пикселях)
    public float returnSpeed = 100f; // Скорость возврата на исходную позицию

    private RectTransform rectTransform; // Система координат канваса
    private Vector2 initialPosition; // Координаты цетра кнопки способности
    private bool isDragging = false; // Флаг таскания кнопки
    private float pressTime; // Время нажатия

    private FillingAbilityButton curFilling; // Шкала способности
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        curFilling = GetComponent<FillingAbilityButton>();
        initialPosition = rectTransform.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;

        rectTransform.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (curFilling.curValue == 100f && Vector2.Distance(rectTransform.anchoredPosition, targetPos.anchoredPosition) <= activationDistance)
        {
            UltimatumAttack();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDragging)
        {
            pressTime = Time.time;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(curFilling.curValue);
        if (curFilling.curValue == 100f && !isDragging && Time.time - pressTime > 0.1f && Time.time - pressTime < 0.5f)
        {
            ActivateAbility();
        }
    }
    void Update()
    {
        if(!isDragging && rectTransform.anchoredPosition != initialPosition)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, initialPosition, returnSpeed * Time.deltaTime);
        }
    }
    private void UltimatumAttack()
    {
        GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>().ClickButtonAttack();
        GameObject.FindGameObjectWithTag("Sword").transform.Find("AttackPos").GetComponent<PlayerAttack>().isUltimative = true;
        GetComponent<FillingAbilityButton>().ClearValue();
        Debug.Log("Бум!!! Ультимативная атака!");
    }
    private void ActivateAbility()
    {
        GetComponent<FillingAbilityButton>().ClearValue();
        Debug.Log("Способность!");
    }
}
