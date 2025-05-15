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

        // ���� Ƣ�� ������
       scaleTween = Logo.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.7f)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        // ���Ʒ� �ս�
        MoveTween = Logo.transform.DOLocalMoveY(Logo.transform.localPosition.y +50f, 0.7f)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        if (GameManager.Instance)
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

    public void OnDisable()
    {
        scaleTween?.Kill();
        MoveTween?.Kill();
    }



}
