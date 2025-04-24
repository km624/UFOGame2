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

    /*── 타이머 관리용 ───────────────────*/
    private Coroutine skillRoutine;
    protected float remainingTime;   // Pause 시 남은 시간

    public void Initialize(UFOPlayer p, SkillManager m, int cnt)
    {
        UFOplayer = p;
        skillManager = m;
        SkillCount = cnt;
        remainingTime = SkillDuration;
    }

    public virtual bool CanUseSkill() => SkillCount > 0 && !bProgressSkill;

    /*── 스킬 사용 ───────────────────────*/
    public int UseSkill()
    {
        if (!CanUseSkill()) { Debug.Log("스킬 사용 불가"); return -1; }

        SkillCount--;
        bProgressSkill = true;
        
        Activate();

        remainingTime = SkillDuration;
        skillRoutine = StartCoroutine(SkillTimer());

        return SkillCount;
    }

    /*── 일시정지 / 재개 인터페이스 ──────*/
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

    /*── 실제 타이머 ─────────────────────*/
    private IEnumerator SkillTimer()
    {
        while (remainingTime > 0f)
        {
            yield return null;
            remainingTime -= Time.unscaledDeltaTime;   // TimeScale 영향 안 받음
        }

        bProgressSkill = false;
        Deactivate();
    }

    /*── 파생 클래스에서 구현할 부분 ───────*/
    public abstract void Activate();
    public abstract void Deactivate();
}
