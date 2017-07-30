using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToolButton : MonoBehaviour
{
    public int ToolIndex;
    public Text QuantityDisplay;
    public Image Highlight;
    public KeyCode trigger;

    protected abstract int Quantity { get; }

    public void OnClick()
    {
        Level.Active.ToolSelectionIndex = ToolIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(trigger))
        {
            OnClick();
        }
        QuantityDisplay.text = Quantity.ToString();
        Highlight.enabled = (Level.Active.ToolSelectionIndex == ToolIndex);
    }
}