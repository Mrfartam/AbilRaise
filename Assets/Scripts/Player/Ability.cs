using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ability
{
    public string name;
    public int damage;
    public int level;
    public float maxTime;
    public float curTime;
    public Color color;
    public bool isEnded;
    public bool isStarted;

    public Ability(string name, int dmg, float maxT)
    {
        this.name = name;
        damage = dmg;
        maxTime = maxT;
        curTime = 0;
        level = 1;
        isEnded = false;
        isStarted = false;
        switch (name)
        {
            case "fire":
                color = new Color((float)230 / 255, (float)90 / 255, 0, 1); break;
            case "no":
                color = new Color(0, 0, 0, 0); break;
            default:
                color = Color.white; break;
        }
    }
    public bool CompareName(string name)
    {
        return name == this.name;
    }
}