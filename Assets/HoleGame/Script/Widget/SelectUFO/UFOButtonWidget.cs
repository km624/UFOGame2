using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UFOButtonWidget : MonoBehaviour
{
    
    private SelectUFOWidget selectUFOWidget;
    private int ufoindex;
    [SerializeField] private Image UFOIcon;
    [SerializeField]private Button UfoSelectbutton;
   
    private int ufoPrice;

    private Color LockColor = new Color32(64, 64, 64, 255);
    private Color UnlockColor = new Color32(255,255, 255, 255); 
    private bool bIsUnlocked;
    


    public void InitializeUFOButton(SelectUFOWidget selectufo, int index, Sprite ufoicon , bool bIsunlock , bool bselect,int price)
    {
        selectUFOWidget = selectufo;
        ufoindex=index;

        if (ufoicon != null)
            UFOIcon.sprite = ufoicon;


        ufoPrice = price;
       

        bIsUnlocked =  bIsunlock;
       if(!bIsUnlocked)
            UFOIcon.color = LockColor;

        UfoSelectbutton.interactable = !bselect;
        
    

    }

    public void OnClickSelectBtn()
    {

        UfoSelectbutton.interactable = false;
        selectUFOWidget.SelectUFOType(ufoindex, bIsUnlocked , ufoPrice);


    }
    public void UnSelect()
    {
       
        UfoSelectbutton.interactable = true;
       
    }

    public void UnlockUFO()
    {
        bIsUnlocked = true;
        UFOIcon.color = UnlockColor;
       
    }
    
}
