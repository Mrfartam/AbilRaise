using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    private GameObject player; // Ссылка на объект игрока
    private RoomsGenerator roomsGenerator; // Ссылка на генератор мира
    private ProceduralLevelGenerator levelGenerator; // Ссылка на генератор уровня
    private PolygonCollider2D polygonCollider; // Коллайдер портала
    
    public SpriteRenderer spriteRenderer; // Объект спрайта портала
    public Camera mainCamera; // Ссылка на объект камеры

    private void Start()
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;
        roomsGenerator = FindObjectOfType<RoomsGenerator>();
        levelGenerator = FindObjectOfType<ProceduralLevelGenerator>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }
    void Update()
    {
        if(spriteRenderer != null && player != null)
        {
            if (player.transform.position.y - transform.position.y >= 0.8f)
                spriteRenderer.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder + 1;
            else
                spriteRenderer.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder - 1;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.IsTouching(polygonCollider))
        {
            Scene loadingScene = SceneManager.GetSceneByBuildIndex(2);
            if (!loadingScene.isLoaded)
            {
                SceneController.Instance.LoadingScene();
            }

            roomsGenerator.DestroyLevel();
            player.transform.position = Vector3.zero;
            levelGenerator.curLevel++;
            levelGenerator.StartGame();

            World world = new World
            {
                name = levelGenerator.worldName,
                curLvl = levelGenerator.curLevel,
                seed = levelGenerator.seed,
                openedGates = new List<Gate>()
            };
            WorldSaver.SaveWorld(world);
        }
    }
}
