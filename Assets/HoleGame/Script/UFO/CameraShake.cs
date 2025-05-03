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
    private float shakeAmplitude = 7.5f;
    private float shakeFrequency = 7.5f;
    
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
        resetTween?.Kill();

        PostEffectController.Instance.ActiveHitEffect(true);
        // ī�޶� ���� ����
        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;

        // DOTween�� ����Ͽ� ��鸲�� ���� ���̸鼭 ���� ���·� �ǵ���
        DOVirtual.Float(shakeAmplitude, 0, shakeDuration, (value) => noise.AmplitudeGain = value)
                 .SetEase(Ease.OutCubic).OnComplete(() => PostEffectController.Instance.ActiveHitEffect(false));
    }
    public void ShakeCamera(float duration)
    {
        resetTween?.Kill();

        // ���� ������ ����
        noise.AmplitudeGain = 0.5f;
        noise.FrequencyGain = 1.0f;

        // duration�� ������ ���� ����
        resetTween = DOVirtual.DelayedCall(duration, ResetCameraShake);
    }

    public void ShakeCameraOneShot(float amplitudegain)
    {
        noise.AmplitudeGain = amplitudegain;
        DOVirtual.Float(shakeAmplitude, 0, 0.5f, (value) => noise.AmplitudeGain = value);
                
    }

    
    public void ResetCameraShake()
    {
        // ��鸲�� ���� ������ ����
        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
    }
}
