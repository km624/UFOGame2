using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UFOButtonWidget : MonoBehaviour
{
    
    private SelectUFOWidget selectUFOWidget;
    private int ufoindex;
    [SerializeField] private Image UFOIcon;
    [SerializeField]private Button UfoSelectbutton;
    [SerializeField] private Button UfoPurchasebutton;
    [SerializeField] private TMP_Text PriceText;
    private int ufoPrice;

    private Color LockColor = new Color32(64, 64, 64, 255);
    private bool bIsUnlocked;
    


    public void InitializeUFOButton(SelectUFOWidget selectufo, int index, Sprite ufoicon , bool bIsunlock , bool bselect,int price)
    {
        selectUFOWidget = selectufo;
        ufoindex=index;

        if (ufoicon != null)
            UFOIcon.sprite = ufoicon;


        ufoPrice = price;
        PriceText.text = ufoPrice.ToString(); 

        bIsUnlocked =  bIsunlock;
       if(!bIsUnlocked)
            UFOIcon.color = LockColor;

        UfoSelectbutton.interactable = !bselect;
        
        UfoPurchasebutton.gameObject.SetActive(false);

    }

    public void OnClickSelectBtn()
    {

        UfoSelectbutton.interactable = true;
        selectUFOWidget.SelectUFOType(ufoindex,bIsUnlocked);
        
        if(!bIsUnlocked)
            UfoPurchasebutton.gameObject.SetActive(!bIsUnlocked);
    }

    public void UnSelect()
    {
        UfoSelectbutton.interactable = true;
        UfoPurchasebutton.gameObject.SetActive(false);
    }

    public void OnClickPurchaseBtn()
    {
        selectUFOWidget.PurchaseUFO(ufoindex, ufoPrice);
    }

    public void UnlockUFO()
    {
        bIsUnlocked = true;
        UfoPurchasebutton.gameObject.SetActive(false);
    }
    
}
