using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public World world; // Выбранный мир
    public Character player; // Выбранный персонаж

    public static SceneController instance; // Объект-одиночка контроллера сцены
    public static SceneController Instance => instance; // Геттер для instance
    public Canvas mainCanvas; // Канвас игрового интерфейса

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadingScene()
    {
        StartCoroutine(StartLoadingScene());
    }
    private IEnumerator StartLoadingScene()
    {
        mainCanvas.enabled = false;

        yield return LoadLoadingScene();

        yield return new WaitForSeconds(2f);

        yield return UnloadLoadingScene();

        mainCanvas.enabled = true;
    }
    private IEnumerator LoadLoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
            yield return null;

        Scene loadingScene = SceneManager.GetSceneByBuildIndex(2);

        if (!loadingScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadingScene);
            Debug.Log(mainCanvas);
        }
    }
    private IEnumerator UnloadLoadingScene()
    {
        Scene mainScene = SceneManager.GetSceneByBuildIndex(1);
        if (mainScene.IsValid())
        {
            SceneManager.SetActiveScene(mainScene);
        }

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(2);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        yield return Resources.UnloadUnusedAssets();
    }
    public void GameScene(World world, Character player)
    {
        this.world = world;
        this.player = player;
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
            yield return null;

        yield return null;

        ProceduralLevelGenerator lvlGenerator = GameObject.Find("ProceduralLevelGenerator").GetComponent<ProceduralLevelGenerator>();
        lvlGenerator.worldName = world.name;
        lvlGenerator.StartGameBySave();
        this.mainCanvas = GameObject.Find("Canvas").gameObject.GetComponent<Canvas>();
        Player curPlayer = GameObject.Find("Player").GetComponent<Player>();
        curPlayer.nickname = player.name;
        curPlayer.numOfKilledEnemy = player.curKilledMobs;
    }
    public void MainMenuScene()
    {
        StartCoroutine(LoadMainMenuScene());
    }
    private IEnumerator LoadMainMenuScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
            yield return null;

        yield return null;
    }
}
