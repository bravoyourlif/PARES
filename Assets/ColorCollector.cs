using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ColorCollector 
//
//  Coded by 
//		Hyun Mo Sung 
//		banramlo95@gmail.com
//		https://github.com/QuoteQuote
//

public class ColorCollector
{
    public int width, height;

    private byte[] _color;
    private static ColorCollector _Instance = null;

    public static ColorCollector Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new ColorCollector();
            return _Instance;
        }
    }

    public byte[] color
    {
        get { return _color; }
        private set
        {
            _color = value;
        }
    }

    const int undefined = 0;

    private ColorCollector() { }

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        if (_color == null)
            _color = new byte[width * height];
        else if ((width * height) != _color.Length)
            _color = new byte[width * height];
    }

    public void SetColor(byte[] data, int width, int height, int offSetX, int offSetY, int realWidth)
    {
        if (data == null)
            return;
        if (width == 0 && height == 0)
            return;
        SetSize(width, height);

        byte r, g, b;
        hsv_color hsv;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                r = data[((x + offSetX) + (y + offSetY) * realWidth) * 4];
                g = data[((x + offSetX) + (y + offSetY) * realWidth) * 4 + 1];
                b = data[((x + offSetX) + (y + offSetY) * realWidth) * 4 + 2];

                hsv = RGB2HSV(r, g, b);



                //red
                if ((hsv.h > 255 * 0.90 || hsv.h < 255 * 0.1) && (hsv.v < 255 * 0.995) && (hsv.s > 255 * 0.4))
                {
                    color[x + y * width] = 2;
                }
                //green
                else if ((hsv.h > 255 * 0.2 && hsv.h < 255 * 0.55) && (hsv.v < 255 * 0.995) && (hsv.s > 255 * 0.2))
                {
                    color[x + y * width] = 3;
                }
                //blue
                else if ((hsv.h > 255 * 0.55 && hsv.h < 255 * 0.62) && (hsv.v < 255 * 0.995) && (hsv.s > 255 * 0.7))
                {
                    color[x + y * width] = 4;
                }
                //yellow
                else if ((hsv.h > 255 * 0.094 && hsv.h < 255 * 0.2) && (hsv.v < 255 * 0.999) && (hsv.s > 255 * 0.2))
                {
                    color[x + y * width] = 5;
                }
                //purple
                else if ((hsv.h > 255 * 0.62 && hsv.h < 255 * 0.7) && (hsv.v < 255 * 0.995) && (hsv.s > 255 * 0.25))
                {
                    color[x + y * width] = 1;
                }
                //black
                else if (hsv.v < 255 * 0.4)
                {
                    color[x + y * width] = 1;
                }
                //backgroud
                else
                {
                    color[x + y * width] = 0;
                }
            }
        }
    }

    struct hsv_color
    {
        public byte h;        // Hue: 0 ~ 255 (red:0, gree: 85, blue: 171)
        public byte s;        // Saturation: 0 ~ 255
        public byte v;        // Value: 0 ~ 255
    };

    hsv_color RGB2HSV(byte r, byte g, byte b)
    {
        byte rgb_min, rgb_max;
        rgb_min = ((g) <= (r) ? ((b) <= (g) ? (b) : (g)) : ((b) <= (r) ? (b) : (r)));
        rgb_max = ((g) >= (r) ? ((b) >= (g) ? (b) : (g)) : ((b) >= (r) ? (b) : (r)));

        hsv_color hsv;
        hsv.v = rgb_max;
        if (hsv.v == 0)
        {
            hsv.h = hsv.s = 0;
            return hsv;
        }

        hsv.s = (byte)(255f * (rgb_max - rgb_min) / hsv.v);
        if (hsv.s == 0)
        {
            hsv.h = 0;
            return hsv;
        }

        if (rgb_max == r)
        {
            hsv.h = (byte)(0 + 43f * (g - b) / (rgb_max - rgb_min));
        }
        else if (rgb_max == g)
        {
            hsv.h = (byte)(85 + 43f * (b - r) / (rgb_max - rgb_min));
        }
        else // rgb_max == rgb.b
        {
            hsv.h = (byte)(171 + 43f * (r - g) / (rgb_max - rgb_min));
        }

        return hsv;
    }





}



//* using System.Collections;
/*
using System.Collections.Generic;
using UnityEngine;

//
// ColorCollector 
//
//  Coded by 
//		Hyun Mo Sung 
//		banramlo95@gmail.com
//		https://github.com/QuoteQuote
//
*/
/*
public class ColorCollector
{
    public int width, height;

    public const float MAXDISTOFCOLOR = 100f;

    private byte[] _color;
    hsv_color c_r, c_g, c_b, c_y;

    private static ColorCollector _Instance = null;

    public static ColorCollector Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new ColorCollector();
            return _Instance;
        }
    }

    public byte[] color
    {
        get { return _color; }
        private set
        {
            _color = value;
        }
    }

    const int undefined = 0;

    private ColorCollector() {
    }

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        if (_color == null)
            _color = new byte[width * height];
        else if ((width * height) != _color.Length)
            _color = new byte[width * height];
    }

    public void SetColor(byte[] data, int width, int height, int offSetX, int offSetY, int realWidth)
    {
        if (data == null)
            return;
        if (width == 0 && height == 0)
            return;
        SetSize(width, height);

        byte r, g, b;
        hsv_color hsv;
        float temph, temps, tempv;

        hsv_color temphsv;
        //10 pepple
        temph = 0;
        temps = 0;
        tempv = 0;
        for (int y = 200 + 79; y < 200 + 92; y++){
            for (int x = 600 + (666 - 486); x < 600 + (666 - 474); x++)
            {
                temphsv = SetRGBAmpt(data[(x + y * realWidth) * 4 + 0],
                            data[(x + y * realWidth) * 4 + 1], data[(x + y * realWidth) * 4 + 2]);
                temph += temphsv.h;
                temps += temphsv.s;
                tempv += temphsv.v;
            }
        }
        temph /= (92 - 79) * (486 - 474);
        temps /= (92 - 79) * (486 - 474);
        tempv /= (92 - 79) * (486 - 474);
        c_r.h = (byte)temph;
        c_r.s = (byte)temps;
        c_r.v = (byte)tempv;

        temph = 0;
        temps = 0;
        tempv = 0;
        //50pepple
        for(int y = 200 + 79 + 2; y < 200 + 96 - 2; y++)
        {
            for (int x = 600 + (666 - 343) - 3; x < 600 + (666 - 328) - 3; x++)
            {
                temphsv = SetRGBAmpt(data[(x + y * realWidth) * 4 + 0],
                            data[(x + y * realWidth) * 4 + 1], data[(x + y * realWidth) * 4 + 2]);
                temph += temphsv.h;
                temps += temphsv.s;
                tempv += temphsv.v;
            }
        }
        temph /= (92 - 79) * (486 - 474);
        temps /= (92 - 79) * (486 - 474);
        tempv /= (92 - 79) * (486 - 474);
        c_b.h = (byte)temph;
        c_b.s = (byte)temps;
        c_b.v = (byte)tempv;

        temph = 0;
        temps = 0;
        tempv = 0;
        //Exit
        for (int y = 200 + 77 + 3; y < 200 + 90 + 3; y++)
        {
            for (int x = 600 + (666 - 213) - 3; x < 600 + (666 - 198) - 3; x++)
            {
                temphsv = SetRGBAmpt(data[(x + y * realWidth) * 4 + 0],
                            data[(x + y * realWidth) * 4 + 1], data[(x + y * realWidth) * 4 + 2]);
                temph += temphsv.h;
                temps += temphsv.s;
                tempv += temphsv.v;
            }
        }
        temph /= (92 - 79) * (486 - 474);
        temps /= (92 - 79) * (486 - 474);
        tempv /= (92 - 79) * (486 - 474);
        c_g.h = (byte)temph;
        c_g.s = (byte)temps;
        c_g.v = (byte)tempv;

        temph = 0;
        temps = 0;
        tempv = 0;
        //Door
        for (int y = 200 + 81; y < 200 + 93; y++)
        {
            for (int x = 600 + (666 - 110); x < 600 + (666 - 90) - 5; x++)
            {
                temphsv = SetRGBAmpt(data[(x + y * realWidth) * 4 + 0],
                            data[(x + y * realWidth) * 4 + 1], data[(x + y * realWidth) * 4 + 2]);
                temph += temphsv.h;
                temps += temphsv.s;
                tempv += temphsv.v;
            }
        }
        temph /= (92 - 79) * (486 - 474);
        temps /= (92 - 79) * (486 - 474);
        tempv /= (92 - 79) * (486 - 474);
        c_y.h = (byte)temph;
        c_y.s = (byte)temps;
        c_y.v = (byte)tempv;

        MonoBehaviour.print("red" + c_r.h + "," + c_r.s + "," + c_r.v +
            "blue" + c_b.h + "," + c_b.s + "," + c_b.v + "green" + "," + c_g.h + "," + c_g.s + "," + c_g.v + 
            "yellow" + c_y.h + "," +c_y.s + "," + c_y.v);

        float disR, disG, disB, disY;
        float min;
        byte colorCode;

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                r = data[((x + offSetX) + (y + offSetY) * realWidth) * 4];
                g = data[((x + offSetX) + (y + offSetY) * realWidth) * 4 + 1];
                b = data[((x + offSetX) + (y + offSetY) * realWidth) * 4 + 2];

                hsv = RGB2HSV(r, g, b);


                //black
                if (hsv.v < 255 * 0.4)
                {
                    color[x + y * width] = 1;
                }
                //backgroud
                else
                {
                    disR = Mathf.Sqrt(Mathf.Pow(r - c_r.h, 2) 
                        + Mathf.Pow(g - c_r.s, 2) + Mathf.Pow(b - c_r.v, 2));
                    disG = Mathf.Sqrt(Mathf.Pow(r - c_g.h, 2) 
                        + Mathf.Pow(g - c_g.s, 2) + Mathf.Pow(b - c_g.v, 2));
                    disB = Mathf.Sqrt(Mathf.Pow(r - c_b.h, 2) 
                        + Mathf.Pow(g - c_b.s, 2) + Mathf.Pow(b - c_b.v, 2));
                    disY = Mathf.Sqrt(Mathf.Pow(r - c_y.h, 2) 
                        + Mathf.Pow(g - c_y.s, 2) + Mathf.Pow(b - c_y.v, 2));
                    min = disR;
                    colorCode = 2;
                    if(min > disG){
                        min = disG;
                        colorCode = 3;
                    }
                    if (min > disB)
                    {
                        min = disB;
                        colorCode = 4;
                    }
                    if (min > disY)
                    {
                        min = disY;
                        colorCode = 5;
                    }
                    if(min < MAXDISTOFCOLOR){
                        color[x + y * width] = colorCode;
                    }
                    else{
                        color[x + y * width] = 0;
                    }
                }
            }
        }
    }

    hsv_color SetRGBAmpt(byte r, byte g, byte b)
    {
        hsv_color temp;

        temp.h = r;
        temp.s = g;
        temp.v = b;

        return temp;
    }

    struct hsv_color
    {
        public byte h;        // Hue: 0 ~ 255 (red:0, gree: 85, blue: 171)
        public byte s;        // Saturation: 0 ~ 255
        public byte v;        // Value: 0 ~ 255
    };

    hsv_color RGB2HSV(byte r, byte g, byte b)
    {
        byte rgb_min, rgb_max;
        rgb_min = ((g) <= (r) ? ((b) <= (g) ? (b) : (g)) : ((b) <= (r) ? (b) : (r)));
        rgb_max = ((g) >= (r) ? ((b) >= (g) ? (b) : (g)) : ((b) >= (r) ? (b) : (r)));

        hsv_color hsv;
        hsv.v = rgb_max;
        if (hsv.v == 0)
        {
            hsv.h = hsv.s = 0;
            return hsv;
        }
        
        hsv.s = (byte)(255f * (rgb_max - rgb_min) / hsv.v);
        if (hsv.s == 0)
        {
            hsv.h = 0;
            return hsv;
        }

        if (rgb_max == r)
        {
            hsv.h = (byte)(0 + 43f * (g - b) / (rgb_max - rgb_min));
        }
        else if (rgb_max == g)
        {
            hsv.h = (byte)(85 + 43f * (b - r) / (rgb_max - rgb_min));
        }
        else // rgb_max == rgb.b 
        {
            hsv.h = (byte)(171 + 43f * (r - g) / (rgb_max - rgb_min));
        }

        return hsv;
    }
}

 */