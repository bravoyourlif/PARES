using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton_script : MonoBehaviour
{
    public Instantiater ist;
    public Button PlayButton;
    public SpriteRenderer SR;
    public GameObject units;
    //Timer variable (to detect evacuation time)
    float timer = 0.00f;
    public Text evacuationTimeText;
    public Text populationText; // display the # of remaining population
    int ExitDots = 0; // # of people exited
    bool ended = false;

    public void OnClickUpdate()
    {
        ended = false;
        //set up timer to 0 onclick
        timer = 0;
    }

    void Start()
    {
        //Let me know if the button is clicked
        PlayButton.onClick.AddListener(TaskOnClick); 
    }

    private void Update()
    {
        //stop when all dots evacuated
        ExitDots = 0;
        for (int i=0;i<Instantiater.Instance.RedDotArray.Length;i++)
        {
            if (Instantiater.Instance.RedDotArray[i].success == true)
                ExitDots++;

        }
        if (ExitDots < Instantiater.Instance.RedDotArray.Length)
        {
            timer += Time.deltaTime;
            evacuationTimeText.text = "Evacuation Time :  " + ((int)(timer * 100) / 100f).ToString();
        }
        else if (ExitDots == Instantiater.Instance.RedDotArray.Length && ExitDots > 0 && !ended)
        {
            //ist.killSimulator();
            for (int y = 0; y < SR.sprite.texture.height; y++)
            {
                for (int x = 0; x < SR.sprite.texture.width; x++)
                {
                    SR.sprite.texture.SetPixel(-x, -y, Color.white);
                }
            }
            SR.sprite.texture.Apply();
            for(int i = 0; i < units.transform.childCount; i++)
            {
                units.transform.GetChild(i).gameObject.active = false;
            }
            ended = true;


        }

            populationText.text = "Remaining Population :" + ((Instantiater.Instance.RedDotArray.Length)-ExitDots).ToString();
    }

    void TaskOnClick()
    {
        //Output to console
        Debug.Log("You have clicked the play button!");
    }
}