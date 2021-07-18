using KZLib;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class SimpleTest
{
    public static readonly System.Random rand = new System.Random();

    void I(string str,params object[] args)
    {
        Log.Normal.I(str,args);
    }

    [Test]
    public void Test()
    {

    }

    [Test]
    public void Test2()
    {
        I($"{Marshal.SizeOf(new Line())}");
    }
}

