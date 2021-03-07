using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UnityEngine.UI;

public class ColorSourceManager2 : MonoBehaviour 
{
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }
    
    private KinectSensor _Sensor;
    private ColorFrameReader _ReaderColor;
    private Texture2D _TextureColor;
    private Texture2D _TextureDepth;
    private byte[] _DataColor;

    public const int SCALE = 2;

    public Instantiater ist;

    const int XOFFSET = 735;//600;//
    const int YOFFSET = 310;//200;//

    SpriteRenderer SR;
    public SpriteRenderer ORGSR;
    public Text exitText; // display the # of exits
    public int remainingPopulation;
    public int realWidth;

    private CoordinateMapper _Mapper;

    public TouchHandler TH;

    void Awake()
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null)
        {
            _ReaderColor = _Sensor.ColorFrameSource.OpenReader();

            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = 600;
            ColorHeight = 320;
            realWidth = frameDesc.Width;
            _TextureColor = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _DataColor = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
            

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }

        SR = this.GetComponent<SpriteRenderer>();
        SR.sprite = Sprite.Create(new Texture2D(ColorWidth, ColorHeight),
            new Rect(new Vector2(0, 0), new Vector2(ColorWidth, ColorHeight))
            , new Vector2(0.5f, 0.5f));

        ORGSR.sprite = Sprite.Create(new Texture2D(ColorWidth, ColorHeight),
            new Rect(new Vector2(0, 0), new Vector2(ColorWidth, ColorHeight))
            , new Vector2(0.5f, 0.5f));

    }


    public void OnClickUpdate () 
    {
        StartCoroutine(DelayDraw());
    }

    public float GetCurrentBright(){
        float bright;
        bright = 0;
        if (_ReaderColor != null)
        {
            var frameColor = _ReaderColor.AcquireLatestFrame();

            if (frameColor != null)
            {
                frameColor.CopyConvertedFrameDataToArray(_DataColor, ColorImageFormat.Rgba);

                for (int i = 0; i < ColorWidth * ColorHeight; i++)
                {
                    bright += (_DataColor[i * 4 + 0] + _DataColor[i * 4 + 1] + _DataColor[i * 4 + 2])/3f;
                }
            }
        }
        bright /= ColorWidth * ColorHeight;
        return bright;
    }

    IEnumerator DelayDraw(){
        if (_ReaderColor != null)
        {
            var frameColor = _ReaderColor.AcquireLatestFrame();

            if (frameColor != null)
            {

                yield return null;
                // w,h 665,350 ofx y 760,300d
                frameColor.CopyConvertedFrameDataToArray(_DataColor, ColorImageFormat.Rgba);


                ColorCollector.Instance.SetColor(_DataColor, ColorWidth, ColorHeight, XOFFSET, YOFFSET, realWidth);
                GridCollector.Instance.SetIndex(ColorCollector.Instance.color, ColorWidth, ColorHeight);
                PathConnector.Instance.SetPath(ColorWidth, ColorHeight, SCALE);


                //display colorhash on screen
                for (int y = 0; y < ColorHeight; y++)
                {
                    for (int x = 0; x < ColorWidth; x++)
                    {
                        ORGSR.sprite.texture.SetPixel(-x, -y,
                            //ColorHash2(PathConnector.Instance.distance[x / SCALE + y / SCALE * ColorWidth / SCALE]));
                            ColorHash(ColorCollector.Instance.color[x + y * ColorWidth]));
                        //new Color(_DataColor[((x + XOFFSET) + (y + YOFFSET) * realWidth) * 4 + 0] / 255f,
                        //_DataColor[((x + XOFFSET) + (y + YOFFSET) * realWidth) * 4 + 1] / 255f,
                        //_DataColor[((x + XOFFSET) + (y + YOFFSET) * realWidth) * 4 + 2] / 255f));
                        //SR.sprite.texture.SetPixel(-x, -y, Color.white);
                            //ColorHash(ColorCollector.Instance.color[x + y * ColorWidth]));
                    }
                }
                int step = 0;
                int ended = 0;
                int i;
                float timeStart;
                float maxDist = 0;
                float newDist;
                timeStart = Time.time;
                
                for (i = 0; i < GridCollector.Instance.red.Count; i++)
                {
                    newDist = PathConnector.Instance.distance[GridCollector.Instance.red[i].x / SCALE + GridCollector.Instance.red[i].y / SCALE * PathConnector.Instance.width];
                    if (maxDist < newDist)
                        maxDist = newDist;
                }
                for (i = 0; i < GridCollector.Instance.blue.Count; i++)
                {
                    newDist = PathConnector.Instance.distance[GridCollector.Instance.blue[i].x / SCALE + GridCollector.Instance.blue[i].y / SCALE * PathConnector.Instance.width];
                    if (maxDist < newDist)
                        maxDist = newDist;
                }
                while (ended < (GridCollector.Instance.red.Count + GridCollector.Instance.blue.Count)) {
                    ended = 0;
                    for (int y = 0; y < ColorHeight; y++)
                        for (int x = 0; x < ColorWidth; x++)
                            SR.sprite.texture.SetPixel(-x, -y, Color.white); //ColorHash(ColorCollector.Instance.color[x + y * ColorWidth]));
                    step = (int)(maxDist * (Time.time - timeStart) / 4);
                    for (i = 0; i < GridCollector.Instance.red.Count; i++){
                        if (PathShower(GridCollector.Instance.red[i].x / SCALE, GridCollector.Instance.red[i].y / SCALE, step, SCALE))
                            ended++;
                    }
                    for (i = 0; i < GridCollector.Instance.blue.Count; i++){
                        if (PathShower(GridCollector.Instance.blue[i].x / SCALE, GridCollector.Instance.blue[i].y / SCALE, step, SCALE))
                            ended++;
                    }
                    SR.sprite.texture.Apply();
                    yield return null;
                }
                print(step);
                TH.OnGo = false;
                //for(int y= 0; y < ColorHeight/2; y++)
                //{
                //    for(int x = 0; x < ColorWidth/2; x++)
                //    {
                //        SR.sprite.texture.SetPixel(-2 * x, -2 * y,
                //        ColorHash2(PathConnector.Instance.distance[x + y * (ColorWidth/2)]));
                //    }
                //}
                ORGSR.sprite.texture.Apply();
                frameColor.Dispose();
                frameColor = null;
            }
        }
        
        exitText.text = "Exit :" + GridCollector.Instance.green.Count.ToString();
        ist.FirstTimeAfterClick = true;
        print("called");
    }

    void OnApplicationQuit()
    {
        if (_ReaderColor != null) 
        {
            _ReaderColor.Dispose();
            _ReaderColor = null;
        }
        if (_Sensor != null) 
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }

    bool PathShower(int x, int y, int proceed, int scale){
        if (PathConnector.Instance.distance[x + y * PathConnector.Instance.width] == 0)
            return true;
        if(proceed > 0)
        {
            for (int ty = y * scale; ty < (y + 1) * scale; ty++){
                for (int tx = x * scale; tx < (x + 1) * scale; tx++){
                    SR.sprite.texture.SetPixel(-tx, -ty, Color.blue);
                }
            }
            // 1 L 2 R 4 D 8 U
            if ((PathConnector.Instance.policy[x + y * PathConnector.Instance.width] & 1) != 0)
            {
                return PathShower(x - 1, y, proceed - 1, scale);
            }
            else if ((PathConnector.Instance.policy[x + y * PathConnector.Instance.width] & 2) != 0)
            {
                return PathShower(x + 1, y, proceed - 1, scale);
            }
            else if ((PathConnector.Instance.policy[x + y * PathConnector.Instance.width] & 4) != 0)
            {
                return PathShower(x, y - 1, proceed - 1, scale);
            }
            else if ((PathConnector.Instance.policy[x + y * PathConnector.Instance.width] & 8) != 0)
            {
                return PathShower(x, y + 1, proceed - 1, scale);
            }
            return true;
        }
        return false;
    }

    Color ColorHash(int value)
    {
        Color purple = new Color(0.58f, 0f, 0.82f, 1f); // initiate a new color purple

        switch (value)
        { // if kinect reads the color 0~6, make it to color white~purle.
            default: break;
            case 0: return Color.white;
            case 1: return Color.black;
            case 2: return Color.red;
            case 3: return Color.green;
            case 4: return Color.blue;
            case 5: return Color.yellow;
            case 6: return purple;
        }
        Random.InitState(value);
        return new Color(Random.value % 1 * 0.7f, Random.value % 1 * 0.7f, Random.value % 1 * 0.7f);
    }
    Color ColorHash2(int value)
    {
        const float specular = 30;
        return new Color((value % specular) / specular, (value % specular) / specular, (value % specular) / specular);
    }
}
