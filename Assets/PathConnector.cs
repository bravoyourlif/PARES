using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConnector{
    byte[] _policy;
    int[] _distance;
    public int width, height;

    private int _INFINITEDISTANCE;
    public const int EXITCOLOR = 3;
    public const int WALLCOLOR = 1;

    static PathConnector _Instance;

    private PathConnector() { }
    
    public byte[] policy {
        get { return _policy; }
        private set {
            _policy = value;
        }
    }

        public int INFINITEDISTANCE {
        get { return _INFINITEDISTANCE; }
        private set
        {
            _INFINITEDISTANCE = value;
        }
    }

    public int[] distance {
        get { return _distance; }
        private set {
            _distance = value;
        }
    }

    public static PathConnector Instance {
        get {
            if (_Instance == null)
                _Instance = new PathConnector();
            return _Instance;
        }
    }

    void SetDataSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        INFINITEDISTANCE = width * height;

        if (policy == null)
            policy = new byte[width * height];
        else if ((width * height) != policy.Length)
            policy = new byte[width * height];

        if (distance == null)
            distance = new int[width * height];
        else if ((width * height) != distance.Length)
            distance = new int[width * height];
    }


    public void SetPath(int width, int height, int scaling)
    {
        width /= scaling;
        height /= scaling;
        SetDataSize(width, height);
        byte[] color = ColorCollector.Instance.color;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                policy[x + y * width] = 0;
                distance[x + y * width] = INFINITEDISTANCE;
            }
        }
        for (int i = 0; i < GridCollector.Instance.green.Count; i++){
            distance[(GridCollector.Instance.green[i].x / scaling) + (GridCollector.Instance.green[i].y / scaling) * width] = 0;
        }
        bool changed = true;
        bool IsItwall;
        do
        {
            changed = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    IsItwall = false;
                    for (int wy = y * scaling; wy < (y + 1) * scaling; wy++)
                    {
                        for (int wx = x * scaling; wx < (x + 1) * scaling; wx++)
                        {

                            if (color[wx + wy * (width * scaling)] == WALLCOLOR)
                            {
                                IsItwall = true;
                            }
                        }
                    }
                    if(!IsItwall)
                    {
                        if (x > 0)
                        {
                            if (distance[(x) + (y) * width] > 1 + distance[(x - 1) + (y) * width])
                            {
                                distance[(x) + (y) * width] = 1 + distance[(x - 1) + (y) * width];
                                changed = true;
                            }
                        }
                        if (y > 0)
                        {
                            if (distance[(x) + (y) * width] > 1 + distance[x + (y - 1) * width])
                            {
                                distance[(x) + (y) * width] = 1 + distance[x + (y - 1) * width];
                                changed = true;
                            }
                        }
                        if (x < width - 1)
                        {
                            if (distance[(x) + (y) * width] > 1 + distance[(x + 1) + (y) * width])
                            {
                                distance[(x) + (y) * width] = 1 + distance[(x + 1) + (y) * width];
                                changed = true;
                            }
                        }
                        if (y < height - 1)
                        {
                            if (distance[(x) + (y) * width] > 1 + distance[x + (y + 1) * width])
                            {
                                distance[(x) + (y) * width] = 1 + distance[x + (y + 1) * width];
                                changed = true;
                            }
                        }
                    }
                }
            }
        } while (changed);
        int min = INFINITEDISTANCE;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                policy[x + y * width] = 0;
                min = INFINITEDISTANCE;
                if (x > 0)
                    if(min > distance[(x - 1) + y * width])
                        min = distance[(x - 1) + y * width];
                if (x < width - 1)
                    if (min > distance[(x + 1) + y * width])
                        min = distance[(x + 1) + y * width];
                if (y > 0)
                    if (min > distance[x + (y - 1) * width])
                        min = distance[x + (y - 1) * width];
                if (y < height - 1)
                    if (min > distance[x + (y + 1) * width])
                        min = distance[x + (y + 1) * width];

                if (x > 0)
                    if (min == distance[(x - 1) + y * width])
                    {
                        IsItwall = false;
                        for (int wy = y * scaling; wy < (y + 1) * scaling; wy++)
                        {
                            for (int wx = (x-1) * scaling; wx < (x) * scaling; wx++)
                            {
                                if (color[wx + wy * width * scaling] == WALLCOLOR)
                                {
                                    IsItwall = true;
                                }
                            }
                        }
                        if (!IsItwall)
                            policy[x + y * width] += 1; // L
                    }
                if (x < width - 1)
                    if (min == distance[(x + 1) + y * width])
                    {
                        IsItwall = false;
                        for (int wy = y * scaling; wy < (y + 1) * scaling; wy++)
                        {
                            for (int wx = (x + 1) * scaling; wx < (x + 2) * scaling; wx++)
                            {

                                if (color[wx + wy * width * scaling] == WALLCOLOR)
                                {
                                    IsItwall = true;
                                }
                            }
                        }
                        if (!IsItwall)
                            policy[x + y * width] += 2; //R
                    }
                if (y > 0)
                    if (min == distance[x + (y - 1) * width])
                    {
                        IsItwall = false;
                        for (int wy = (y-1) * scaling; wy < (y) * scaling; wy++)
                        {
                            for (int wx = x * scaling; wx < (x + 1) * scaling; wx++)
                            {

                                if (color[wx + wy * width * scaling] == WALLCOLOR)
                                {
                                    IsItwall = true;
                                }
                            }
                        }
                        if (!IsItwall)
                            policy[x + y * width] += 4; // D
                    }
                if (y < height - 1)
                    if (min == distance[x + (y + 1) * width])
                    {
                        IsItwall = false;
                        for (int wy = (y + 1) * scaling; wy < (y + 2) * scaling; wy++)
                        {
                            for (int wx = x * scaling; wx < (x + 1) * scaling; wx++)
                            {

                                if (color[wx + wy * width * scaling] == WALLCOLOR)
                                {
                                    IsItwall = true;
                                }
                            }
                        }
                        if (!IsItwall)
                            policy[x + y * width] += 8; // U
                    }
            }
        }
    }
}
