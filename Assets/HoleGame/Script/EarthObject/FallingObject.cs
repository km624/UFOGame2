
using UnityEngine;
using UnityEngine.UI;


public class FallingObject : MonoBehaviour
{
    [Header("���")]
    public ShapeEnum Shape = ShapeEnum.baseball;
    [Header("����ġ��")]
    [SerializeField]
    public float EXPCnt  = 50.0f;
    [Header("Ŭ���� ��ǥ")]
    [SerializeField]
    private bool bRequired = true;
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

    

    private void OnValidate()
    {
        // ���� Shape�� Bomb��� bRequired�� false�� ����
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

    public bool GetRequired() { return bRequired; }
}
