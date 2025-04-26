using System.Collections.Generic;

using UnityEngine;

public class MagneticTrigger : MonoBehaviour
{
   
    private Transform UFOtransform;
    private float MindControlTime = 0.0f;
    private Sprite StateIcon;

    public List<ParticleSystem> myParticlelist = new List<ParticleSystem>();

    private float EffectDuringTime;
    private void SetParticleLifetime(float lifetime)
    {
        foreach (var particle in myParticlelist)
        {
            var main = particle.main;
            main.startLifetime = lifetime;
        }

    }
    public void SetMindcontrolData(Transform Ufotransform, float time,Sprite icon,float effecttime )
    {
        UFOtransform = Ufotransform;
        MindControlTime = time;
        StateIcon = icon;
        EffectDuringTime = effecttime;
        SetParticleLifetime(EffectDuringTime);
    }

    //List<ObjectMovement> MagneticObjects = new List<ObjectMovement>();
  /*  private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (other.GetComponent<BossObject>() != null) return;

            FallingObject FObject = other.GetComponent<FallingObject>();
            if (FObject != null)
            {
                FObject.SetStateSpriteIcon(StateIcon);
            }
        }
    }
*/
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer==9) 
        {

            if (other.GetComponent<BossObject>() != null) return;

            ObjectMovement movement = other.GetComponent<ObjectMovement>();
            
            FallingObject FObject = other.GetComponent<FallingObject>();
            if (FObject != null)
            {
                FObject.SetStateSpriteIcon(StateIcon);
            }
            if (movement != null && movement.enabled)
            {
                
                movement.ForceSetMoveOBject(UFOtransform, MindControlTime);
            }
        }
    }
   /* private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            ObjectMovement movement = other.GetComponent<ObjectMovement>();
            if (movement != null)
            {
                if (MagneticObjects.Contains(movement))
                {
                    movement.ForceSetJumpDirection(Vector3.zero);
                    MagneticObjects.Remove(movement);
                }
                    

            }
        }
    }
    void OnDestroy()
    {
        ResetAllObjects();
        UFOtransform=null;
    }*/



    // ��� MagneticObjects�� ForceSetJumpDirection(Vector3.zero) ȣ��
    /*private void ResetAllObjects()
    {
        foreach (var movement in MagneticObjects)
        {
            if (movement != null)
            {
                movement.ForceSetJumpDirection(Vector3.zero);
            }
        }
        MagneticObjects.Clear();
    }*/

   /* Vector3 CalculateUfoDirection(Transform objecttransform)
    {
        // UFO�� ��ġ
        Vector3 ufoPosition = UFOtransform.position;

        // ������Ʈ�� ��ġ
        Vector3 objectPosition = objecttransform.position;

        // Y ���� �����ϰ� ���� (���̸� �����ϱ� ����)
        ufoPosition.y = 0;
        objectPosition.y = 0;

        // UFO ���� ���� ��� (���� ����)
        Vector3 directionToUFO = (ufoPosition - objectPosition).normalized;

        return directionToUFO;
      *//*  // �����: �� �信�� ���� ǥ��
        Debug.DrawLine(objectPosition, objectPosition + directionToUFO * 5f, Color.red);*//*

        
    }*/
}
