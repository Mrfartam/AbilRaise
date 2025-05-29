using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMotion : MonoBehaviour
{
    public float speed = 2f; // �������� ��������
    private GameObject player; // ������ �� ������
    private Weapon weapon; // ������ �� ������ ������
    private Vector2 direction; // ����������� � ������
    private bool isAttacking; // �����
    private bool isKicking; // �����
    private Rigidbody2D rb; // ������ �������
    private Vector2 targetPos; // ������� ������
    private Animator animator; // �������� �������
    private float jumpCooldown; // ������� ����� ������
    private Transform spriteObject; // �������� ������� �� ��������

    public float jumpForce = 1f; // ���� ������
    public float jumpHeight = 3f; // ������������ ������ ������
    public float jumpRange = 2f; // ���������� ��� ������ ������
    public float jumpTime = 1f; // �����, �� ������� ���� ��������� ������

    public float kickTime = 0.2f; // �����, �� ������� ���� ����� �� �����
    public bool firstKick; // ������ ������� �����
    Vector2 kickDistance; // ���������� �����

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();
        rb = GetComponent<Rigidbody2D>();
        spriteObject = transform.Find("SpriteObject");
        animator = spriteObject.GetComponent<Animator>();
        isAttacking = false;
        isKicking = false;
        jumpCooldown = 0f;
    }

    void Update()
    {
        if(GetComponent<Enemy>().curRoom == player.GetComponent<Player>().curRoom)
        {
            Vector2 dir = player.transform.localPosition - transform.localPosition;

            if(dir.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)-dir.y;
        
            if (!isKicking && !isAttacking && jumpCooldown <= 0f) {
                direction = dir.normalized * speed;
                rb.velocity = direction;

                if(dir.magnitude <= jumpRange)
                {
                    targetPos = player.transform.position;
                    StartCoroutine(JumpToTarget());
                }
            }
            else if (jumpCooldown > 0 || isKicking)
            {
                rb.velocity = Vector2.zero;
                jumpCooldown -= Time.deltaTime;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    IEnumerator JumpToTarget()
    {
        isAttacking = true;

        animator.speed = 0f;

        Vector2 startPos = transform.position;
        Vector2 dir = (targetPos - startPos).normalized;

        float elipsedTime = 0f;

        while (elipsedTime < jumpTime)
        {
            float height = Mathf.Sin(Mathf.PI * (elipsedTime / jumpTime)) * jumpHeight;
            rb?.MovePosition(Vector2.Lerp(startPos, targetPos, elipsedTime / jumpTime) + new Vector2(0, height));
            
            elipsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);

        animator.speed = 1f;
        jumpCooldown = 1f;

        isAttacking = false;
    }

    public IEnumerator KickOfEnemy(int kickPower)
    {
        if (!isAttacking && !isKicking)
        {
            firstKick = true;
            isKicking = true;

            Vector2 startPos = transform.position;
            Vector2 newTargetPos = player.transform.position;
            kickDistance = (startPos - newTargetPos).normalized * 2;

            //rb.AddForce(dir * kickPower * 100, ForceMode2D.Impulse);
            //rb.velocity = new Vector2(dir.x * kickPower, dir.y * kickPower);
            //rb.MovePosition(dir * 3);

            float elipsedTime = 0f;

            while (elipsedTime < kickTime && firstKick)
            {
                rb?.MovePosition(Vector2.Lerp(startPos, startPos + kickDistance * kickPower, elipsedTime / kickTime));

                elipsedTime += Time.deltaTime;
                yield return null;
            }

            isKicking = false;
        }
        /*else if (isKicking)
        {
            firstKick = false;
            kickDistance *= (kickDistance.magnitude + 1) / (kickDistance.magnitude);
            Vector2 startPos = transform.position;

            float elipsedTime = 0f;

            while (elipsedTime < kickTime)
            {
                rb.MovePosition(Vector2.Lerp(startPos, kickDistance * kickPower, elipsedTime / kickTime));

                elipsedTime += Time.deltaTime;
                yield return null;
            }

            isKicking = false;
        }*/
    }
}
