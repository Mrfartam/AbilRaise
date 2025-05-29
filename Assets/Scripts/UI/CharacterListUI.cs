using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListUI : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки
    public Transform contentParent; // Контейнер ScrollView (Content)

    static public Character character; // Никнейм выбранного персонажа

    void Start()
    {
        LoadCharactersToList();
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
    public void LoadCharactersToList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        CharacterData data = CharacterSaver.LoadAllCharacters();

        foreach (Character character in data.characters)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentParent);
            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            text.text = character.name + "\nУбито врагов:" + character.curKilledMobs;

            Button button = buttonObj.GetComponent<Button>();
            string characterName = character.name;
            button.onClick.AddListener(() => OnCharacterSelected(characterName, button.gameObject));
            buttonObj.transform.Find("DeleteButton").gameObject.GetComponent<Button>().onClick.AddListener(
                () => OnCharacterDelete(character.name));
        }
    }

    public void OnCharacterSelected(string name, GameObject button)
    {
        Debug.Log("Выбран персонаж: " + name);

        foreach (Transform child in contentParent)
        {
            child.gameObject.GetComponent<Image>().color = Color.white;
            child.Find("DeleteButton").gameObject.SetActive(false);
        }
        button.GetComponent<Image>().color = Color.gray;
        button.transform.Find("DeleteButton").gameObject.SetActive(true);

        GameObject nextButtonObj = transform.Find("NextButton").gameObject;
        nextButtonObj.SetActive(true);

        Character character = CharacterSaver.LoadCharacter(name);
        CharacterListUI.character = character;
    }
    public void OnCharacterDelete(string name)
    {
        CharacterSaver.DeleteCharacter(name);
        LoadCharactersToList();
    }
}
