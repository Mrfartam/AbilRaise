using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 1;
    public int kickPower;
    public bool isUltimative;
    private Player player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        kickPower = 1;
    }
    private void Update()
    {
        if(GameObject.Find("Sword").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            isUltimative = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(enabled == true)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Коллизия с врагом произошла!");
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    if (enemy.health > 1)
                        StartCoroutine(enemy?.GetComponent<EnemyMotion>().KickOfEnemy(kickPower));
                    if (isUltimative)
                    {
                        Debug.Log("Ульта!");
                        enemy.SetEffect(player.curAbility);
                    }
                    enemy.TakeDamage(damage);
                    if (enemy.health <= 0)
                    {
                        Debug.Log("Враг убит!");
                        Character character = CharacterSaver.LoadCharacter(player.nickname);
                        character.curKilledMobs = ++player.numOfKilledEnemy;
                        CharacterSaver.SaveCharacter(character);

                        FindObjectOfType<RoomsGenerator>().GetNumRoom(player.curRoom).transform.Find("SpawnerPrefab(Clone)").GetComponent<Spawn>().OnEnemyDestroyed();
                        FindObjectOfType<FillingAbilityButton>().IncreaseValue(10);
                        FindObjectOfType<RoomsGenerator>().GetNumRoom(player.curRoom).GetComponent<RoomController>().curKilledEnemy++;
                    }
                }
            }
        }
    }
}
