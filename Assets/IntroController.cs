using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public static readonly string FULL_TEXT = "Renewable energy is great, but we used it all.\n\nWe covered the earth with solar panels and wind turbines.\n\nWe dammed up every river.\n\nNow we're running out of oil and coal too.\n\nIt's time to enter the...";

    public Text Terminal;
    public GameObject Title;

    protected bool showingText;
    protected bool showingTitle;

	void Start ()
    {
        Title.SetActive(false);
        StartCoroutine(TextIntro());
	}

    public static IEnumerator ShowText(string text, Text textField, bool finishNow = false)
    {
        if (finishNow)
        {
            textField.text = text;
            yield break;
        }

        textField.text = "";
        for (int i = 1; i <= text.Length; i++)
        {
            if (text[i - 1] == '\n')
            {
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
            }
            textField.text = text.Substring(0, i);
        }
        yield return new WaitForSeconds(0.55f);
    }

    private IEnumerator TextIntro()
    {
        showingText = true;
        yield return ShowText("|", Terminal);
        yield return ShowText("", Terminal);
        yield return ShowText("|", Terminal);
        yield return ShowText("", Terminal);
        yield return ShowText("|", Terminal);
        yield return ShowText("", Terminal);
        yield return ShowText(FULL_TEXT, Terminal);
        showingText = false;
        yield return EndOfText();
    }

    private IEnumerator EndOfText(bool skippedAlready = false)
    {
        showingText = false;
        yield return ShowText(FULL_TEXT, Terminal, true);
        if (skippedAlready)
        {
            yield return new WaitForSeconds(4f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        ShowTitle();
    }

    private void ShowTitle()
    {
        showingTitle = true;
        StopAllCoroutines();
        Terminal.enabled = false;
        showingText = false;
        StartCoroutine(TitleCoroutine());
    }

    private IEnumerator TitleCoroutine()
    {
        Title.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            if (showingText)
            {
                StopAllCoroutines();
                StartCoroutine(EndOfText(true));
            }
            else if (showingTitle)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                ShowTitle();
            }
        }
    }
}
