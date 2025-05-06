using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float originalAmplitude;
    private float originalFrequency;

    [Header("Shake Settings")]
    private float shakeDuration = 1.0f;
    private float shakeAmplitude = 10.0f;
    private float shakeFrequency = 10.0f;
    
    private Tween resetTween;
    private void Awake()
    {
        
        noise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // 기존 값 저장
        originalAmplitude = noise.AmplitudeGain;
        originalFrequency = noise.FrequencyGain;
    }

    public void HitShakeCamera()
    {

        PostEffectController.Instance.ActiveHitEffect(true);
        // 카메라 흔들기 시작
        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;

        // DOTween을 사용하여 흔들림을 점점 줄이면서 원래 상태로 되돌림
        DOVirtual.Float(shakeAmplitude, 0, shakeDuration, (value) => noise.AmplitudeGain = value)
                 .SetEase(Ease.OutCubic).OnComplete(() => PostEffectController.Instance.ActiveHitEffect(false));
    }
    

    
    public void ResetCameraShake()
    {
        // 흔들림을 원래 값으로 복원
        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
    }
}
