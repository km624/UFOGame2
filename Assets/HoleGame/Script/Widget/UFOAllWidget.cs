using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using UnityEngine;

public class UFOAllWidget : MonoBehaviour
{
    [SerializeField] private RectTransform UFOSelectWidgetTransform;
    [SerializeField] private RectTransform UFOAllWidgetTransform;
    
    
    [SerializeField] private float slideDuration = 0.6f;
   
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;

    [SerializeField] private SelectUFOWidget selectUFOWidget;
    [SerializeField] private SelectPalletWidget selectPalletWidget;
    [SerializeField] private ReinForceWidget reinForceWidget;

    void Start()
    {
       
    }

    public void TogglePanel()
    {
        if (isVisible)
            HidePanel();
        else
            ShowPanel();
    }
    private void ShowPanel()
    {


        // 블록 끄고 → 끝나면 켜기
        //SetInteractable(false);
        Vector2 shownPos = new Vector2(-800f, 50f);
        UFOSelectWidgetTransform.DOAnchorPos(shownPos, slideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                //SetInteractable(true);
                isVisible = true;
            });
    }

    private void HidePanel()
    {
        // 블록 끄고 → 끝나면 비활성화
        // SetInteractable(false);
        Vector2 hiddenPos = new Vector2(0f, 50f);
        UFOSelectWidgetTransform.DOAnchorPos(hiddenPos, slideDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isVisible = false;
                
            });
    }

    private void SetInteractable(bool state)
    {
        CanvasGroup cg = UFOAllWidgetTransform.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = state;
            cg.blocksRaycasts = state;
        }
    }







}
