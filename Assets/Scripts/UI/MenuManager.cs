using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header ("Панели")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectPanel;
    public GameObject characterCreatePanel;
    public GameObject worldSelectPanel;
    public GameObject worldCreatePanel;

    [Header ("Поля ввода")]
    public TMP_InputField nicknameInputField;
    public TMP_InputField worldnameInputField;
    public TMP_InputField worldseedInputField;
    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }
    public void CloseCharacterSelect()
    {
        characterSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        CharacterListUI listUI = characterSelectPanel.GetComponent<CharacterListUI>();
        listUI.ClosePanel();
    }
    public void ShowCharacterCreate()
    {
        characterSelectPanel.SetActive(false);
        characterSelectPanel.transform.Find("NextButton").gameObject.SetActive(false);
        CharacterListUI listUI = characterSelectPanel.GetComponent<CharacterListUI>();
        listUI.ClosePanel();
        characterCreatePanel.SetActive(true);
    }
    public void CloseCharacterCreate()
    {
        characterCreatePanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }
    public void SaveCharacter()
    {
        string characterName = nicknameInputField.text.Trim();

        characterName = string.IsNullOrEmpty(characterName) ?
            "MyPlayer" :
            characterName;

        nicknameInputField.text = "";

        CharacterData data = CharacterSaver.LoadAllCharacters();

        string finalName = characterName;
        int counter = 1;
        while (data.characters.Exists(c => c.name == finalName))
        {
            finalName = $"{characterName} ({counter})";
            counter++;
        }

        Character newCharacter = new Character { name = finalName, curKilledMobs = 0 };
        CharacterSaver.SaveCharacter(newCharacter);

        characterCreatePanel.SetActive(false);
        characterSelectPanel.SetActive(true);

        CharacterListUI listUI = characterSelectPanel.GetComponent<CharacterListUI>();
        listUI.LoadCharactersToList();
    }
    public void ShowWorldSelect()
    {
        characterSelectPanel.SetActive(false);
        worldSelectPanel.SetActive(true);
    }
    public void CloseWorldSelect()
    {
        worldSelectPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
        
        WorldListUI listUI = worldSelectPanel.GetComponent<WorldListUI>();
        listUI.ClosePanel();
    }
    public void ShowWorldCreate()
    {
        worldSelectPanel.SetActive(false);
        worldSelectPanel.transform.Find("NextButton").gameObject.SetActive(false);
        WorldListUI listUI = worldSelectPanel.GetComponent<WorldListUI>();
        listUI.ClosePanel();
        worldCreatePanel.SetActive(true);
    }
    public void CloseWorldCreate()
    {
        worldCreatePanel.SetActive(false);
        worldSelectPanel.SetActive(true);
    }
    public void SaveWorld()
    {
        string worldName = worldnameInputField.text.Trim();
        string worldSeed = worldseedInputField.text.Trim();

        worldName = string.IsNullOrEmpty(worldName) ?
            "MyWorld" :
            worldName;
        worldSeed = string.IsNullOrEmpty(worldSeed) ?
            new string(Enumerable.Repeat(0, 12).Select(_ => (char)(new System.Random().Next(10) + '0')).ToArray()) :
            worldSeed;

        WorldData data = WorldSaver.LoadAllWorlds();

        string finalName = worldName;
        int counter = 1;
        while (data.worlds.Exists(c => c.name == finalName))
        {
            finalName = $"{worldName} ({counter})";
            counter++;
        }

        World newWorld = new World
        {
            name = finalName,
            seed = worldSeed.Length > 12 ? worldSeed.Substring(0, 12) : worldSeed,
            curLvl = 1,
            openedGates = new List<Gate>()
        };
        
        worldnameInputField.text = "";
        worldseedInputField.text = "";

        // World data = CharacterSaver.LoadCharacter(worldName);
        // Пока не нужен, но потом добавить нового перса с таким именем (+ число копий)

        WorldSaver.SaveWorld(newWorld);

        worldCreatePanel.SetActive(false);
        worldSelectPanel.SetActive(true);

        WorldListUI listUI = worldSelectPanel.GetComponent<WorldListUI>();
        if (listUI != null)
        {
            listUI.SendMessage("LoadWorldsToList");
        }
    }
    public void StartGameWithWorld()
    {
        World world = WorldListUI.world;
        Character player = CharacterListUI.character;
        SceneController.Instance.GameScene(world, player);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Приложение закрыто");
    }
}
