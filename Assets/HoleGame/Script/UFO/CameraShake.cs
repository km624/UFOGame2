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

        // ���� �� ����
        originalAmplitude = noise.AmplitudeGain;
        originalFrequency = noise.FrequencyGain;
    }

    public void HitShakeCamera()
    {

        PostEffectController.Instance.ActiveHitEffect(true);
        // ī�޶� ���� ����
        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;

        // DOTween�� ����Ͽ� ��鸲�� ���� ���̸鼭 ���� ���·� �ǵ���
        DOVirtual.Float(shakeAmplitude, 0, shakeDuration, (value) => noise.AmplitudeGain = value)
                 .SetEase(Ease.OutCubic).OnComplete(() => PostEffectController.Instance.ActiveHitEffect(false));
    }
    

    
    public void ResetCameraShake()
    {
        // ��鸲�� ���� ������ ����
        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
    }
}
