using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldListUI : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки
    public Transform contentParent; // Контейнер ScrollView (Content)

    static public World world; // Название выбранного мира
    void Start()
    {
        LoadWorldsToList();
    }
    public void ClosePanel()
    {
        foreach (Transform child in contentParent)
        {
            child.gameObject.GetComponent<Image>().color = Color.white;
            child.Find("DeleteButton").gameObject.SetActive(false);
        }

        GameObject nextButtonObj = transform.Find("NextButton").gameObject;
        nextButtonObj.SetActive(false);
    }
    void LoadWorldsToList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        WorldData data = WorldSaver.LoadAllWorlds();

        foreach (World world in data.worlds)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentParent);
            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            text.text = world.name + "\n" + world.seed;

            Button button = buttonObj.GetComponent<Button>();
            string worldName = world.name;
            button.onClick.AddListener(() => OnWorldSelected(worldName, button.gameObject));
            buttonObj.transform.Find("DeleteButton").gameObject.GetComponent<Button>().onClick.AddListener(
                () => OnWorldDelete(world.name));
        }
    }
    void OnWorldSelected(string name, GameObject button)
    {
        Debug.Log("Выбран мир: " + name);

        foreach (Transform child in contentParent)
        {
            child.gameObject.GetComponent<Image>().color = Color.white;
            child.Find("DeleteButton").gameObject.SetActive(false);
        }
        button.GetComponent<Image>().color = Color.gray;
        button.transform.Find("DeleteButton").gameObject.SetActive(true);

        GameObject nextButtonObj = transform.Find("NextButton").gameObject;
        nextButtonObj.SetActive(true);

        World world = WorldSaver.LoadWorld(name);
        WorldListUI.world = world;
    }
    void OnWorldDelete(string name)
    {
        WorldSaver.DeleteWorld(name);
        LoadWorldsToList();
    }
}
