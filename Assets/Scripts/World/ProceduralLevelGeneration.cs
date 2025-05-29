using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelGenerator : MonoBehaviour
{
    private int countRooms; // Текущее количество комнат
    private int numOfRoom; // Номер текущей генерируемой комнаты
    private int numOfRooms; // Общее количество необходимых комнат
    private int x, y; // Переменные для координат
    public int curLevel; // Текущий уровень игры
    public string seed; // Сид мира
    public string worldName; // Имя мира
    public List<int> numsOfRooms; // Список сгенерированных на уровне комнат
    public RoomsGenerator roomGenerator; // Объект со скриптом генератора комнаты
    private Dictionary<int, List<int>> gatesOfRoom; // Врата конкретной комнаты
    private List<int> pathOfGeneration; // Путь генерации в виде последовательности направлений

    public void StartGameBySave()
    {
        World world = WorldSaver.LoadWorld(worldName);
        curLevel = world.curLvl;
        seed = world.seed;
        StartGame();
        foreach (Gate gate in world.openedGates)
        {
            if (gate.from != 0)
            {
                RoomController room = roomGenerator.GetNumRoom(gate.from).GetComponent<RoomController>();
                room.timeOfOpen += 1;
                room.curKilledEnemy += 10;
                
                Destroy(roomGenerator.GetGates(gate.from)[gate.dir]);
                roomGenerator.GetGates(gate.from).Remove(gate.dir);
                roomGenerator.GetGates(gate.to).Remove((gate.dir + 3) % 6);

                GameObject warfog = roomGenerator.GetWarFog(gate.to);
                if (warfog != null)
                    Destroy(warfog);
                List<GameObject> warfogCollision = roomGenerator.GetWarFogCollision(gate.to);
                foreach (var tile in warfogCollision)
                    if (tile != null) Destroy(tile);
                warfogCollision.Clear();
            }
        }
    }

    public void StartGame()
    {
        countRooms = 1;
        numOfRoom = 0;
        numsOfRooms = new List<int>();
        pathOfGeneration = new List<int>();


        string levelSeed = Calculations.CalcNewSeed(seed, curLevel);

        numOfRooms = 4 + Calculations.CalcNumOfExtraRooms(levelSeed);

        roomGenerator.GenerateRoom(0);
        numsOfRooms.Add(0);

        int lenOfPart = levelSeed.Length / (numOfRooms - 1);
        while (countRooms < numOfRooms)
        {
            int direction = (Convert.ToInt16(levelSeed.Substring(lenOfPart * (countRooms - 1), lenOfPart))) % 6;
            pathOfGeneration.Add(direction);

            numOfRoom = Calculations.CalcNumOfNeighbour(numOfRoom, direction);
            while (numsOfRooms.IndexOf(numOfRoom) != -1)
            {
                numsOfRooms.Add(numOfRoom);
                pathOfGeneration.Add(direction);
                numOfRoom = Calculations.CalcNumOfNeighbour(numOfRoom, direction);
            }

            roomGenerator.GenerateRoom(numOfRoom);
            countRooms++;
            numsOfRooms.Add(numOfRoom);
        }

        /*foreach (int i in nums_of_rooms)
        {
            Debug.Log(i);
        }*/
        
        FindGates();
        roomGenerator.BuildGates(gatesOfRoom, numsOfRooms);
        roomGenerator.GreateWarFog(gatesOfRoom, numsOfRooms);
        roomGenerator.BuildPortal(numsOfRooms[^1]);
    }
    private void FindGates() // Функция обнаружения врат между комнатами
    {
        gatesOfRoom = new Dictionary<int, List<int>>();

        for(int i = 0; i < pathOfGeneration.Count; i++)
        {
            if (!gatesOfRoom.ContainsKey(numsOfRooms[i]))
                gatesOfRoom[numsOfRooms[i]] = new List<int>();
            if (!gatesOfRoom[numsOfRooms[i]].Contains(pathOfGeneration[i]))
                gatesOfRoom[numsOfRooms[i]].Add(pathOfGeneration[i]);

            if (!gatesOfRoom.ContainsKey(numsOfRooms[i + 1]))
                gatesOfRoom[numsOfRooms[i + 1]] = new List<int>();
            if (!gatesOfRoom[numsOfRooms[i + 1]].Contains((pathOfGeneration[i] + 3) % 6))
                gatesOfRoom[numsOfRooms[i + 1]].Add((pathOfGeneration[i] + 3) % 6);
        }

        foreach(var el in gatesOfRoom)
        {
            el.Value.Sort();
        }
    }
}
