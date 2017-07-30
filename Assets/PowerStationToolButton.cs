using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStationToolButton : ToolButton
{
    protected override int Quantity
    {
        get
        {
            return Level.Active.PlantsLeft;
        }
    }
}