using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButtonBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerUpHandler
{
    public RectTransform targetPos; // ���������� ������ ������ �����
    public float activationDistance = 30f; // ������ ��������� �� ������ ������ ����� (� ��������)
    public float returnSpeed = 100f; // �������� �������� �� �������� �������

    private RectTransform rectTransform; // ������� ��������� �������
    private Vector2 initialPosition; // ���������� ����� ������ �����������
    private bool isDragging = false; // ���� �������� ������
    private float pressTime; // ����� �������

    private FillingAbilityButton curFilling; // ����� �����������
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
        Debug.Log("���!!! ������������� �����!");
    }
    private void ActivateAbility()
    {
        GetComponent<FillingAbilityButton>().ClearValue();
        Debug.Log("�����������!");
    }
}
