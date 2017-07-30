using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tileButton : MonoBehaviour
{
    public static event Action<int, int> ClickedTile = (x, y) => { };

	public int x;
	public int y;
    public Sprite building;
    public Sprite powerPlant;
    public Sprite inactiveLine;
    public Sprite activeLine;
    public Sprite boulder;
    public Sprite sun;
    public Sprite empty;
    public Sprite buildingLocked;
    public Text number;

    protected Image display;

    public void Awake()
    {
        display = GetComponent<Image>();
    }

    public void Update()
    {
        TileInfo myInfo = Level.Active.Tiles[x, y];

        number.enabled = myInfo.ContainsBuilding && myInfo.RemainingMagnitude != 0;

        if (myInfo.ContainsActiveLine)
        {
            display.sprite = activeLine;
        }
        else if (myInfo.ContainsBuilding)
        {
            display.sprite = myInfo.RemainingMagnitude == 0 ? buildingLocked : building;
            number.text = myInfo.RemainingMagnitude.ToString();
        }
        else if (myInfo.ContainsInactiveLine)
        {
            display.sprite = inactiveLine;
        }
        else if (myInfo.ContainsPowerPlant && !myInfo.SuppressPowerPlant)
        {
            display.sprite = powerPlant;
        }
        else if (myInfo.ContainsBoulder)
        {
            display.sprite = boulder;
        }
        else if (myInfo.ContainsSun)
        {
            display.sprite = sun;
        }
        else
        {
            display.sprite = empty;
        }
    }

    public void OnClicked()
    {
        ClickedTile(x, y);
    }
}
