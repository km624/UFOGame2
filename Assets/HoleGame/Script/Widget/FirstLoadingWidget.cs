using DG.Tweening;
using EasyTransition;
using Lean.Transition;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstLoadingWidget : MonoBehaviour
{

    public Image loadingProgressBar;
    Task data;

    public TransitionSettings transition;

    public GameObject Logo;

    private Tween scaleTween;
    private Tween MoveTween;
    async void Start()
    {

        // 통통 튀는 스케일
       scaleTween = Logo.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.7f)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        // 위아래 둥실
        MoveTween = Logo.transform.DOLocalMoveY(Logo.transform.localPosition.y +50f, 0.7f)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        if (GameManager.Instance)
        {
            data = GameManager.Instance.InitData2();
            // 2) 로딩 진행도 표시
            while (!data.IsCompleted)
            {
                // 진행도에 따라 프로그래스바 업데이트
                loadingProgressBar.fillAmount = GameManager.Instance.progress;

                // 다음 프레임까지 대기
                await Task.Yield();
            }

            // 마지막으로 진행도 1 설정
            loadingProgressBar.fillAmount = 1f;

            // 3) 로딩 끝났으니 다음 씬으로 이동 (예: "MainScene")
            TransitionManager.Instance().Transition("MainScene", transition, 0);

        }
    }

    public void OnDisable()
    {
        scaleTween?.Kill();
        MoveTween?.Kill();
    }



}
