using UnityEngine;
using TMPro;
using Lean.Gui;


public class GameStateWidget : MonoBehaviour
{
    public TMP_Text PlayTimeText;
    public TMP_Text ScoreText;

    public LeanWindow GameStateModal;
    
    public void SetGameState(int totalplaytime ,int totalscore)
    {

        int min = totalplaytime / 60;
        int sec = totalplaytime % 60;
        PlayTimeText.text = min.ToString("D2") + " : " + sec.ToString("D2");
       
        ScoreText.text = totalscore.ToString();

        //star 먼저 리셋
     

        GameStateModal.TurnOn();
        
    }
    
    
}
