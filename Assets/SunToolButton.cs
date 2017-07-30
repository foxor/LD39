using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunToolButton : ToolButton
{
    protected override int Quantity
    {
        get
        {
            return Level.Active.PowerLeft;
        }
    }
}