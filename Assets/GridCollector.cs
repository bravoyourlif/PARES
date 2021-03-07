using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// GridCollector 
//
//  Coded by 
//		Hyun Mo Sung 
//		banramlo95@gmail.com
//		https://github.com/QuoteQuote
//

public class BlockMedian
{
    public int x;
    public int y;
    public int inRoom;
    public int ownIndex;
    public BlockMedian(int x, int y, int ownIndex)
    {
        this.x = x;
        this.y = y;
        this.ownIndex = ownIndex;
    }
}

public class Room
{
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
    public int ownIndex;

    public Room(int minX, int maxX, int minY, int maxY, int ownIndex)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        this.ownIndex = ownIndex;
    }
}


public class GridCollector
{

    public const int ERRORTOLERATION = 100;
    public const int interpolatedPiexel = 5;

    private int _width;
    private int _height;
    private int[] _index;
    private int[] size;
    private bool[] counted;
    private List<BlockMedian> _red;
    private List<BlockMedian> _blue;
    private List<BlockMedian> _green;
    private List<Room> _white;

    private GridCollector()
    {
        _red = new List<BlockMedian>();
        _blue = new List<BlockMedian>();
        _green = new List<BlockMedian>();
        _white = new List<Room>();
    }

    private static GridCollector _Instance = null;

    public static GridCollector Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new GridCollector();
            return _Instance;
        }
    }

    public int width
    {
        get { return _width; }
        private set
        {
            _width = value;
        }
    }

    public int height
    {
        get { return _height; }
        private set
        {
            _height = value;
        }
    }

    public int[] index
    {
        get { return _index; }
        private set
        {
            _index = value;
        }
    }

    public List<BlockMedian> red
    {
        get { return _red; }
        private set
        {
            _red = value;
        }
    }

    public List<BlockMedian> blue
    {
        get { return _blue; }
        private set
        {
            _blue = value;
        }
    }

    public List<BlockMedian> green
    {
        get { return _green; }
        private set
        {
            _green = value;
        }
    }

    public List<Room> white
    {
        get { return _white; }
        private set
        {
            _white = value;
        }
    }

    const int undefined = 0;

    public void SetDataSize(int width, int height)
    {
        this.width = width;
        this.height = height;

        if (index == null)
            index = new int[width * height];
        else if ((width * height) != index.Length)
            index = new int[width * height];

        if (size == null)
            size = new int[width * height];
        else if ((width * height) != size.Length)
            size = new int[width * height];

        if (counted == null)
            counted = new bool[width * height];
        else if ((width * height) != counted.Length)
            counted = new bool[width * height];
    }

    public void SetIndex(byte[] data, int width, int height)
    {
        if (data == null)
            return;
        if (width == 0)
            return;
        SetDataSize(width, height);
        bool changed;

        InitIndex();
        bool coin1 = true;
        bool coin2 = true;
        do
        {
            changed = GoOnce(data, width, height, coin1, coin2);
            if (coin1)
            {
                if (coin2)
                { // 11 => 00
                    coin1 = false;
                    coin2 = false;
                }
                else
                { // 10 => 11
                    coin1 = true;
                    coin2 = true;
                }
            }
            else
            {
                if (coin2)
                { // 01 => 10
                    coin1 = true;
                    coin2 = false;
                }
                else
                { // 00 => 01
                    coin1 = false;
                    coin2 = true;
                }
            }
        } while (changed);

        #region //Merge failed
        /*
        int tempIdx;
        bool rowChecker;
        int count;
        int toIndex;
        byte toColor;
        do
        {
            changed = false;
            for (int i = 0; i < width * height; i++)
                counted[i] = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tempIdx = index[x + y * width];
                    if (!counted[tempIdx])
                    {
                        count = 0;
                        for (int ty = y; ty < height; ty++)
                        {
                            rowChecker = true;
                            for (int tx = 0; tx < width; tx++)
                            {
                                if (index[tx + ty * width] == tempIdx)
                                {
                                    count++;
                                    rowChecker = false;
                                }
                            }
                            if (rowChecker)
                                break;
                        }
                        if(count < ERRORTOLERATION){
                            //will be tolerated to nearby

                            changed = true;
                            toIndex = -1;
                            toColor = 0;
                            for (int ty = y - 1 > 0 ? y - 1 : 0; ty < height; ty++)
                            {
                                for (int tx = 0; tx < width; tx++)
                                {
                                    if(tx < width - 1)
                                        if (index[tx + ty * width] == tempIdx &&
                                                tempIdx != index[(tx + 1) + ty * width])
                                        {
                                            toIndex = index[(tx + 1) + ty * width];
                                            toColor = data[(tx + 1) + ty * width];
                                        }

                                    if (tx > 0)
                                        if (index[tx + ty * width] == tempIdx &&
                                                tempIdx != index[(tx - 1) + ty * width])
                                        {
                                            toIndex = index[(tx - 1) + ty * width];
                                            toColor = data[(tx - 1) + ty * width];
                                        }

                                    if (ty < height - 1)
                                        if (index[tx + ty * width] == tempIdx &&
                                                tempIdx != index[tx + (ty + 1) * width])
                                        {
                                            toIndex = index[tx + (ty + 1) * width];
                                            toColor = data[tx + (ty + 1) * width];
                                        }

                                    if (ty > 0)
                                        if (index[tx + ty * width] == tempIdx &&
                                                tempIdx != index[(tx - 1) + ty * width])
                                        {
                                            toIndex = index[tx + (ty - 1) * width];
                                            toColor = data[tx + (ty - 1) * width];
                                        }
                                    if (toIndex != -1)
                                        break;
                                }
                                if (toIndex != -1)
                                    break;
                            }
                            for (int ty = y; ty < height; ty++)
                            {
                                rowChecker = true;
                                for (int tx = 0; tx < width; tx++)
                                {
                                    if (index[tx + ty * width] == tempIdx)
                                    {
                                        index[tx + ty * width] = toIndex;
                                        data[tx + ty * width] = toColor;
                                        rowChecker = false;
                                    }
                                }
                                if (rowChecker)
                                    break;
                            }
                            
                        }
                    }
                }
            }
        } while (changed);
        */
        #endregion

        int tempIdx;
        int count;
        bool rowChecker;
        for (int i = 0; i < width * height; i++)
            counted[i] = false;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tempIdx = index[x + y * width];
                if (!counted[tempIdx])
                {
                    counted[tempIdx] = true;
                    
                    count = 0;
                    for (int ty = y; ty < height; ty++)
                    {
                        rowChecker = true;
                        for (int tx = 0; tx < width; tx++)
                        {
                            if (index[tx + ty * width] == tempIdx)
                            {
                                count++;
                                rowChecker = false;
                            }
                        }
                        if (rowChecker)
                            break;
                    }
                    size[tempIdx] = count;
                }
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (size[index[x + y * width]] < ERRORTOLERATION)
                {
                    //shadow tolerance
                    //if(ColorCollector.Instance.color[x + y * width] == 1)
                   //     ColorCollector.Instance.color[x + y * width] = 0;
                    if (ColorCollector.Instance.color[x + y * width] == 4)
                        ColorCollector.Instance.color[x + y * width] = 1;
                }
            }
        }

        byte[] colorInterpolated;
        colorInterpolated = new byte[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorInterpolated[x + y * width] = ColorCollector.Instance.color[x + y * width];
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (ColorCollector.Instance.color[x + y * width] == 1)
                {
                    for (int tx = ((x - interpolatedPiexel) > 0 ? (x - interpolatedPiexel) : 0);
                        tx < ((x + interpolatedPiexel) < width ? (x + interpolatedPiexel) : width); tx++)
                    {
                        for (int ty = ((y - interpolatedPiexel) > 0 ? (y - interpolatedPiexel) : 0);
                            ty < ((y + interpolatedPiexel) < height ? (y + interpolatedPiexel) : height); ty++)
                        {
                            if (ColorCollector.Instance.color[tx + ty * width] == 1 
                                || ColorCollector.Instance.color[tx + ty * width] == 3
                                || ColorCollector.Instance.color[tx + ty * width] == 5)
                            {
                                //Interpolate x,y to tx,ty
                                for (int ix = ((x < tx) ? x : tx); ix < ((x < tx) ? tx : x); ix++)
                                {
                                    for (int iy = ((y < ty) ? y : ty); iy < ((y < ty) ? ty : y); iy++)
                                    {
                                        colorInterpolated[ix + iy * width] = ColorCollector.Instance.color[tx + ty * width];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ColorCollector.Instance.color[x + y * width] = colorInterpolated[x + y * width];
            }
        }

        InitIndex();
        coin1 = true;
        coin2 = true;
        do
        {
            changed = GoOnce(data, width, height, coin1, coin2);
            if (coin1)
            {
                if (coin2)
                { // 11 => 00
                    coin1 = false;
                    coin2 = false;
                }
                else
                { // 10 => 11
                    coin1 = true;
                    coin2 = true;
                }
            }
            else
            {
                if (coin2)
                { // 01 => 10
                    coin1 = true;
                    coin2 = false;
                }
                else
                { // 00 => 01
                    coin1 = false;
                    coin2 = true;
                }
            }
        } while (changed);

        SetDetail(data);
    }

    public void SetDetail(byte[] data)
    {
        
        for (int i = 0; i < width * height; i++)
            counted[i] = false;
        red.Clear();
        blue.Clear();
        green.Clear();
        white.Clear();
        if (tempBuffer == null)
            tempBuffer = new int[width * height];
        else if (tempBuffer.Length != width * height)
            tempBuffer = new int[width * height];

        bool rowChecker;
        int sx, sy;
        int count;
        int tempIdx;
        int colorNow;
        int maxX, maxY, minX, minY;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tempIdx = index[x + y * width];
                if (!counted[tempIdx])
                {
                    counted[tempIdx] = true;
                    colorNow = data[x + y * width];
                    
                    sx = 0;
                    sy = 0;
                    count = 0;
                    minX = width;
                    maxX = 0;
                    minY = height;
                    maxY = 0;
                    for (int ty = y; ty < height; ty++)
                    {
                        rowChecker = true;
                        for (int tx = 0; tx < width; tx++)
                        {
                            if (index[tx + ty * width] == tempIdx)
                            {
                                if (minX > tx)
                                    minX = tx;
                                if (minY > ty)
                                    minY = ty;
                                if (maxX < tx)
                                    maxX = tx;
                                if (maxY < ty)
                                    maxY = ty;
                                sx += tx;
                                sy += ty;
                                count++;
                                rowChecker = false;
                            }
                        }
                        if (rowChecker)
                            break;
                    }
                    size[tempIdx] = count;
                    if (count > ERRORTOLERATION)
                    {
                        sx /= count;
                        sy /= count;
                        if (colorNow == 0)
                        {//white
                            white.Add(new Room(minX, maxX, minY, maxY, tempIdx));
                        }
                        else if (colorNow == 2)
                        {//r
                            red.Add(new BlockMedian(sx, sy, tempIdx));
                        }
                        else if (colorNow == 3)
                        {//g
                            green.Add(new BlockMedian(sx, sy, tempIdx));

                        }
                        else if (colorNow == 4)
                        {//b
                            blue.Add(new BlockMedian(sx, sy, tempIdx));
                        }
                    }
                }
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (data[x + y * width] == 1 && size[index[x + y * width]] < ERRORTOLERATION)
                    data[x + y * width] = 0;
            }
        }
        int inx;
        float inDist;
        float nowDist;
        for (int i = 0; i < red.Count; i++){
            inx = -1;
            inDist = Mathf.Sqrt(width * width + height * height);
            for (int j = 0; j < white.Count; j++)
            {
                if ((white[j].minX <= red[i].x) && (red[i].x <= white[j].maxX) &&
                    (white[j].minY <= red[i].y) && (red[i].y <= white[j].maxY)){
                    nowDist = Mathf.Sqrt(Mathf.Pow(white[j].maxX - white[j].minX, 2) +
                        Mathf.Pow(white[j].maxY - white[j].minY, 2));
                    if (inDist > nowDist)
                    {
                        inDist = nowDist;
                        inx = j;
                    }
                }
            }
            red[i].inRoom = inx;
            //if(red[i].inRoom >= 0)
            //MonoBehaviour.print(red[i].x + "," + red[i].y + "/ x : "+
            //    white[red[i].inRoom].minX + "~" + white[red[i].inRoom].maxX + ". / y : " +
            //    white[red[i].inRoom].minY + "~" + white[red[i].inRoom].maxY);
        }

        for (int i = 0; i < blue.Count; i++)
        {
            inx = -1;
            inDist = Mathf.Sqrt(width * width + height * height);
            MonoBehaviour.print("blue : " + blue[i].x + "," + blue[i].y);
            for (int j = 0; j < white.Count; j++)
            {
                if ((white[j].minX <= blue[i].x) && (blue[i].x <= white[j].maxX) &&
                    (white[j].minY <= blue[i].y) && (blue[i].y <= white[j].maxY))
                {
                    nowDist = Mathf.Sqrt(Mathf.Pow(white[j].maxX - white[j].minX, 2) +
                        Mathf.Pow(white[j].maxY - white[j].minY, 2));
                    if (inDist > nowDist)
                    {
                        inDist = nowDist;
                        inx = j;
                    }
                }
            }
            MonoBehaviour.print("InThis : " + inx);
            blue[i].inRoom = inx;
        }

        /*
        const int RAD = 3;
        for (int i = 0; i < red.Count; i++)
        {
            for (int y = -RAD; y <= RAD; y++)
            {
                for (int x = -RAD; x <= RAD; x++)
                    index[x + red[i].x + (y + +red[i].y) * width] = i;
            }
        }
        for (int i = 0; i < white.Count; i++)
        {
            for (int y = white[i].minY; y <= white[i].maxY; y++)
            {
                index[white[i].minX + y * width] = i;
                index[white[i].maxX + y * width] = i;
            }
            for (int x = white[i].minX; x <= white[i].maxX; x++)
            {
                index[x+ white[i].minY * width] = i;
                index[x + white[i].maxY * width] = i;
            }
        }
        */
    }

    public void InitIndex()
    {
        for (int i = 0; i < index.Length; i++)
            index[i] = i;
    }

    public bool GoOnce(byte[] data, int width, int height, bool coin1, bool coin2)
    {
        bool changed = false;
        if (coin1)
        {
            if (coin2)
            { // + +
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < width - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x + 1) + (y) * width])
                                && (index[(x) + (y) * width] != index[(x + 1) + (y) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x + 1) + (y) * width])
                                {
                                    index[(x + 1) + (y) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x + 1) + (y) * width];
                                }
                            }
                        }
                        if (y < height - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x) + (y + 1) * width])
                                && (index[(x) + (y) * width] != index[(x) + (y + 1) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x) + (y + 1) * width])
                                {
                                    index[(x) + (y + 1) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x) + (y + 1) * width];
                                }
                            }
                        }
                    }
                }
            }  // - +
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        if (x < width - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x + 1) + (y) * width])
                                && (index[(x) + (y) * width] != index[(x + 1) + (y) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x + 1) + (y) * width])
                                {
                                    index[(x + 1) + (y) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x + 1) + (y) * width];
                                }
                            }
                        }
                        if (y < height - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x) + (y + 1) * width])
                                && (index[(x) + (y) * width] != index[(x) + (y + 1) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x) + (y + 1) * width])
                                {
                                    index[(x) + (y + 1) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x) + (y + 1) * width];
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (coin2)
            { // + -
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < width - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x + 1) + (y) * width])
                                && (index[(x) + (y) * width] != index[(x + 1) + (y) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x + 1) + (y) * width])
                                {
                                    index[(x + 1) + (y) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x + 1) + (y) * width];
                                }
                            }
                        }
                        if (y < height - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x) + (y + 1) * width])
                                && (index[(x) + (y) * width] != index[(x) + (y + 1) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x) + (y + 1) * width])
                                {
                                    index[(x) + (y + 1) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x) + (y + 1) * width];
                                }
                            }
                        }
                    }
                }
            }
            else
            { // - -
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        if (x < width - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x + 1) + (y) * width])
                                && (index[(x) + (y) * width] != index[(x + 1) + (y) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x + 1) + (y) * width])
                                {
                                    index[(x + 1) + (y) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x + 1) + (y) * width];
                                }
                            }
                        }
                        if (y < height - 1)
                        {
                            if ((data[(x) + (y) * width] == data[(x) + (y + 1) * width])
                                && (index[(x) + (y) * width] != index[(x) + (y + 1) * width]))
                            {
                                changed = true;
                                if (index[(x) + (y) * width] < index[(x) + (y + 1) * width])
                                {
                                    index[(x) + (y + 1) * width] = index[(x) + (y) * width];
                                }
                                else
                                {
                                    index[(x) + (y) * width] = index[(x) + (y + 1) * width];
                                }
                            }
                        }
                    }
                }
            }
        }
        return changed;
    }

    int[] tempBuffer = null;
}