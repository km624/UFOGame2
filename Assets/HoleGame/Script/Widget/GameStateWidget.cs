using UnityEngine;
using TMPro;
using Lean.Gui;


public class GameStateWidget : MonoBehaviour
{
    public TMP_Text PlayTimeText;
    public TMP_Text ScoreText;
    public TMP_Text StarCntText;
    public LeanWindow GameStateModal;
    
    public void SetGameState(int totalplaytime ,int totalscore ,int starcnt)
    {

        int min = totalplaytime / 60;
        int sec = totalplaytime % 60;
        PlayTimeText.text = min.ToString("D2") + " : " + sec.ToString("D2");
       
        ScoreText.text = totalscore.ToString();

        //star ���� ����
        StarCntText.text = starcnt.ToString();

        GameStateModal.TurnOn();
        
    }

    
}
