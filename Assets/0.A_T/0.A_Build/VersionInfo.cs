using UnityEngine;
using System;
using System.Collections;

// DO NOT MODIFY SOURCE : BuildEdior.cs에서 소스를 수정하므로 내용을 수정하면 안됨 ( 변수 선언 부분은 인덴트와 값도 변경하면 안된다 )
public class VersionInfo
{
    static private bool m_Enable = false;
    static private DateTime m_BuildDate = DateTime.Now;
    static private int m_ExexpireDay = 30;               
    static private string m_Reversion = ""; 

    static public bool CheckExpire()
    {
        if (m_Enable == false)
            return false;

        if(DateTime.Now > m_BuildDate.AddDays(m_ExexpireDay))        
            return true;

        return false;
    }

    static public DateTime GetBuildDate()
    {
        return m_BuildDate;
    }

    static public DateTime GetExpireBuildDate()
    {
        return m_BuildDate.AddDays(m_ExexpireDay);
    }

    static public string GetReversion()
    {
        return m_Reversion;
    }
}
