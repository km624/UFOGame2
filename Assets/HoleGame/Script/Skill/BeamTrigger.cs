using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BeamTrigger : MonoBehaviour
{
    private Transform GunTransform;
    private Transform FireTransform;
    private UFOPlayer player;
    private List<FallingObject> Inrange = new List<FallingObject>();

    private Coroutine BeamCoroutine = null;
    // 1�ʸ��� üũ
    private float BeamInterval = 0.0f;

    private GameObject Beamobject = null;

    private float BeamActiveTime = 0.1f;
    private float RotateDuration = 0.2f;
    public void SetBeamData(Transform gun , float interval,GameObject beamprefab, UFOPlayer ufoplayer)
    {
        GunTransform = gun;
        BeamInterval = interval;
        Beamobject = beamprefab;
        Beamobject.SetActive(false);
        FireTransform = GunTransform.GetChild(0).transform;
        player = ufoplayer;


    }

  

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {


            FallingObject FObject = other.GetComponent<FallingObject>();
            if (FObject != null)
            {
                //�÷��̾� ���� ���� ������ return
                if (player.CurrentLevel < FObject.ObjectMass) return;

                BossObject bossObject = other.GetComponent<BossObject>();
                //���� ������Ʈ�� �� Ȱ��ȭ X
                if (bossObject != null) return;

                if (!Inrange.Contains(FObject))
                {
                    Inrange.Add(FObject);
                    if (BeamCoroutine == null)
                    {

                        BeamCoroutine = StartCoroutine(DestroyClosestRoutine());

                    }

                }

            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {

            FallingObject FObject = other.GetComponent<FallingObject>();
            if (FObject != null)
            {
                if (Inrange.Contains(FObject))
                {
                    Inrange.Remove(FObject);
                }

            }

        }
    }

    private void OnDestroy()
    {
       
        if(BeamCoroutine!=null)
            StopCoroutine(BeamCoroutine);
    }

    private IEnumerator DestroyClosestRoutine()
    {
        while (true)
        {
           
            Inrange.RemoveAll(item => item == null);
            if (Inrange.Count > 0)
            {
              
                FallingObject closestObj = null;
                float closestDist = Mathf.Infinity;
                Vector3 triggerPos = transform.position;

                // ����Ʈ ��ȸ
                foreach (FallingObject obj in Inrange)
                {
                    if (obj.gameObject.layer == 9)
                    {
                        // ������Ʈ�� �̹� �ı��Ǿ����� �ǳʶݴϴ�.
                        if (obj == null)
                            continue;

                        float dist = Vector3.Distance(triggerPos, obj.transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestObj = obj;
                        }
                    }
                }

                // ���� ����� ������Ʈ�� �ִٸ� �ı��ϰ� ����Ʈ���� ����
                if (closestObj != null)
                {
                    // ���� �ִٸ�, �ı� ��� ������Ʈ �������� ȸ���ϵ��� ����
                    if (GunTransform != null)
                    {
                        Vector3 direction = closestObj.transform.position - GunTransform.position;
                        if (direction != Vector3.zero)
                        {
                          
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                          
                            // DOTween�� ����� �ε巴�� ȸ�� (0.2��)
                            //GunTransform.DORotateQuaternion(targetRotation, RotateDuration).OnComplete(() => DestroyObject(closestObj));
                            Sequence sequence = DOTween.Sequence();

                            sequence.Append(GunTransform.DORotateQuaternion(targetRotation, RotateDuration))
                                    .AppendCallback(() => BeamActive(closestObj.transform.position))  // �� Ȱ��ȭ
                                    .AppendInterval(BeamActiveTime)         // 0.5�� ���� �� ����
                                    .OnComplete(() => DestroyObject(closestObj));  // �� ��Ȱ��ȭ �� Ÿ�� �ı�
                        }
                        else
                        {
                            DestroyObject(closestObj); // �ٷ� �ı�
                        }
                    }


                }
            }
            else
            {
                BeamCoroutine = null;
                yield break;
            }
            yield return new WaitForSeconds(BeamInterval);
        }
    }

    public void BeamActive(Vector3 targetpostion)
    {
        float distance = Vector3.Distance(FireTransform.position, targetpostion);

        Vector3 direction = targetpostion - FireTransform.position;

        Beamobject.transform.localScale = new Vector3(0.1f, 0.1f, distance);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Beamobject.transform.rotation = targetRotation;
        Beamobject.transform.position = FireTransform.position + direction.normalized * (distance / 2);
        Beamobject.SetActive(true);
    }

    private void DestroyObject(FallingObject obj)
    {
        Beamobject.SetActive(false);
        Inrange.Remove(obj);

        if (obj.GetShapeEnum() == ShapeEnum.boomb)
            Destroy(obj);
        else
            obj.OnSwallow();

    }

}
