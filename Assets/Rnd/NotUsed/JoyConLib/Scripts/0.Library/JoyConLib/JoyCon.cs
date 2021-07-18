using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJoyCon
{
    void SetStick(Vector2 _stick);
    void SetGyro(Vector3 _gyro);
    void SetAccel(Vector3 _accel);
    void SetOrientation(Quaternion _orientation);

    void SetButtonDown(JoyConLib.Button _button);
    void SetButton(JoyConLib.Button _button);
    void SetButtonUp(JoyConLib.Button _button);
}