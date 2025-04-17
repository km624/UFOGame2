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
    // 1초마다 체크
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
                //플레이어 보다 레벨 높으면 return
                if (player.CurrentLevel < FObject.ObjectMass) return;

                BossObject bossObject = other.GetComponent<BossObject>();
                //보스 오브젝트면 빔 활성화 X
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

                // 리스트 순회
                foreach (FallingObject obj in Inrange)
                {
                    if (obj.gameObject.layer == 9)
                    {
                        // 오브젝트가 이미 파괴되었으면 건너뜁니다.
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

                // 가장 가까운 오브젝트가 있다면 파괴하고 리스트에서 제거
                if (closestObj != null)
                {
                    // 총이 있다면, 파괴 대상 오브젝트 방향으로 회전하도록 설정
                    if (GunTransform != null)
                    {
                        Vector3 direction = closestObj.transform.position - GunTransform.position;
                        if (direction != Vector3.zero)
                        {
                          
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                          
                            // DOTween을 사용해 부드럽게 회전 (0.2초)
                            //GunTransform.DORotateQuaternion(targetRotation, RotateDuration).OnComplete(() => DestroyObject(closestObj));
                            Sequence sequence = DOTween.Sequence();

                            sequence.Append(GunTransform.DORotateQuaternion(targetRotation, RotateDuration))
                                    .AppendCallback(() => BeamActive(closestObj.transform.position))  // 빔 활성화
                                    .AppendInterval(BeamActiveTime)         // 0.5초 동안 빔 유지
                                    .OnComplete(() => DestroyObject(closestObj));  // 빔 비활성화 및 타겟 파괴
                        }
                        else
                        {
                            DestroyObject(closestObj); // 바로 파괴
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
