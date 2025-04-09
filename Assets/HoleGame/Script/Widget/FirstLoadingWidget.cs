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

    async void Start()
    {
        if(GameManager.Instance)
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

   
  
}
