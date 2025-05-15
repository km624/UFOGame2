
using UnityEngine;
using UnityEngine.UI;

public class UFOButtonWidget : MonoBehaviour
{
    
    private SelectUFOWidget selectUFOWidget;
    private string ufoName;
    [SerializeField] private Image UFOIcon;
    [SerializeField]private Button UfoSelectbutton;
    [SerializeField] private Image LockImage;
    [SerializeField] private Image SelectFrame;
    [SerializeField] private Sprite RewardIcon;
    [SerializeField] private Sprite LockIcon;

    private int ufoPrice;

    private Color LockColor = new Color32(64, 64, 64, 255);
    private Color UnlockColor = new Color32(255,255, 255, 255); 
    private bool bIsUnlocked;

    private bool bIsReward;
    


    public void InitializeUFOButton(SelectUFOWidget selectufo, string ufoname, Sprite ufoicon , bool bIsunlock , bool bselect,int price ,bool bisreward)
    {
        selectUFOWidget = selectufo;
        ufoName = ufoname;

        if (ufoicon != null)
            UFOIcon.sprite = ufoicon;


        ufoPrice = price;
       

        bIsUnlocked =  bIsunlock;
        bIsReward = bisreward;

        if (!bIsUnlocked)
            UFOIcon.color = LockColor;

        if (bIsReward)
            LockImage.sprite = RewardIcon;
        else
        {
            LockImage.sprite = LockIcon;
        }

        UfoSelectbutton.interactable = !bselect;

       

        SelectFrame.gameObject.SetActive(bselect);
        LockImage.gameObject.SetActive(!bIsUnlocked);
        

    }

    public void OnClickSelectBtn()
    {

        UfoSelectbutton.interactable = false;
        SelectFrame.gameObject.SetActive(true);
        selectUFOWidget.SelectUFOType(ufoName, bIsUnlocked , ufoPrice , bIsReward);


    }

    public void OnSelect()
    {
        SelectFrame.gameObject.SetActive(true);
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
