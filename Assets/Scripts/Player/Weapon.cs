using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float dirX, dirY;
    public float offset;
    public Joystick joystick;
    public Animator animator;
    private float rotateWeapon;
    private int swordDirection;
    private float cooldownAttack;
    private Player player;

    void Start()
    {
        offset = -45;
        swordDirection = 1;
        cooldownAttack = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (cooldownAttack > 0)
            cooldownAttack -= Time.deltaTime;

        // Фрагмент с направлением оружия на врага.
        // Пока выглядит некрасиво, поэтому не используется

        /*Enemy[] enemies = player.curRoom != 0 ?
            GameObject.Find("RoomGenerator").
            GetComponent<RoomsGenerator>().
            GetNumRoom(player.curRoom).
            GetComponentsInChildren<Enemy>() :
            new Enemy[0];

        if(enemies.Length != 0)
        {
            Vector3 nearestEnemyPos = CalcNearestEnemyPos(enemies);
            dirX = nearestEnemyPos.x;
            dirY = nearestEnemyPos.y;
        }
        else*/
        {
            dirX = joystick.Horizontal;
            dirY = joystick.Vertical;
        }

        if (dirX != 0 && dirY != 0)
        {
            rotateWeapon = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotateWeapon + offset);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, rotateWeapon + offset);
        }
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("InverseAttack")) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetTrigger("EndAttack");
        }
    }
    private Vector3 CalcNearestEnemyPos(Enemy[] enemies)
    {
        Vector3 playerPos = player.transform.position;
        Vector3 pos = enemies[0].transform.position - playerPos;
        float minLen = pos.magnitude;

        foreach(Enemy enemy in enemies)
        {
            float curLen = (enemy.transform.position - playerPos).magnitude;
            if (curLen < minLen)
            {
                minLen = curLen;
                pos = enemy.transform.position - playerPos;
            }
        }
        return pos;
    }
    public void ClearColor(Color color)
    {
        SpriteRenderer bladeColor = transform.Find("Sword").Find("Blade").GetComponent<SpriteRenderer>();
        bladeColor.color = new Color(color.r, color.g, color.b, 1);
    }
    public void StopParticles()
    {
        ParticleSystem bladeParticle = transform.Find("Sword").Find("Blade").GetComponent<ParticleSystem>();
        bladeParticle.Stop();
    }
    public void PlayParticles()
    {
        ParticleSystem bladeParticle = transform.Find("Sword").Find("Blade").GetComponent<ParticleSystem>();
        bladeParticle.Play();
    }
    public void SetColorOfTrail(Color[] color)
    {
        TrailRenderer bladeTrail = transform.Find("Sword").Find("Trail").GetComponent<TrailRenderer>();
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[color.Length];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[color.Length];
        for(int i = 0; i < color.Length - 1; i++)
        {
            colorKey[i].color = color[i];
            colorKey[i].time = (float)i / color.Length;
            alphaKey[i].alpha = 1.0f;
            alphaKey[i].time = (float)i / color.Length;
        }
        colorKey[color.Length - 1].color = color[color.Length - 1];
        colorKey[color.Length - 1].time = 1;
        alphaKey[color.Length - 1].alpha = 1.0f;
        alphaKey[color.Length - 1].time = 1;
        gradient.SetKeys(colorKey, alphaKey);
        bladeTrail.colorGradient = gradient;
    }
    public void ClickButtonAttack()
    {
        if (cooldownAttack > 0)
            return;

        cooldownAttack = 0.5f;

        animator.SetFloat("curRotation", rotateWeapon);

        /*Transform attack = transform.Find("Sword").Find("AttackPos");
        var script = attack.GetComponent<PlayerAttack>();
        script.enabled = true;*/

        if (swordDirection == -1)
        {
            animator.SetTrigger("InverseAttack");
            swordDirection = 1;
        }
        else
        {
            animator.SetTrigger("Attack");
            swordDirection = -1;
        }
    }
}
