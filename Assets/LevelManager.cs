using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject Popup;
    public Text Explanation;

    protected Action NextAction;

    public void Awake()
    {
        GameOver.Win += OnWin;
        GameOver.Lose += OnLose;
        Popup.SetActive(false);
        Level.ReadLevel(1);
    }

    private void OnWin()
    {
        StartCoroutine(WinCoroutine());
    }

    protected IEnumerator WaitForAnimations()
    {
        while (BoltController.IsFiring)
        {
            yield return null;
        }
    }

    protected IEnumerator WinCoroutine()
    {
        NextAction = PlayNextLevel;
        yield return WaitForAnimations();
        Popup.SetActive(true);
        yield return IntroController.ShowText("Everyone is so happy the lights came back on!", Explanation);
        yield return new WaitForSeconds(2f);
        Finish();
    }

    private void OnLose(string message)
    {
        StartCoroutine(LoseCoroutine(message));
    }

    private IEnumerator LoseCoroutine(string message)
    {
        NextAction = ResetCurrentLevel;
        yield return WaitForAnimations();
        Popup.SetActive(true);
        yield return IntroController.ShowText(message, Explanation);
        yield return new WaitForSeconds(2f);
        Finish();
    }

    private void Finish()
    {
        StopAllCoroutines();
        Popup.SetActive(false);
        Action reentrancy = NextAction;
        NextAction = null;
        reentrancy();
    }

    private void PlayNextLevel()
    {
        Level.ReadLevel(Level.Active.Number + 1);
    }

    private void ResetCurrentLevel()
    {
        Level.ReadLevel(Level.Active.Number);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            if (NextAction != null)
            {
                Finish();
            }
        }
    }
}