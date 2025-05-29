using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{

    public float dirX, dirY;
    public float speed;
    public Animator animator;
    public Joystick joystick;
    private Rigidbody2D rb;
    public int curLvl;
    public int curRoom;
    public Ability curAbility;
    public string nickname;
    public int numOfKilledEnemy;

    void Start()
    {
        curAbility = new Ability("fire", 1, 5);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateRendering();
        speed = 7 * (float)(Math.Sqrt(joystick.Horizontal * joystick.Horizontal + joystick.Vertical * joystick.Vertical));
        dirX = joystick.Horizontal * speed;
        dirY = joystick.Vertical * speed;

        animator.SetFloat("Speed", speed);

        if (dirY == 0 && dirX == 0)
        {
            animator.SetInteger("Right", 0);
            animator.SetInteger("Up", 0);
        }
        else if (dirX != 0 && Math.Abs(dirY / dirX) >= 1)
        {
            animator.SetInteger("Right", 0);
            if (dirY > 0)
            {
                animator.SetInteger("Up", 1);
            }
            else
            {
                animator.SetInteger("Up", -1);
            }
        }
        else
        {
            animator.SetInteger("Up", 0);
            if (dirX > 0)
            {
                animator.SetInteger("Right", 1);
            }
            else
            {
                animator.SetInteger("Right", -1);
            }
        }
        // Без портала
        /*if (curRoom == roomsGenerator.GetNumRooms().Last().Key)
        {
            roomsGenerator.DestroyLevel();
            transform.position = new Vector3(0, 0, 0);
            levelGenerator.curLevel++;
            levelGenerator.StartGame();
        }*/
    }
    public void UpdateRendering()
    {
        GetComponent<SortingGroup>().sortingOrder = -(int)transform.position.y;
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2 (dirX, dirY);
    }
}