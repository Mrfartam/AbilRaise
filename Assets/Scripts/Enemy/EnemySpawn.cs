using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject enemyPrefab; //Префаб
    public GameObject player; //Игрок
    public float cooldownSpawn; //Кулдаун спавна
    public float minSpawnDistance; //Минимальное расстояние до игрока
    public int maxEnemies; //Максимальное количество врагов
    public Vector2 spawnAreaSize; //Область спавна
    public int numRoom; //Номер текущей комнаты

    private int curEnemyCount = 0; //Текущее количество врагов на сцене

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), cooldownSpawn, cooldownSpawn);
        player = GameObject.Find("Player");
        cooldownSpawn = 2f;
        minSpawnDistance = 2f;
        maxEnemies = 10;
        spawnAreaSize = new Vector2(18f, 18f);
    }

    void SpawnEnemy()
    {
        if (curEnemyCount >= maxEnemies) return;
        if (numRoom != player.GetComponent<Player>().curRoom ||
            numRoom == FindObjectOfType<RoomsGenerator>().GetNumRooms().Last().Key ||
            numRoom == FindObjectOfType<RoomsGenerator>().GetNumRooms().First().Key) return;
        Vector3 roomPos = transform.position;
        

        Vector2 spawnPos;
        float distanceToPlayer;

        do
        {
            //Генерация случайной позиции
            spawnPos = new Vector2(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2));
            distanceToPlayer = Vector2.Distance(roomPos, roomPos + (Vector3)spawnPos);
        }
        while (distanceToPlayer < minSpawnDistance);

        //Спавн врага
        GameObject newEnemy = Instantiate(enemyPrefab, roomPos + (Vector3)spawnPos, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().curRoom = numRoom;

        float randomScale = Random.Range(0.75f, 1.25f);
        newEnemy.GetComponent<CapsuleCollider2D>().offset = new(0, newEnemy.GetComponent<CapsuleCollider2D>().size.y / 2);
        newEnemy.transform.localScale = new Vector3 (randomScale, randomScale, 1f);
        curEnemyCount++;
    }

    public void OnEnemyDestroyed()
    {
        curEnemyCount--;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
