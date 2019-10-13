using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings singleton;

    [Header("Time")]
    public float CurrentTime = 0;
    public float TimeFlow = 1;
    public Text TimeFlow_Text;

    [Header("Orbits")]
    public bool Orbits = true;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        ChangeTimeFlow();
        ShowHideOrbits();
        CurrentTime += Time.deltaTime * TimeFlow;
    }

    private void ShowHideOrbits()
    {
        if (Input.GetKeyDown(KeyCode.O) && Orbits)
        {
            Orbits = false;
        }
        else if (Input.GetKeyDown(KeyCode.O) && !Orbits)
        {
            Orbits = true;
        }
    }

    private void ChangeTimeFlow()
    {
        if (Input.GetKeyDown(KeyCode.Equals) && TimeFlow >= 1 && TimeFlow < 1000)
        {
            TimeFlow *= 10;
            TimeFlow_Text.text = "TimeFlow: " + TimeFlow;
        }
        if (Input.GetKeyDown(KeyCode.Minus) && TimeFlow <= -1 && TimeFlow > -1000)
        {
            TimeFlow *= 10;
            TimeFlow_Text.text = "TimeFlow: " + TimeFlow;
        }
        if (Input.GetKeyDown(KeyCode.Minus) && TimeFlow == 0)
        {
            TimeFlow = -1;
            TimeFlow_Text.text = "TimeFlow: -1";
        }
        if (Input.GetKeyDown(KeyCode.Minus) && TimeFlow == 1)
        {
            TimeFlow = 0;
            TimeFlow_Text.text = "TimeFlow: Stopped";
        }
        if (Input.GetKeyDown(KeyCode.Minus) && TimeFlow > 1)
        {
            TimeFlow /= 10;
            TimeFlow_Text.text = "TimeFlow: " + TimeFlow;
        }
        if (Input.GetKeyDown(KeyCode.Equals) && TimeFlow == 0)
        {
            TimeFlow = 1;
            TimeFlow_Text.text = "TimeFlow: 1";
        }
        if (Input.GetKeyDown(KeyCode.Equals) && TimeFlow == -1)
        {
            TimeFlow = 0;
            TimeFlow_Text.text = "TimeFlow: Stopped";
        }
        if (Input.GetKeyDown(KeyCode.Equals) && TimeFlow < -1)
        {
            TimeFlow /= 10;
            TimeFlow_Text.text = "TimeFlow: " + TimeFlow;
        }
    }
}