using UnityEngine;
using System.Collections;

public abstract  class SkillBase : MonoBehaviour
{
    public int SkillCount { get; private set; }
    public Sprite SkillIcon;
    public float SkillDuration = 3f;

   // protected bool bProgressSkill = false;
    public bool bProgressSkill { get; private set; }

    protected UFOPlayer UFOplayer;
    protected SkillManager skillManager;

    /*���� Ÿ�̸� ������ ��������������������������������������*/
    private Coroutine skillRoutine;
    protected float remainingTime;   // Pause �� ���� �ð�

    public void Initialize(UFOPlayer p, SkillManager m, int cnt)
    {
        UFOplayer = p;
        skillManager = m;
        SkillCount = cnt;
        remainingTime = SkillDuration;
    }

    public virtual bool CanUseSkill() => SkillCount > 0 && !bProgressSkill;

    /*���� ��ų ��� ����������������������������������������������*/
    public int UseSkill()
    {
        if (!CanUseSkill()) { Debug.Log("��ų ��� �Ұ�"); return -1; }

        SkillCount--;
        bProgressSkill = true;
        
        Activate();

        remainingTime = SkillDuration;
        skillRoutine = StartCoroutine(SkillTimer());

        return SkillCount;
    }

    /*���� �Ͻ����� / �簳 �������̽� ������������*/
    public virtual void PauseSkill()
    {
        if (!bProgressSkill) return;
        if (skillRoutine != null)
        {
            StopCoroutine(skillRoutine);
            Deactivate();
            skillRoutine = null;
        }
    }

    public virtual void ResumeSkill()
    {
        if (!bProgressSkill) return;
        Activate();
        skillRoutine = StartCoroutine(SkillTimer());
    }

    /*���� ���� Ÿ�̸� ������������������������������������������*/
    private IEnumerator SkillTimer()
    {
        while (remainingTime > 0f)
        {
            yield return null;
            remainingTime -= Time.unscaledDeltaTime;   // TimeScale ���� �� ����
        }

        bProgressSkill = false;
        Deactivate();
    }

    /*���� �Ļ� Ŭ�������� ������ �κ� ��������������*/
    public abstract void Activate();
    public abstract void Deactivate();
}
