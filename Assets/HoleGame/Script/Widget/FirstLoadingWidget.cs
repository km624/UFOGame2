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
            // 2) �ε� ���൵ ǥ��
            while (!data.IsCompleted)
            {
                // ���൵�� ���� ���α׷����� ������Ʈ
                loadingProgressBar.fillAmount = GameManager.Instance.progress;

                // ���� �����ӱ��� ���
                await Task.Yield();
            }

            // ���������� ���൵ 1 ����
            loadingProgressBar.fillAmount = 1f;

            // 3) �ε� �������� ���� ������ �̵� (��: "MainScene")
            TransitionManager.Instance().Transition("MainScene", transition, 0);

        }
    }

   
  
}
