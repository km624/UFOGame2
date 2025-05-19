using UnityEngine;
using TMPro;

using DG.Tweening;
using System.Collections;


public class TimerWidget : MonoBehaviour
{
    public TMP_Text Minute;
    public TMP_Text Second;

    private int TotalSecond;

    [SerializeField]public Color dangerColor = Color.red;
    public float pulseScale = 1.3f;
    public float pulseDuration = 0.2f;

    private Color defaultColor;
    private Vector3 defaultScale;

    private Coroutine dangerSoundCoroutine;

    private void Awake()
    {
        defaultColor = Second.color;
        defaultScale = Second.transform.localScale;
    }


    public void SetTime(float totaltime)
    {
        TotalSecond = (int)totaltime;
        CallBack_UpdateTimeText(TotalSecond);
    }
   
    public void CallBack_UpdateTimeText(int total)
    {
        int min = total / 60;
        int sec = total % 60;
        Minute.text = min.ToString("D2")+" :";
        Second.text = sec.ToString("D2");
        
        // 5�� ������ ���� �ִϸ��̼�
        if (total <= 5)
        {
            // �÷� ����
            Second.color = dangerColor;
            Minute.color = dangerColor; 
            
            // �ؽ�Ʈ �ñ� ȿ��
           transform.DOKill(); // ���� Ʈ�� ����
           transform.localScale = defaultScale; // ����
           transform.DOPunchScale(Vector3.one * (pulseScale - 1f), pulseDuration, 1, 0.5f);

            if (dangerSoundCoroutine == null)
                dangerSoundCoroutine = StartCoroutine(DangerSoundRoutine());

            if(total<=0)
            {
                StopCoroutine(dangerSoundCoroutine);
                dangerSoundCoroutine = null;
            }
        }
        else
        {
            // ���� ����/�����Ϸ� ����
            Second.color = defaultColor;
            Minute.color = defaultColor;
           transform.DOKill();
            transform.localScale = defaultScale;

            if (dangerSoundCoroutine != null)
            {
                StopCoroutine(dangerSoundCoroutine);
                dangerSoundCoroutine = null;
            }
        }
    }

    private IEnumerator DangerSoundRoutine()
    {
        while (true)
        {
            GameManager.Instance.soundManager.PlaySfx(SoundEnum.DangerSFX, 0.5f);
            yield return new WaitForSeconds(0.5f); // 1�ʸ��� �ݺ�
        }
    }
}

  


