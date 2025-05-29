using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyRendering : MonoBehaviour
{
    private GameObject player; // Объект игрока
    private SpriteRenderer spriteRenderer; // Спрайт врага
    private bool isFlashing = false; // Флаг нанесения урона
    private void Start()
    {
        player = GameObject.Find("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(transform.position.y - player.transform.position.y >= 1)
            spriteRenderer.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder - 2;
        else
            spriteRenderer.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder + 2;
    }
    public IEnumerator FlashRed()
    {
        if(isFlashing)
            yield break;

        isFlashing = true;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;

        isFlashing = false;
    }
}
