
using UnityEngine;
using UnityEngine.UI;


public class FallingObject : MonoBehaviour
{
    [Header("모습")]
    public ShapeEnum Shape = ShapeEnum.baseball;
    [Header("경험치량")]
    [SerializeField]
    public float EXPCnt  = 50.0f;
    [Header("클리어 목표")]
    [SerializeField]
    private bool bRequired = true;
    [Header("이동하는지")]
    [SerializeField]
    private bool bMovement = false;

    private BounceShape idleBounce;
    private ObjectMovement MoveBounce;

    private bool BouncesdActivate = true;

    [SerializeField]
    [Header("스텟 데이터 ")]
    private EarthObjectStatData ForceStatData = null;
    [SerializeField]
    private GameObject CanvasGameObject;
    [SerializeField]
    private Image StateIconWidget;
    [SerializeField]
    private GameObject Ice;

    

    private void OnValidate()
    {
        // 만약 Shape가 Bomb라면 bRequired를 false로 설정
        if (Shape == ShapeEnum.boomb)
        {
            bRequired = false;
        }
    }

    public void SetStatData(EarthObjectStatData data)
    {
        ForceStatData =  data;
    }

    public EarthObjectStatData GetForceData() { return ForceStatData; }


    private void SetObjectData(EarthObjectStatData data)
    {

        idleBounce = GetComponent<BounceShape>();
        MoveBounce = GetComponent<ObjectMovement>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null ) rb.mass = data.mass;

        EXPCnt = data.EXPCnt;
        bRequired = data.bRequired;
        bMovement = data.bMovement;

        float squishspeed = Mathf.Clamp(data.squishAmount, 0.2f, 0.5f);
        if(idleBounce&& MoveBounce)
        {
            idleBounce.ForceSetBounce(data.squishAmount, squishspeed);
            MoveBounce.SetJupmpDistance(data.jumpDistance);
        }
      
       
    }

    private void OnEnable()
    {
        if (ForceStatData == null)
        {
            ForceStatData = ScriptableObject.CreateInstance<EarthObjectStatData>();
        }


        SetObjectData(ForceStatData);


        if (bRequired)
        {
            if(Shape !=ShapeEnum.boomb)
            {
               GameState.Instance.RegisterFallingObject(this);
            }

           
        }
            
       
    }

    void Start()
    {

       /* idleBounce = GetComponent<BounceShape>();
        MoveBounce = GetComponent<ObjectMovement>();
*/
      
        SelectMove();

    }

    
    void Update()
    {
        
    }

    void SelectMove()
    {
        if (idleBounce != null && MoveBounce != null)
        {
            if (BouncesdActivate)
            {

                MoveBounce.enabled = bMovement ? true : false;
                idleBounce.enabled = bMovement ? false : true;
                //Debug.Log("바운스 실행");
            }
            else
            {
                //Debug.Log("바운스 없음");
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
                Debug.Log("얼어있는중");
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

    public bool GetRequired() { return bRequired; }
}
