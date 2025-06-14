using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 10;
    public float speed;
    public int curLvl;
    public int curRoom;
    private Ability effect;
    public GameObject spriteObject;

    public void Start()
    {
        effect = new Ability("no", 0, 0);
    }

    private void Update()
    {
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        if (effect.name != "no")
        {
            CheckEndedEffects();
            if (!effect.isStarted)
            {
                StartCoroutine(TakeEffect());
            }
        }
    }
    private void CheckEndedEffects()
    {
        if (effect.isEnded)
            effect = new Ability("no", 0, 0);
    }
    IEnumerator TakeEffect()
    {
        Transform sprite = transform.Find("SpriteObject");
        sprite.GetComponent<ParticleSystem>().Play();
        effect.isStarted = true;
        while (effect.curTime < effect.maxTime)
        {
            TakeDamage(effect.damage);
            effect.curTime += Time.deltaTime;
            yield return new WaitForSeconds(1);
            effect.curTime += 1;
            
        }
        if (effect.curTime >= effect.maxTime)
        {
            effect.isEnded = true;
            sprite.GetComponent<ParticleSystem>().Stop();
        }
    }
    public void TakeDamage(int damage)
    {
        StartCoroutine(spriteObject.GetComponent<EnemyRendering>().FlashRed());
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Враг разрушен!");
            FindObjectOfType<Spawn>().OnEnemyDestroyed();
        }
    }
    public void SetEffect(Ability eff)
    {
        effect = eff;
    }
}