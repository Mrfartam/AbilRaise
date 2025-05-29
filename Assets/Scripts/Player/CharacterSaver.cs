using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharacterSaver : MonoBehaviour
{
    static private string savePath => Path.Combine(Application.persistentDataPath, "characters.json");
    static public void SaveCharacter(Character character)
    {
        CharacterData data = LoadAllCharacters();

        int index = data.characters.FindIndex(w => w.name == character.name);
        if (index >= 0)
        {
            data.characters[index] = character;
        }
        else
        {
            data.characters.Add(character);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Персонаж \"{character.name}\" сохранён.");
    }

    static public Character LoadCharacter(string name)
    {
        CharacterData data = LoadAllCharacters();

        Character result = data.characters.Find(w => w.name == name);
        if (result == null)
            Debug.LogWarning($"Персонаж с именем \"{name}\" не найден.");

        return result;
    }
    static public CharacterData LoadAllCharacters()
    {
        if (!File.Exists(savePath))
        {
            return new CharacterData();
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<CharacterData>(json);
    }
    static public void DeleteCharacter(string name)
    {
        CharacterData data = LoadAllCharacters();

        int index = data.characters.FindIndex(c => c.name == name);
        if (index >= 0)
        {
            data.characters.RemoveAt(index);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Персонаж \"{name}\" удалён.");
        }
        else
        {
            Debug.LogWarning($"Персонаж \"{name}\" не найден. Удаление невозможно.");
        }
    }
}

[Serializable]
public class Character
{
    public string name;
    public int curKilledMobs;
}

[Serializable]
public class CharacterData
{
    public List<Character> characters = new List<Character>();
}