using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class OnnOff : MonoBehaviour
{
    private KeyCode control= KeyCode.F;

    public GameObject _object, _object2;
    private Light _light;

    private float timer=5;
    private float currtime;
    public float check;
    private float bWorkTime = 12;

    public Slider sliderTime;
    public Slider sliderButtery;

    // Start is called before the first frame update
    void Awake()
    {
        _object = GameObject.Find("Spot Light");
        _light = GetComponent<Light>();
        _object2 = GameObject.Find("Button");
        _object2.SetActive(false);
        sliderTime.maxValue = timer;
        sliderButtery.maxValue = bWorkTime;
    }

    private void TriggerLight(bool val)
    {
        _light.enabled = val; 
    }

    public void LoadUpB()
    {
        sliderButtery.value = bWorkTime;
        check = 0; 
        sliderTime.value = timer; 
        currtime = 0;
    }

    // Update is called once per frame
    void Update()
    {
                if (Input.GetKeyDown(control) && _light.enabled)
                {
                    TriggerLight(false);
                    sliderTime.value = timer; currtime=0;
                }

                else if (Input.GetKeyDown(control) && !_light.enabled && (bWorkTime - check)>0)
                {
                    TriggerLight(true);
        }

                if (_light.enabled)
                {
                    currtime += Time.deltaTime;
                    sliderTime.value = timer - currtime;
                    if (currtime > timer)
                    {
                        TriggerLight(false);
                        currtime = 0;
                    }
                    check += Time.deltaTime;
                    sliderButtery.value = bWorkTime-check;
                }

                if (check > bWorkTime)
                {
                    TriggerLight(false);
                    sliderTime.value = timer; currtime = 0;
                    _object2.SetActive(true);
                }
                else
                {
                }

    }

 }

