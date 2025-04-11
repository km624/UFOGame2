
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class FallingObject : MonoBehaviour
{
    [Header("���")]
    [SerializeField]
    private ShapeEnum Shape = ShapeEnum.baseball;
    [Header("����ġ��")]
    [SerializeField]
    public float EXPCnt { get; private set; }  = 50.0f;
    [SerializeField]
    public float TimeCnt { get; private set; } = 0.5f;

    public float ObjectMass { get; private set; } = 1.0f;


    //[Header("Ŭ���� ��ǥ")]
   /* [SerializeField]
    private bool bRequired = true;*/
    [Header("�̵��ϴ���")]
    [SerializeField]
    private bool bMovement = false;

    private BounceShape idleBounce;
    private ObjectMovement MoveBounce;

    private bool BouncesdActivate = true;

    [SerializeField]
    [Header("���� ������ ")]
    private EarthObjectStatData ForceStatData = null;
    [SerializeField]
    private GameObject CanvasGameObject;
    [SerializeField]
    private Image StateIconWidget;
    [SerializeField]
    private GameObject Ice;

    public UnityEvent<FallingObject> onSwallowed;

    public bool bIsAttacked { get; private set; } = false;

    private Vector3 originalScale;

    
    //private int SpawnLayer = 6;
    private int NormalLayer = 9;

    public void SetStatData(EarthObjectStatData data)
    {
        ForceStatData =  data;

    }

    public EarthObjectStatData GetForceData() { return ForceStatData; }


    private void SetObjectData(EarthObjectStatData data)
    {

        idleBounce = GetComponent<BounceShape>();
        MoveBounce = GetComponent<ObjectMovement>();
    
        EXPCnt = data.EXPCnt;
        TimeCnt = data.TimeCnt;
        bMovement = data.bMovement;
        ObjectMass =data.mass;
        float squishspeed = Mathf.Clamp(data.squishAmount, 0.2f, 0.5f);
        if(idleBounce&& MoveBounce)
        {
            idleBounce.ForceSetBounce(data.squishAmount, squishspeed);
            MoveBounce.SetJupmpDistance(data.jumpDistance);
        }

    }

    public virtual void AddGenerationMass(int generation)
    {
        SetObjectData(ForceStatData);

        ObjectMass += (4 * generation);

        LiftAbsorption lift = GetComponent<LiftAbsorption>();
        if (lift)
        {
            lift.InitiaLiftAbsorption(originalScale, ObjectMass);
        }


        transform.DOScale(originalScale, 1.0f)
                 .SetEase(Ease.OutElastic).OnComplete(() => SelectMove());
    }

    private void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = originalScale * 0.01f;
       
    }

    private void OnEnable()
    {
        if (ForceStatData == null)
        {
            ForceStatData = ScriptableObject.CreateInstance<EarthObjectStatData>();
        }
        

       /* SetObjectData(ForceStatData);

        LiftAbsorption lift = GetComponent<LiftAbsorption>();
        if (lift)
        {
            lift.InitiaLiftAbsorption(originalScale, ObjectMass);
        }

       
        transform.DOScale(originalScale, 1.0f)
                 .SetEase(Ease.OutElastic).OnComplete(() => SelectMove());*/

    }


    public void OnSwallow()
    {
        onSwallowed?.Invoke(this);

    }

    void SelectMove()
    {
       
        gameObject.layer = NormalLayer;

        if (idleBounce != null && MoveBounce != null)
        {
            if (BouncesdActivate)
            {

                MoveBounce.enabled = bMovement ? true : false;
                idleBounce.enabled = bMovement ? false : true;
                //Debug.Log("�ٿ ����");
            }
            else
            {
                //Debug.Log("�ٿ ����");
                MoveBounce.enabled = false;
                idleBounce.enabled = false;

            }
        }
    }

    public void ActivateBounce(bool boucneactive)
    {
        if(boucneactive)
        {
            if (Ice.activeSelf)
            {
                Debug.Log("����ִ���");
                return;
            }
        }
      
            BouncesdActivate = boucneactive;
       
       SelectMove();
    }

    public void ActiveIce(bool active)
    {
        Ice.SetActive(active);
    }
    public void SetStateSpriteIcon(Sprite icon)
    {
        if (icon != null)
            StateIconWidget.sprite = icon;
    }
    public void ActiveStateIconWidget(bool active)
    {
        CanvasGameObject.SetActive(active);
        
      
    }
    public ShapeEnum GetShapeEnum() { return Shape; }
   
}
