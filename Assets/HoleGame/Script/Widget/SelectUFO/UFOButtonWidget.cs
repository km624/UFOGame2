using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UFOButtonWidget : MonoBehaviour
{
    
    private SelectUFOWidget selectUFOWidget;
    private int ufoindex;
    [SerializeField] private Image UFOIcon;
    [SerializeField]private Button UfoSelectbutton;
    [SerializeField] private Image LockImage;
    [SerializeField] private Image SelectFrame;

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

        SelectFrame.gameObject.SetActive(bselect);
        LockImage.gameObject.SetActive(!bIsUnlocked);


    }

    public void OnClickSelectBtn()
    {

        UfoSelectbutton.interactable = false;
        SelectFrame.gameObject.SetActive(true);
        selectUFOWidget.SelectUFOType(ufoindex, bIsUnlocked , ufoPrice);


    }
    public void UnSelect()
    {
       
        UfoSelectbutton.interactable = true;
        SelectFrame.gameObject.SetActive(false);
        //LockImage.gameObject.SetActive(!bIsunlock);
    }

    public void UnlockUFO()
    {
        bIsUnlocked = true;
        UFOIcon.color = UnlockColor;
        LockImage.gameObject.SetActive(!bIsUnlocked);

    }
    
}
