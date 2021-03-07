using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Simulator
{
    byte[] policy;
    int width;

    enum Direction{
        Left, Right, Down, Up, None
    }

    private Simulator()
    {
    }

    private static Simulator _Instance = null;

    public static Simulator Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new Simulator();
            return _Instance;
        }
    }

    // Update is called once per frame
    public void Step(int i, int scaling)
    {
        bool col;
        policy = PathConnector.Instance.policy;
        width = ColorCollector.Instance.width/scaling;
        Direction dir;
        float count;
        dir = Direction.None;
        count = 0;
        
        if ((policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 1) != 0)
        { // L
            count++;
        }
        if ((policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 2) != 0)
        { // R
            count++;
        }
        if ((policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 4) != 0)
        { // D
            count++;
        }
        if ((policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 8) != 0)
        { // U
            count++;
        }

        if (count > 0)
        {
            if ((dir == Direction.None) && (policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 4) != 0)
            { // D
                if ((1 / count) >= Random.Range(0, 1.0f)) // 1/n
                    dir = Direction.Down;
                count--;
            }
            if ((dir == Direction.None) && (policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 8) != 0)
            { // U
                if ((1 / count) >= Random.Range(0, 1.0f)) // (n - 1) / n  * 1/(n-1) = 1/n
                    dir = Direction.Up;
                count--;
            }
            if ((dir == Direction.None) && (policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 1) != 0)
            { // L
                if ((1 / count) >= Random.Range(0, 1.0f))
                    dir = Direction.Left;
                count--;
            }
            if ((dir == Direction.None) && (policy[Instantiater.Instance.RedDotArray[i].x / scaling + Instantiater.Instance.RedDotArray[i].y / scaling * width] & 2) != 0)
            { // R
                if ((1 / count) >= Random.Range(0, 1.0f))
                    dir = Direction.Right;
                count--;
            }
        }
        int tx, ty;

        tx = Instantiater.Instance.RedDotArray[i].x;
        ty = Instantiater.Instance.RedDotArray[i].y;

        switch (dir)
        {
            default:
            case Direction.None:
                break;
            case Direction.Left:
                tx -= scaling;
                break;
            case Direction.Right:
                tx += scaling;
                break;
            case Direction.Down:
                ty -= scaling;
                break;
            case Direction.Up:
                ty += scaling;
                break;
        }
        col = false;
        for (int j = 0; j < Instantiater.Instance.RedDotArray.Length; j++)
        { //collision 
            if (Instantiater.Instance.RedDotArray[j].x/scaling == tx / scaling && 
                Instantiater.Instance.RedDotArray[j].y / scaling == ty / scaling
                && !Instantiater.Instance.RedDotArray[j].success)
            {
                col = true;
                break;
            }
        }

        // if it gets to the green exit, change its bool value to true
        for(int j = 0; j < GridCollector.Instance.green.Count; j++)
        {
            if(GridCollector.Instance.green[j].x / scaling == tx / scaling
                && GridCollector.Instance.green[j].y / scaling == ty / scaling)
                Instantiater.Instance.RedDotArray[i].success = true;
        }


        if (!col)
        {
            Instantiater.Instance.RedDotArray[i].x = tx;
            Instantiater.Instance.RedDotArray[i].y = ty;
        }
        else
        {
            tx = Instantiater.Instance.RedDotArray[i].x + Random.Range(-2, 2);
            ty = Instantiater.Instance.RedDotArray[i].y + Random.Range(-2, 2);
            col = false;
            for (int j = 0; j < Instantiater.Instance.RedDotArray.Length; j++)
            { //collision 
                if (Instantiater.Instance.RedDotArray[j].x / scaling == tx / scaling &&
                    Instantiater.Instance.RedDotArray[j].y / scaling == ty / scaling
                    && !Instantiater.Instance.RedDotArray[j].success)
                {
                    col = true;
                    break;
                }
            }
            if(PathConnector.Instance.distance[tx / scaling + ty / scaling * width]
                    == PathConnector.Instance.INFINITEDISTANCE)
            {
                col = true;
            }
            if (!col)
            {
                Instantiater.Instance.RedDotArray[i].x = tx;
                Instantiater.Instance.RedDotArray[i].y = ty;
            }
        }
    }

}
