using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int num; // ����� ������ �������
    public bool isOpen; // ����, ������� �� ��� ��� ������
    public GameObject player; // ������ �� ������
    public int curKilledEnemy; // ������� ���������� ������ ������
    public int timeOfOpen; // ���������� ���, ����� ����������� ������
    
    private BoxCollider2D playerCollider; // ��������� ������
    private PolygonCollider2D roomCollider; // ��������� �������
    private RoomsGenerator roomsGenerator; // ��������� �������
    private ProceduralLevelGenerator levelGenerator; // ��������� ������
    
    public void Start()
    {
        playerCollider = player.GetComponent<BoxCollider2D>();
        roomCollider = GetComponent<PolygonCollider2D>();
        roomsGenerator = FindObjectOfType<RoomsGenerator>();
        levelGenerator = FindObjectOfType<ProceduralLevelGenerator>();
    }
    public void Update()
    {
        if (curKilledEnemy >= 10 * timeOfOpen)
        {
            Dictionary<int, GameObject> gates = roomsGenerator.GetGates(num);

            // �������� ����� ��������� �������� �������
            System.Random rand = new System.Random();
            if (gates.Count > 0)
            {
                int dir, neight;

                dir = gates.Keys.ElementAt(rand.Next(0, gates.Count));
                neight = Calculations.CalcNumOfNeighbour(num, dir);

                if(neight == 0 && gates.Count > 0)
                {
                    gates.Remove(dir);
                    dir = gates.Keys.ElementAt(rand.Next(0, gates.Count));
                    neight = Calculations.CalcNumOfNeighbour(num, dir);
                }

                OpenRoom(neight);
                Destroy(gates[dir]);
                gates.Remove(dir);
                roomsGenerator.GetGates(neight).Remove((dir + 3) % 6);
                timeOfOpen++;

                World world = WorldSaver.LoadWorld(levelGenerator.worldName);
                if (!world.openedGates.Exists(g => g.from == num && g.to == neight))
                    world.openedGates.Add(new Gate(num, neight, dir));
                WorldSaver.SaveWorld(world);
            }

            // �������� ���� ����� �������
            /*
            foreach(int dir in gates.Keys)
            {
                OpenRoom(Calculations.CalcNumOfNeighbour(num, dir));
                if(gates[dir] != null)
                    Destroy(gates[dir]);
            }*/
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<Player>().curRoom = num;
        }
    }
    public void OpenRoom(int n)
    {
        GameObject warfog = roomsGenerator.GetWarFog(n);
        if (warfog != null)
            Destroy(warfog);
        List<GameObject> warfogCollision = roomsGenerator.GetWarFogCollision(n);
        foreach (var tile in warfogCollision)
            if (tile != null) Destroy(tile);
        warfogCollision.Clear();
    }
}
