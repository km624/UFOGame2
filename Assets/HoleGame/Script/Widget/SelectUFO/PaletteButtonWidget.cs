using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButtonWidget : MonoBehaviour
{
    private SelectPaletteWidget selectpaletteWidget;
    private int colorindex;
    [SerializeField] private Image ColorIcon;
    [SerializeField] private Button ColorSelectbutton;
    [SerializeField] private Image LockImage;
    [SerializeField] private Button ColorPurchasebutton;
    [SerializeField] private TMP_Text PriceText;
    private int ColorPrice;

    private Color LockColor = new Color32(64, 64, 64, 255);
    private Color UnlockColor = new Color32(255, 255, 255, 255);
    private bool bIsUnlocked;



    public void InitializePaletteButton(SelectPaletteWidget selectpalette, int index, Color32 iconcolor,int price, bool bselect,bool bIsUnlock)
    {
        selectpaletteWidget = selectpalette;
        colorindex = index;

        ColorIcon.color = iconcolor;


        ColorPrice = price;

        PriceText.text = ColorPrice.ToString();

        bIsUnlocked = bIsUnlock;

       
        LockImage.gameObject.SetActive(!bIsUnlock);

        ColorSelectbutton.interactable = !bselect;

        ColorPurchasebutton.gameObject.SetActive(false);

    }

    public void OnClickSelectBtn()
    {

        ColorSelectbutton.interactable = false;

        LockImage.gameObject.SetActive(false);

        selectpaletteWidget.SelectColor(colorindex,bIsUnlocked);

        if (!bIsUnlocked)
            ColorPurchasebutton.gameObject.SetActive(!bIsUnlocked);
    }

    public void UnSelect()
    {

        ColorSelectbutton.interactable = true;
       
       LockImage.gameObject.SetActive(!bIsUnlocked);

        ColorPurchasebutton.gameObject.SetActive(false);
    }

    public void OnClickPurchaseBtn()
    {
        selectpaletteWidget.PurchaseColor(colorindex, ColorPrice);
    }

    public void UnlockPalette()
    {
        bIsUnlocked = true;
        LockImage.gameObject.SetActive(false);
        ColorPurchasebutton.gameObject.SetActive(false);
    }
}
