using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextManager : MonoBehaviour
{
    public Text text;

    private void Update()
    {
        text.text = "LEVEL: " + Level.Active.Number + (Level.Active.Number > 11 ? " (Random)" : "");
    }
}