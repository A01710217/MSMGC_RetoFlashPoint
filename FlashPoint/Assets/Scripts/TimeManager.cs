using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static Action OnSecondChanged;
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Second{get; private set;}
    public static int Minuted{get;private set;}
    public static int Hour{get;private set;}

    private float secondToRealTime = 0.5f;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Second = 0;
        Minuted = 0;
        Hour = 0;
        timer = secondToRealTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = secondToRealTime;
            Second++;
            OnSecondChanged?.Invoke();
            if(Second >= 60)
            {
                Second = 0;
                Minuted++;
                OnMinuteChanged?.Invoke();
                if(Minuted >= 60)
                {
                    Minuted = 0;
                    Hour++;
                    OnHourChanged?.Invoke();
                }
            }
        }
        
    }
}
