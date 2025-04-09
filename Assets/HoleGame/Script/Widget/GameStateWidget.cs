using UnityEngine;
using TMPro;
using Lean.Gui;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStateWidget : MonoBehaviour
{
    public TMP_Text GameStateText;
    public TMP_Text TimeText;

    public LeanWindow GameStateModal;
    [SerializeField]
    private List<Image> Star = new List<Image>();
    public void SetGameState(int time , bool GameClear, int star)
    {
        string GameText;
        if (GameClear)
            GameText = "Game Clear";
        else
            GameText = "Game Over";
        GameStateText.text = GameText;

        //star 먼저 리셋
        foreach (Image image in Star)
        {
            image.gameObject.SetActive(false);
        }

        for(int i=0;i< star; i++)
        {
            Star[i].gameObject.SetActive(true);
        }

        if(!GameClear)
            TimeText.gameObject.SetActive(false);

        int min = time / 60;
        int sec = time % 60;
        TimeText.text = min.ToString("D2") + " : " + sec.ToString("D2");

        GameStateModal.TurnOn();
        
    }
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
