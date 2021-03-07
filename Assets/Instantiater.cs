using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// each red dot's x,y coordinate
public class RedDot
{
    public float elapsedTime;
    public float speed;
    public int x;
    public int y;
    public bool success; // did the dot evacuate?
    public RedDot(int x, int y, bool success, float speed, float elapsedTime)
    {
        this.elapsedTime = 0;
        this.x = x;
        this.y = y;
        this.success = false;
        this.speed = speed;
    }
}

public class Instantiater : MonoBehaviour
{
    //play button 처음 클릭했을 때 true, 인간 생성 후에 false
    public bool FirstTimeAfterClick;

    const int TRIALTIME = 100;

    // number of each color blocks
    private int RedBlockNum = 0;
    private int BlueBlockNum = 0;
    private int GreenBlockNum = 0;

    //array of created dots' coordinate x,y
    public RedDot[] RedDotArray;
    private int NewX;
    private int NewY;

    // parent object of red dots
    public GameObject exit;
    public GameObject person = null;
    public Transform unitsParents;
    

    private Random r = new Random();

    private Instantiater() { }
    static private Instantiater _Instance;
    static public Instantiater Instance
    {
        get { if (_Instance == null) print("NoInstantiate"); return _Instance; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _Instance = this;
        FirstTimeAfterClick = false;
        RedDotArray = new RedDot[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (FirstTimeAfterClick)
        {
            int nowRoom, posIndex;
            //count number of blocks for each color
            RedBlockNum = GridCollector.Instance.red.Count;
            BlueBlockNum = GridCollector.Instance.blue.Count;
            GreenBlockNum = GridCollector.Instance.green.Count;

            //for all red instances, get x,y coordinate and create 10 clones around the coordinate
            RedDotArray = new RedDot[10 * RedBlockNum + 50 * BlueBlockNum];
            for (int i = 0; i < 10 * RedBlockNum + 50 * BlueBlockNum; i++)
                RedDotArray[i] = new RedDot(0,0,false,Random.Range(0.01f,0.05f),0);
            
             for (int n = 0; n < RedBlockNum; n++)
            {
                for (int i = 0; i < 10; i++)
                {
                    NewX = 0;
                    NewY = 0;
                    nowRoom = GridCollector.Instance.red[n].inRoom;
                    if (nowRoom >= 0)
                    {
                        for (int j = 0; j < TRIALTIME; j++)
                        {
                            NewX = Random.Range(GridCollector.Instance.white[nowRoom].minX, GridCollector.Instance.white[nowRoom].maxX);
                            NewY = Random.Range(GridCollector.Instance.white[nowRoom].minY, GridCollector.Instance.white[nowRoom].maxY);
                            posIndex = GridCollector.Instance.index[NewX + NewY * GridCollector.Instance.width];
                            if (posIndex == GridCollector.Instance.white[GridCollector.Instance.red[n].inRoom].ownIndex ||
                                posIndex == GridCollector.Instance.red[n].ownIndex)
                            {
                                break;
                            }
                            if(j == (TRIALTIME - 1)){
                                NewX = GridCollector.Instance.red[n].x;
                                NewY = GridCollector.Instance.red[n].y;
                            }
                        }
                    }
                    Instantiate(person, new Vector3(ToRealX(NewX), ToRealY(NewY), 0), Quaternion.identity, unitsParents);
                    RedDotArray[(n * 10) + i].x = NewX;
                    RedDotArray[(n * 10) + i].y = NewY;
                }
            }
            //for all blue instances, get x,y coordinate and create 50 clones around the coordinate
            int trialTime;
            for (int n = 0; n < BlueBlockNum; n++)
            {
                for (int i = 0; i < 50; i++)
                {
                    NewX = 0;
                    NewY = 0;
                    nowRoom = GridCollector.Instance.blue[n].inRoom;
                    if (nowRoom >= 0)
                    {
                        trialTime = (int)Mathf.Sqrt(Mathf.Pow(GridCollector.Instance.white[nowRoom].maxX - GridCollector.Instance.white[nowRoom].minX, 2)
                            + Mathf.Pow(GridCollector.Instance.white[nowRoom].maxX - GridCollector.Instance.white[nowRoom].minX, 2));
                        for (int j = 0; j < TRIALTIME * trialTime; j++)
                        {
                            NewX = Random.Range(GridCollector.Instance.white[nowRoom].minX, GridCollector.Instance.white[nowRoom].maxX);
                            NewY = Random.Range(GridCollector.Instance.white[nowRoom].minY, GridCollector.Instance.white[nowRoom].maxY);
                            posIndex = GridCollector.Instance.index[NewX + NewY * GridCollector.Instance.width];
                            if (posIndex == GridCollector.Instance.white[GridCollector.Instance.blue[n].inRoom].ownIndex ||
                                posIndex == GridCollector.Instance.blue[n].ownIndex)
                            {
                                break;
                            }
                            if (j == (TRIALTIME - 1))
                            {
                                NewX = GridCollector.Instance.blue[n].x;
                                NewY = GridCollector.Instance.blue[n].y;
                            }
                        }
                    }
                    Instantiate(person, new Vector3(ToRealX(NewX), ToRealY(NewY), 0), Quaternion.identity, unitsParents);
                    RedDotArray[(RedBlockNum * 10) + (n * 50) + i].x = NewX;
                    RedDotArray[(RedBlockNum * 10) + (n * 50) + i].y = NewY;
                }
            }
            for (int n = 0; n < GreenBlockNum; n++)
            {
                Instantiate(exit, new Vector3(ToRealX(GridCollector.Instance.green[n].x - 25),
                    ToRealY(GridCollector.Instance.green[n].y), 0), Quaternion.Euler(0,0,-180), unitsParents);
            }
            FirstTimeAfterClick = false;
            StartCoroutine(SimulationStart());
        }
    }
    bool OnRunning = false;
    public bool killCheckerFlag = false;
    IEnumerator SimulationStart(){
        OnRunning = true;
        while (OnRunning)
        {
            //move every dots to next step's position
            for (int i = 0; i < RedDotArray.Length; i++)
            {
                unitsParents.transform.GetChild(i).position = new Vector3(ToRealX(RedDotArray[i].x), ToRealY(RedDotArray[i].y), 0);
            }

            //move every 0.1f
            for (int i = 0; i < RedDotArray.Length; i++)
            {
                RedDotArray[i].elapsedTime += Time.deltaTime;
            }

            for (int i = 0; i < RedDotArray.Length; i++)
            {
                while (RedDotArray[i].elapsedTime > RedDotArray[i].speed)
                {
                    Simulator.Instance.Step(i, 2);
                    RedDotArray[i].elapsedTime -= RedDotArray[i].speed;
                }
            }

            //if bool is true, make the dot invisible
            for (int i = 0; i < RedDotArray.Length; i++)
            {
                if (RedDotArray[i].success == true && unitsParents.GetChild(i).gameObject.activeSelf == true)
                    unitsParents.GetChild(i).gameObject.SetActive(false);
            }
            yield return null;

        }
        killCheckerFlag = false;
    }

    public void killSimulator()
    {
        if (OnRunning)
        {
            killCheckerFlag = true;
            OnRunning = false;
        }
    }

    float ToRealX(int pixelX){
        return -(pixelX - ColorCollector.Instance.width / 2f)/100;
    }
    float ToRealY(int pixelY){
        return -(pixelY - ColorCollector.Instance.height / 2f)/100;
    }
}
