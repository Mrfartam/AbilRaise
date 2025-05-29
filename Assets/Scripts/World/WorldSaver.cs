using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldSaver : MonoBehaviour
{
    static private string savePath => Path.Combine(Application.persistentDataPath, "worlds.json");
    static public void SaveWorld(World world)
    {
        WorldData data = LoadAllWorlds();

        int index = data.worlds.FindIndex(w => w.name == world.name);
        if (index >= 0)
        {
            data.worlds[index] = world;
        }
        else
        {
            data.worlds.Add(world);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Мир \"{world.name}\" сохранён.");
    }
    static public World LoadWorld(string name)
    {
        WorldData data = LoadAllWorlds();

        World result = data.worlds.Find(w => w.name == name);
        if (result == null)
            Debug.LogWarning($"Мир с именем \"{name}\" не найден.");

        return result;
    }
    static public WorldData LoadAllWorlds()
    {
        if (!File.Exists(savePath))
        {
            return new WorldData();
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<WorldData>(json);
    }
    static public void DeleteWorld(string name)
    {
        WorldData data = LoadAllWorlds();

        int index = data.worlds.FindIndex(c => c.name == name);
        if (index >= 0)
        {
            data.worlds.RemoveAt(index);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Мир \"{name}\" удалён.");
        }
        else
        {
            Debug.LogWarning($"Мир \"{name}\" не найден. Удаление невозможно.");
        }
    }
}

[Serializable]
public class World
{
    public string name;
    public string seed;
    public int curLvl;
    public List<Gate> openedGates;
}

[Serializable]
public class Gate
{
    public int from;
    public int to;
    public int dir;

    public Gate(int from, int to, int dir)
    {
        this.from = from;
        this.to = to;
        this.dir = dir;
    }
}

[Serializable]
public class WorldData
{
    public List<World> worlds = new List<World>();
}