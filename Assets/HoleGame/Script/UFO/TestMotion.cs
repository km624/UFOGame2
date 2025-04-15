using DG.Tweening;
using UnityEngine;

public class TestMotion : MonoBehaviour
{
    [Header("���� ��¦ �ö� ����")]
    public float upOffset = 0.7f;

    [Header("�ö� �� �ɸ��� �ð�(��)")]
    public float upDuration = 0.5f;

    [Header("������ �� �ɸ��� �ð�(��)")]
    public float downDuration = 0.2f;


    public Vector3 defaultscale;

    public bool test;

   
    private void Start()
    {

        defaultscale = transform.localScale;
        if (test)
        {
            MotionStart();
        }
        else { MotionStart2(); }
        
      
    }

    public void MotionStart()
    {

        Sequence seq = DOTween.Sequence();

        // 1) �ö󰡸鼭 UFO�� ��¦ �����ϰ� �����
        //    - y������ �ö󰡸鼭 x�� �ø��� y�� ���̴� ������ ����
        seq.Append(
            transform.DOMoveY(transform.position.y + upOffset, upDuration)
                     .SetEase(Ease.OutQuad) // ������ ������ ��ȣ�� ���� ����
        );
        // squashScaleRatio�� Ȱ���ؼ� x/y�� ���� ������
        seq.Join(
            transform.DOScale(new Vector3(
               defaultscale.x*0.9f,
                defaultscale.y *1.4f,
                defaultscale.z*0.9f),
                upDuration
            ).SetEase(Ease.OutBack)
        );

        // 2) ������ �� UFO�� ������ 0.3 -> 0.8�� �ڿ������� Ű��鼭
        //    y���� endY���� �̵�
        seq.Append(
            transform.DOMoveY(transform.position.y, downDuration)
                     .SetEase(Ease.InQuad)
        );
        seq.Join(
            transform.DOScale(new Vector3(
                defaultscale.x * 1.3f,
                defaultscale.y * 0.7f,
                defaultscale.z *1.3f),
                downDuration
            ).SetEase(Ease.OutQuad)
        );

        // 3) ���������� x,y,z�� endScale�� ���������� ���� (��¦ ��Ǯ�� �� ���� �ǵ��� �������)
        //    - �ʿ� ������ ���� ��

        seq.Append(
           transform.DOMoveY(transform.position.y, 0.2f)
                    .SetEase(Ease.InQuad)
        );
        seq.Join(
           transform.DOScale(defaultscale, 0.2f)
                    .SetEase(Ease.Linear)
       );

        // ������ ���
        seq.SetLoops(-1, LoopType.Restart);
    }

    public void MotionStart2()
    {

        Sequence seq = DOTween.Sequence();

        // 1) �ö󰡸鼭 UFO�� ��¦ �����ϰ� �����
        //    - y������ �ö󰡸鼭 x�� �ø��� y�� ���̴� ������ ����
      
        // squashScaleRatio�� Ȱ���ؼ� x/y�� ���� ������
        seq.Append(
            transform.DOScale(new Vector3(
               defaultscale.x * 0.7f,
                defaultscale.y * 1.4f,
                defaultscale.z * 0.7f),
                upDuration
            ).SetEase(Ease.OutBack)
        );

        // 2) ������ �� UFO�� ������ 0.3 -> 0.8�� �ڿ������� Ű��鼭
        //    y���� endY���� �̵�
       
      
        seq.Append(
            transform.DOScale(new Vector3(
                defaultscale.x * 1.2f,
                defaultscale.y * 0.8f,
                defaultscale.z * 1.2f),
                downDuration
            ).SetEase(Ease.OutQuad)
        );

        // 3) ���������� x,y,z�� endScale�� ���������� ���� (��¦ ��Ǯ�� �� ���� �ǵ��� �������)
        //    - �ʿ� ������ ���� ��

        seq.Append(
           transform.DOScale(defaultscale, 0.2f)
                    .SetEase(Ease.Linear)
       );

        // ������ ���
        seq.SetLoops(-1, LoopType.Restart);
    }
}
