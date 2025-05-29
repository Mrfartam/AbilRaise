using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject tilePrefab; //Префаб плитки
    public GameObject wallPrefab; //Префаб стены
    public GameObject warfogPrefab; //Префаб тумана войны
    public GameObject roomPrefab; //Префаб комнаты
    public GameObject spawnerPrefab; //Префаб спавнера
    public GameObject containerPrefab; //Префаб контейнера (для чего угодно без спрайтов и хитбоксов)
    public GameObject portalPrefab; //Префаб портала

    [Header("Спрайты")]
    public Sprite[] tileSprites; //Массив спрайтов пола
    public Sprite[] wallSprites; //Массив спрайтов стен
    public Sprite[] gateSprites; //Массив спрайтов ворот
    public Sprite warfogSprite; //Спрайт тумана войны

    private Dictionary<int, GameObject> warfog; //Словарь с элементами тумана войны, соответствующими данной комнате
    private Dictionary<int, GameObject> rooms; //Словарь с объектом комнаты, соответствующим данному номеру
    private Dictionary<int, Dictionary<int, GameObject>> gates; //Словарь с объектами ворот, соответствующими данной комнате
    private Dictionary<int, List<GameObject>> collisionWarfog; //Словарь с объектами пересечений варфогов, соответствующих данной комнате
    private int roomWidth = 25; //Ширина комнаты
    private int roomHeight = 29; //Высота комнаты (нужны были для разного размера комнат, но сейчас не актуальны)
    public void BuildPortal(int num)
    {
        GameObject room = rooms[num];
        Instantiate(portalPrefab, room.transform.position, Quaternion.identity, room.transform);
    }
    public void DestroyLevel()
    {
        if (rooms == null) return;

        foreach (var room in rooms)
        {
            if (room.Value != null)
            {
                Destroy(room.Value);
            }
        }
        foreach (var fog in warfog)
        {
            if (fog.Value != null)
                Destroy(fog.Value);
        }
        foreach (var gateDict in gates)
        {
            foreach (var gate in gateDict.Value)
            {
                if (gate.Value != null)
                    Destroy(gate.Value);
            }
        }

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        warfog?.Clear();
        rooms?.Clear();
        gates?.Clear();

        warfog = null;
        rooms = null;
        gates = null;
    }
    public GameObject GetNumRoom(int num)
    {
        return rooms[num];
    }
    public Dictionary<int, GameObject> GetNumRooms()
    {
        return rooms;
    }
    public GameObject GetWarFog(int num)
    {
        return warfog[num];
    }
    public Dictionary<int, GameObject> GetGates(int num)
    {
        return gates[num];
    }
    public List<GameObject> GetWarFogCollision(int num)
    {
        return collisionWarfog[num];
    }
    private void CreateTile(Vector3 position, Transform parent)
    {
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, parent);
        tile.GetComponent<SpriteRenderer>().sprite = tileSprites[UnityEngine.Random.Range(0, tileSprites.Length)];
    }
    private void CreateWall(Vector3 position, Transform parent)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, parent);
        wall.GetComponent<SpriteRenderer>().sprite = wallSprites[UnityEngine.Random.Range(0, wallSprites.Length)];
        wall.GetComponent<SpriteRenderer>().sortingOrder = (int) -position.y;
    }
    private GameObject CreateWarFog(Vector3 position, Transform parent)
    {
        GameObject tile = Instantiate(warfogPrefab, position, Quaternion.identity, parent);
        tile.GetComponent<SpriteRenderer>().sprite = warfogSprite;
        return tile;
    }
    private void CreateGate(Vector3 position, Transform parent)
    {
        GameObject gate = Instantiate(wallPrefab, position, Quaternion.identity, parent);
        gate.GetComponent<SpriteRenderer>().sprite = gateSprites[UnityEngine.Random.Range(0, gateSprites.Length)];
        gate.GetComponent<SpriteRenderer>().sortingOrder = (int) -position.y;
    }
    private void AddCollisionWarFog(int curRoom, int neightbourRoom, GameObject tile)
    {
        collisionWarfog[curRoom] ??= new List<GameObject>();
        collisionWarfog[curRoom].Add(tile);

        collisionWarfog[neightbourRoom] ??= new List<GameObject>();
        collisionWarfog[neightbourRoom].Add(tile);
    }
    public void GenerateRoom(int numOfRoom) //Генерация комнаты
    {
        int xCenter = 0, yCenter = 0;
        Calculations.CalcCoord(numOfRoom, ref xCenter, ref yCenter);
        Vector3 roomPosition = new(xCenter, yCenter, 0);

        GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity, transform);
        RoomController roomController = room.GetComponent<RoomController>();
        roomController.num = numOfRoom;
        roomController.player = GameObject.Find("Player");
        roomController.curKilledEnemy = numOfRoom != 0 ? 0 : 60;
        roomController.timeOfOpen = 1;

        collisionWarfog ??= new Dictionary<int, List<GameObject>>();
        collisionWarfog[numOfRoom] = new List<GameObject>();

        GameObject spawner = Instantiate(spawnerPrefab, roomPosition, Quaternion.identity, room.transform);
        spawner.GetComponent<Spawn>().numRoom = numOfRoom;

        rooms ??= new Dictionary<int, GameObject>();
        rooms[numOfRoom] = room;
        
        for (int y = 0; y <= roomHeight / 2; y++)
        {
            Vector3 tilePosition;

            for (int x = 0; (x <= roomWidth / 2) && y <= 7 ||
                (x <= roomWidth / 2 - 1) && y == 8 ||
                (x <= roomWidth / 2 - 3) && y == 9 ||
                (x <= roomWidth / 2 - 4) && y == 10 ||
                (x <= roomWidth / 2 - 6) && y == 11 ||
                (x <= roomWidth / 2 - 8) && y == 12 ||
                (x <= roomWidth / 2 - 9) && y == 13 ||
                (x <= roomWidth / 2 - 11) && y == 14; x++)
            {
                if (y == 0)
                {
                    //Создание центральной плитки
                    if (x == 0)
                    {
                        //Центральная плитка
                        tilePosition = new(xCenter, yCenter, 0);
                        CreateTile(tilePosition, room.transform);
                    }
                    //Создание плиток центральной полосы
                    else
                    {
                        //Правая плитка
                        tilePosition = new(xCenter + x, yCenter, 0);
                        CreateTile(tilePosition, room.transform);

                        //Левая плитка
                        tilePosition = new(xCenter - x, yCenter, 0);
                        CreateTile(tilePosition, room.transform);
                    }
                }
                else
                {
                    //Создание плиток центрального столбца
                    if (x == 0)
                    {
                        //Верхняя плитка
                        tilePosition = new(xCenter, yCenter + y, 0);
                        CreateTile(tilePosition, room.transform);

                        //Нижняя плитка
                        tilePosition = new(xCenter, yCenter - y, 0);
                        CreateTile(tilePosition, room.transform);
                    }
                    //Создание остальных плиток
                    else
                    {
                        //Правая верхняя плитка
                        tilePosition = new(xCenter + x, yCenter + y, 0);
                        CreateTile(tilePosition, room.transform);

                        //Правая нижняя плитка
                        tilePosition = new(xCenter + x, yCenter - y, 0);
                        CreateTile(tilePosition, room.transform);

                        //Левая верхняя плитка
                        tilePosition = new(xCenter - x, yCenter + y, 0);
                        CreateTile(tilePosition, room.transform);

                        //Левая нижняя плитка
                        tilePosition = new(xCenter - x, yCenter - y, 0);
                        CreateTile(tilePosition, room.transform);
                    }
                }
            }
        }

        //Длины рёбер
        int[] xCount = { 1, 2, 1, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1};

        Vector3 wallPosition;

        //Верхняя центральная стена
        wallPosition = new(xCenter, yCenter + 15, 0);
        CreateWall(wallPosition, room.transform);

        //Все остальные
        int xOffset = 1;
        for (int y = 15; y >= -15; y--)
        {
            if (y > 0)
            {
                for (int k = 0; k < xCount[15 - y]; k++)
                {
                    //Правая стена
                    wallPosition = new(xCenter + xOffset, yCenter + y, 0);
                    CreateWall(wallPosition, room.transform);

                    //Левая стена
                    wallPosition = new(xCenter - xOffset, yCenter + y, 0);
                    CreateWall(wallPosition, room.transform);

                    if (xOffset < 13)
                        xOffset ++;
                }
                if (y == 13)
                {
                    y -= 2;
                    xOffset += 4;
                }
                else if (y == 2)
                    y -= 3;
            }
            /*else if (y == 0)
            {
                //Правая стена
                wallPosition = new(xCenter + xOffset, yCenter, 0);
                wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);

                randomWall = wallSprites[Random.Range(0, wallSprites.Length)];

                wall.GetComponent<SpriteRenderer>().sprite = randomWall;
                wall.GetComponent<SpriteRenderer>().sortingOrder = -yCenter;

                //Левая стена
                wallPosition = new(xCenter - xOffset, yCenter, 0);
                wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);

                randomWall = wallSprites[Random.Range(0, wallSprites.Length)];

                wall.GetComponent<SpriteRenderer>().sprite = randomWall;
                wall.GetComponent<SpriteRenderer>().sortingOrder = -yCenter;
            }*/
            else
            {
                for (int k = 0; k < xCount[15 + y]; k++)
                {
                    //Правая стена
                    wallPosition = new(xCenter + xOffset, yCenter + y, 0);
                    CreateWall(wallPosition, room.transform);

                    //Левая стена
                    wallPosition = new(xCenter - xOffset, yCenter + y, 0);
                    CreateWall(wallPosition, room.transform);
                    
                    if (y <= -7)
                        xOffset--;
                }
                if (y == -10)
                {
                    y -= 2;
                    xOffset -= 4;
                }
            }
        }

        //Нижняя центральная стена
        wallPosition = new(xCenter, yCenter - 15, 0);
        CreateWall(wallPosition, room.transform);
    }
    public void GreateWarFog(Dictionary<int, List<int>> gates, List<int> orderOfNums) // Генерация тумана войны
    {
        warfog = new Dictionary<int, GameObject>();
        foreach (int num in gates.Keys)
        {
            if (num == 0) continue;

            GameObject roomWarfog = Instantiate(containerPrefab, rooms[num].transform.position, Quaternion.identity, rooms[num].transform);
            warfog[num] = roomWarfog;

            int xCenter = 0, yCenter = 0;
            Calculations.CalcCoord(num, ref xCenter, ref yCenter);

            Vector3 warfogPosition;
            GameObject tile;

            // Буквально копипаст кладки плиток, но вместо них туман войны
            for (int y = 0; y <= roomHeight / 2; y++)
            {
                for (int x = 0; (x <= roomWidth / 2) && y <= 7 ||
                    (x <= roomWidth / 2 - 1) && y == 8 ||
                    (x <= roomWidth / 2 - 3) && y == 9 ||
                    (x <= roomWidth / 2 - 4) && y == 10 ||
                    (x <= roomWidth / 2 - 6) && y == 11 ||
                    (x <= roomWidth / 2 - 8) && y == 12 ||
                    (x <= roomWidth / 2 - 9) && y == 13 ||
                    (x <= roomWidth / 2 - 11) && y == 14; x++)
                {
                    if (y == 0)
                    {
                        //Создание центральной плитки
                        if (x == 0)
                        {
                            //Центральная плитка
                            warfogPosition = new(xCenter, yCenter, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                        }
                        //Создание плиток центральной полосы
                        else
                        {
                            //Правая плитка
                            warfogPosition = new(xCenter + x, yCenter, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            //Левая плитка
                            warfogPosition = new(xCenter - x, yCenter, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                        }
                    }
                    else
                    {
                        //Создание плиток центрального столбца
                        if (x == 0)
                        {
                            //Верхняя плитка
                            warfogPosition = new(xCenter, yCenter + y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            //Нижняя плитка
                            warfogPosition = new(xCenter, yCenter - y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                            if (y == 14)
                            {
                                tile.transform.localScale = new(1, (float)1/2, 1);
                                tile.transform.position = new(xCenter, yCenter - y + (float)1 / 4, 0);
                            }

                        }
                        //Создание остальных плиток
                        else
                        {
                            //Правая верхняя плитка
                            warfogPosition = new(xCenter + x, yCenter + y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            //Правая нижняя плитка
                            warfogPosition = new(xCenter + x, yCenter - y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            if ((x == roomWidth / 2) && y == 7 || 
                                (x == roomWidth / 2 - 1) && y == 8 ||
                                (x == roomWidth / 2 - 2) && y == 8 ||
                                (x == roomWidth / 2 - 3) && y == 9 ||
                                (x == roomWidth / 2 - 4) && y == 10 ||
                                (x == roomWidth / 2 - 5) && y == 10 ||
                                (x == roomWidth / 2 - 6) && y == 11 ||
                                (x == roomWidth / 2 - 7) && y == 11 ||
                                (x == roomWidth / 2 - 8) && y == 12 ||
                                (x == roomWidth / 2 - 9) && y == 13 ||
                                (x == roomWidth / 2 - 10) && y == 13 ||
                                (x == roomWidth / 2 - 11) && y == 14)
                            {
                                tile.transform.position = new(xCenter + x, yCenter - y + (float)1/4, 0);
                                tile.transform.localScale = new(1, (float)1/2, 1);
                            }

                            //Левая верхняя плитка
                            warfogPosition = new(xCenter - x, yCenter + y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            //Левая нижняя плитка
                            warfogPosition = new(xCenter - x, yCenter - y, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);

                            if ((x == roomWidth / 2) && y == 7 ||
                                (x == roomWidth / 2 - 1) && y == 8 ||
                                (x == roomWidth / 2 - 2) && y == 8 ||
                                (x == roomWidth / 2 - 3) && y == 9 ||
                                (x == roomWidth / 2 - 4) && y == 10 ||
                                (x == roomWidth / 2 - 5) && y == 10 ||
                                (x == roomWidth / 2 - 6) && y == 11 ||
                                (x == roomWidth / 2 - 7) && y == 11 ||
                                (x == roomWidth / 2 - 8) && y == 12 ||
                                (x == roomWidth / 2 - 9) && y == 13 ||
                                (x == roomWidth / 2 - 10) && y == 13 ||
                                (x == roomWidth / 2 - 11) && y == 14)
                            {
                                tile.transform.position = new(xCenter - x, yCenter - y + (float)1 / 4, 0);
                                tile.transform.localScale = new(1, (float)1 / 2, 1);
                            }
                        }
                    }
                }
            }

            for (int k = 0; k <= 6; k++)
            {
                int neighbour = Calculations.CalcNumOfNeighbour(num, k);
                
                if (neighbour == 0)
                    continue;

                bool isCollision = orderOfNums.Contains(neighbour);

                switch (k)
                {
                    case 0:
                        for (int y = 0; y <= 7; y++)
                        {
                            int x = 13;
                            warfogPosition = new(xCenter + x, yCenter + y + (float)1/4, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                            tile.transform.localScale = new(1, (float)3/2, 1);
                            if(isCollision) AddCollisionWarFog(num, neighbour, tile);

                            
                            if (y < 7 || !orderOfNums.Contains(Calculations.CalcNumOfNeighbour(num, 5)))
                            {
                                warfogPosition = new(xCenter + x, yCenter - y + (float)1/4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x, yCenter - y + (float)1 / 2, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);
                            }
                        }
                        
                        break;

                    case 1:
                        for(float y = (float)23.0 / 2, d = (float)1.0 /2, d2 = (float)1.0/2, x = (float)13.0 / 2; d <= (float)7.0 / 2; d++)
                        {
                            if((y + d) % 2 == 0)
                            {
                                warfogPosition = new(xCenter + x - d2, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2 - 1, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2 + 1, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 2;
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x - d2, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 1;
                            }
                        }

                        break;

                    case 2:
                        for (float y = (float)23.0 / 2, d = (float)1.0 / 2, d2 = (float)1.0 / 2, x = (float)-13.0 / 2; d <= (float)7.0 / 2; d++)
                        {
                            if ((y + d) % 2 == 0)
                            {
                                warfogPosition = new(xCenter + x + d2, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2 + 1, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2 - 1, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 2;
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x + d2, yCenter + y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2, yCenter + y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 1;
                            }
                        }

                        break;

                    case 3:
                        for (int y = 0; y <= 7; y++)
                        {
                            int x = -13;
                            warfogPosition = new(xCenter + x, yCenter + y + (float)1 / 4, 0);
                            tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                            tile.transform.localScale = new(1, (float)3 / 2, 1);
                            if (isCollision) AddCollisionWarFog(num, neighbour, tile);


                            if (y < 7 || !orderOfNums.Contains(Calculations.CalcNumOfNeighbour(num, 4)))
                            {
                                warfogPosition = new(xCenter + x, yCenter - y + (float)1 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x, yCenter - y + (float)1 / 2, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);
                            }
                        }

                        break;

                    case 4:
                        for (float y = (float)23.0 / 2, d = (float)1.0 / 2, d2 = (float)1.0 / 2, x = (float)-13.0 / 2; d <= (float)7.0 / 2; d++)
                        {
                            if ((y + d) % 2 == 0)
                            {
                                warfogPosition = new(xCenter + x - d2, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2 - 1, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2 + 1, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 2;
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x - d2, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 1;
                            }
                        }

                        break;
                    
                    case 5:
                        for (float y = (float)23.0 / 2, d = (float)1.0 / 2, d2 = (float)1.0 / 2, x = (float)13.0 / 2; d <= (float)7.0 / 2; d++)
                        {
                            if ((y + d) % 2 == 0)
                            {
                                warfogPosition = new(xCenter + x + d2, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x + d2 + 1, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2 - 1, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 2;
                            }
                            else
                            {
                                warfogPosition = new(xCenter + x + d2, yCenter - y + d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                warfogPosition = new(xCenter + x - d2, yCenter - y - d + (float)1.0 / 4, 0);
                                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                                tile.transform.localScale = new(1, (float)3 / 2, 1);
                                if (isCollision) AddCollisionWarFog(num, neighbour, tile);

                                d2 += 1;
                            }
                        }

                        break;
                }
                
                warfogPosition = new(xCenter, yCenter + 15 + (float)1.0 / 4, 0);
                tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                tile.transform.localScale = new(1, (float)3 / 2, 1);
                
                if (!orderOfNums.Contains(Calculations.CalcNumOfNeighbour(num, 4)) &&
                    !orderOfNums.Contains(Calculations.CalcNumOfNeighbour(num, 5)))
                {
                    warfogPosition = new(xCenter, yCenter - 15 + (float)1.0 / 4, 0);
                    tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                    tile.transform.localScale = new(1, (float)3 / 2, 1);
                }
                else
                {
                    warfogPosition = new(xCenter, yCenter - (float)29.0 / 2, 0);
                    tile = CreateWarFog(warfogPosition, roomWarfog.transform);
                }
            }
        }
    }
    public void BuildGates(Dictionary<int, List<int>> gates, List<int> nums) // Создание ворот
    {
        this.gates = new Dictionary<int, Dictionary<int, GameObject>>();

        foreach (int num in gates.Keys)
        {
            //Вывод комнат и соответствующих им ворот
            Debug.Log($"{num}: " + string.Join(", ", gates[num]));
            if(!this.gates.ContainsKey(num))
                this.gates[num] = new Dictionary<int, GameObject>();
            for(int i = 0; i < 6; i++)
            {
                int neighbour = Calculations.CalcNumOfNeighbour(num, i);

                if (!nums.Contains(neighbour))
                {
                    BuildWall(num, i);
                    continue;
                }

                if (!this.gates.ContainsKey(neighbour) && neighbour != 0)
                    this.gates[neighbour] = new Dictionary<int, GameObject>();

                if (gates[num].IndexOf(i) != -1)
                {
                    if (neighbour != 0 && !this.gates[neighbour].ContainsKey((i + 3) % 6))
                    {
                        GameObject gate = BuildGate(num, i);
                        this.gates[num][i] = gate;
                        this.gates[neighbour][(i + 3) % 6] = gate;
                    }
                }
                else if (gates[num].IndexOf(i) == -1)
                    BuildWall(num, i);
            }
        }
    }
    private GameObject BuildGate(int num, int k) // Собственно, генерация ворот
    {
        Vector3 gatePosition;

        int x = 0, y = 0;
        Calculations.CalcCoord(num, ref x, ref y);
        GameObject gate = Instantiate(containerPrefab, new Vector3(x, y, 0), Quaternion.identity, rooms[num].transform);
        switch (k)
        {
            case 0:
                {
                    gatePosition = new(x + 13, y + 1, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 13, y, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 13, y - 1, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
            case 1:
                {
                    gatePosition = new(x + 5, y + 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 6, y + 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 7, y + 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 8, y + 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
            case 2:
                {
                    gatePosition = new(x - 5, y + 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 6, y + 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 7, y + 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 8, y + 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
            case 3:
                {
                    gatePosition = new(x - 13, y + 1, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 13, y, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 13, y - 1, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
            case 4:
                {
                    gatePosition = new(x - 5, y - 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 6, y - 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 7, y - 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x - 8, y - 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
            case 5:
                {
                    gatePosition = new(x + 5, y - 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 6, y - 12, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 7, y - 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    gatePosition = new(x + 8, y - 11, 0);
                    CreateGate(gatePosition, gate.transform);
                    CreateTile(gatePosition, rooms[num].transform);

                    break;
                }
        }
        return gate;
    }
    private void BuildWall(int num, int k) // Генерация стен
    {
        Vector3 wallPosition;

        int x = 0, y = 0;
        Calculations.CalcCoord(num, ref x, ref y);
        switch (k)
        {
            case 0:
                {
                    wallPosition = new(x + 13, y, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 13, y - 1, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 13, y + 1, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
            case 1:
                {
                    wallPosition = new(x + 5, y + 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 6, y + 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 7, y + 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 8, y + 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
            case 2:
                {
                    wallPosition = new(x - 5, y + 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 6, y + 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 7, y + 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 8, y + 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
            case 3:
                {
                    wallPosition = new(x - 13, y, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 13, y + 1, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 13, y - 1, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
            case 4:
                {
                    wallPosition = new(x - 5, y - 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 6, y - 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 7, y - 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x - 8, y - 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
            case 5:
                {
                    wallPosition = new(x + 5, y - 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 6, y - 12, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 7, y - 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);

                    wallPosition = new(x + 8, y - 11, 0);
                    CreateWall(wallPosition, rooms[num].transform);
                    break;
                }
        }
    }
}
