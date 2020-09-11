using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Flashlight : BaseObject
{
    private KeyCode control = KeyCode.F;
    public Light _light;

    public float timeout = 2;
    public float currTime;
    public float sumCurrTime; //счетчик общего отработанного времени
    //For Battery
    public float batteryCharge = 8;

    private float currReloadTime;
    public Slider timeoutSlider;
    public Slider butterySlider;
    public bool TimerWorkable = true;
    public bool FlashLightWorkable = true;

    protected override void Awake()
    {
        base.Awake();
        _light = GetComponentInChildren<Light>();
        timeoutSlider = GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        butterySlider = GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(3).GetChild(1).GetComponent<Slider>();
        timeoutSlider.maxValue = timeout;
        butterySlider.maxValue = batteryCharge;
    }

    //private void OnOff(bool val)
    //{
    //    _light.enabled = val;
    //}


    void Start()
    {
        //timeoutSlider.value = timeout;

    }

    void Update()
    {

    //    if (Input.GetKeyDown(control) && !_light.enabled && (batteryCharge - sumCurrTime) > 0)
    //    {
    //        OnOff(true);

    //    }
    //    else if (Input.GetKeyDown(control) && _light.enabled && (batteryCharge - sumCurrTime) > 0)
    //    {
    //        OnOff(false);
    //    }

    //    if (_light.enabled)
    //    {
    //        currTime += Time.deltaTime;
    //        sumCurrTime += Time.deltaTime;

    //        if (currTime > timeout)
    //        {
    //            TimerWorkable = false;
    //            currTime = 0;
    //            _light.enabled = false;
    //            onTimerOff = true;
    //            OnOff(false);
    //        }
    //    }
    //    if ((batteryCharge - sumCurrTime) <= 0)
    //    {
    //        FlashLightWorkable = false;
    //        timeoutSlider.gameObject.SetActive(false);
    //        OnOff(false);
    //    }

    //    timeoutSlider.value = timeout - currTime;
    //    butterySlider.value = batteryCharge - sumCurrTime;
    }

}
