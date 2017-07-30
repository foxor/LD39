using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLineToolButton : ToolButton
{
    protected override int Quantity
    {
        get
        {
            return Level.Active.LinesLeft;
        }
    }
}