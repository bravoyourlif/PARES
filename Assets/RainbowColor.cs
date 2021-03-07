using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        const int TIME = 1;
        float intense = Time.time / TIME;
        intense = intense - (int)(intense/2)*2;
        if (intense > 1)
            intense = 2 - intense;
        this.GetComponent<SpriteRenderer>().color = Color.red * intense + Color.green * (1 - intense);
    }
}
