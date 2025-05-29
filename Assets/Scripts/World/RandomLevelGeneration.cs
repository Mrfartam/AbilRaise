using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    private int count_rooms = 1;
    private int num_of_room = 1;
    private int x;
    private int y;
    private int need_new_room;
    public List<int> nums_of_rooms;
    public RoomsGenerator roomGenerator;
    void Start()
    {
        roomGenerator.GenerateRoom(0);
        need_new_room = UnityEngine.Random.Range(0, 10);
        while (count_rooms < 4 || need_new_room < 5)
        {
            if(need_new_room < 5)
            {
                Calculations.CalcCoord(num_of_room, ref x, ref y);
                roomGenerator.GenerateRoom(num_of_room);
                count_rooms++;
                nums_of_rooms.Add(num_of_room);
            }
            num_of_room++;
            need_new_room = UnityEngine.Random.Range(0, 10);
        }
        foreach (int i in nums_of_rooms)
        {
            Debug.Log(i);
        }
    }
}
