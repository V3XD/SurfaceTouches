using System;
using UnityEngine;

public class Touch
{
    private int id;
    private Vector2 position;
    private DateTime lastUpdated;

    public Touch(int X, int Y, int id)
    {
        position = new Vector2(X, Y);
        this.id = id;
        lastUpdated = DateTime.Now;
    }

    public int getID()
    {
        return id;
    }

    public Vector2 getPosition()
    {
        return position;
    }

    public DateTime getUpdateTime()
    {
        return lastUpdated;
    }
}