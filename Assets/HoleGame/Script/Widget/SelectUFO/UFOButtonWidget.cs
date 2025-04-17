using UnityEngine;
using UnityEngine.UI;

public class UFOButtonWidget : MonoBehaviour
{
    
    private SelectUFOWidget selectUFOWidget;
    private int ufoindex;
    private Image UFOIcon;
    private Button Ufobutton;

    private bool bIsUnlocked;

   

    void InitializeUFOButton(SelectUFOWidget selectufo, int index, Sprite ufoicon , bool bIsunlock , bool bselect)
    {
        selectUFOWidget = selectufo;
        ufoindex=index;

        if (ufoicon != null)
            UFOIcon.sprite = ufoicon;

        bIsUnlocked =  bIsunlock;


        Ufobutton.interactable = bselect;

    }

    public void OnClickBtn()
    {

        Ufobutton.interactable = true;
    }

    public void UnSelect()
    {
        Ufobutton.interactable = false;
    }

    public void UnlockUFO()
    {
        bIsUnlocked = true;
    }
    
}
