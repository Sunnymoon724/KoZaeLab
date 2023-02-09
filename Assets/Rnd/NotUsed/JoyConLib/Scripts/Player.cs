using JCLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IJoyCon
{
    public TextMesh text;
    private int index;

    public void SetAccel(Vector3 _accel)
    {

    }

    public void SetButton(JoyConLib.Button _button)
    {

    }

    public void SetButtonDown(JoyConLib.Button _button)
    {
        if(_button == JoyConLib.Button.SHOULDER_1)
        {
            transform.localPosition = Vector3.zero;
        }
        else if (_button == JoyConLib.Button.SHOULDER_2)
        {
            JoyConMgr.In.SetVibration(index);
        }

        text.text = _button.ToString();
    }

    public void SetButtonUp(JoyConLib.Button _button)
    {

    }

    public void SetGyro(Vector3 _gyro)
    {

    }

    public void SetOrientation(Quaternion _orientation)
    {

    }

    public void SetStick(Vector2 _stick)
    {
        transform.Translate(new Vector3(-_stick.x,0.0f,_stick.y));
    }

    void Start()
    {
        index = JoyConMgr.In.AddJoyCon(this);
    }
}
