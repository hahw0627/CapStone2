using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeProgressSlider : MonoBehaviour
{
    [Header("Gameplay Settings")]
    [SerializeField] private float stageDuration = 120f; // 2분(120초)
    [SerializeField] private float timeRemaining;

    [Header("UI References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text timerText;
    [SerializeField] private Image fillImage; // 슬라이더의 Fill 이미지
    [SerializeField] private Image handleImage; // 슬라이더의 Handle 이미지 (선택적)

    [Header("Visual Effects")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.8f, 0.2f); // 기본 초록색
    [SerializeField] private Color warningColor = new Color(0.9f, 0.9f, 0.2f); // 경고 노란색
    [SerializeField] private Color dangerColor = new Color(0.9f, 0.2f, 0.2f); // 위험 빨간색
    [SerializeField] private float warningThreshold = 0.5f; // 경고 색상으로 변경할 시간 비율
    [SerializeField] private float dangerThreshold = 0.25f; // 위험 색상으로 변경할 시간 비율
    [SerializeField] private bool usePulseEffect = true; // 시간이 얼마 안 남았을 때 펄스 효과 사용

    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 3f; // 펄스 효과 속도
    [SerializeField] private float pulseMinAlpha = 0.7f; // 펄스 효과의 최소 알파값

   /* [Header("Sound")]
    [SerializeField] private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioClip warningSound; // 경고 사운드
    [SerializeField] private AudioClip tickSound; // 초 단위 틱 사운드
    [SerializeField] private bool playTickEverySecond = false; // 매 초마다 틱 사운드 재생 */

    private bool isGameOver = false;
    private float lastSecond;
    private bool isWarningPlayed = false;
    private bool isDangerZone = false;

    private void Start()
    {
        if (progressSlider == null)
        {
            Debug.LogError("Progress Slider is not assigned!");
            return;
        }

        // 타이머 초기화
        timeRemaining = stageDuration;
        lastSecond = Mathf.Floor(timeRemaining);

        // 슬라이더 설정
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        progressSlider.value = 0f;

        // Fill 이미지가 없으면 슬라이더에서 찾기
        if (fillImage == null)
        {
            fillImage = progressSlider.fillRect.GetComponent<Image>();
        }

        // 초기 색상 설정
        if (fillImage != null)
        {
            fillImage.color = normalColor;
        }

        // 타이머 시작
        StartCoroutine(CountdownTimer());
    }

    private void Update()
    {
        if (isGameOver) return;

        // 진행 상황 업데이트
        UpdateProgressSlider();

        // 틱 사운드 재생 확인
        // CheckTickSound();

        // 펄스 효과 업데이트
        if (usePulseEffect && isDangerZone)
        {
            UpdatePulseEffect();
        }
    }

    private IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(0.05f);
            timeRemaining -= 0.05f;

            // 시간이 다 되면 게임 오버
            if (timeRemaining <= 0)
            {
                GameOver(false);
            }

            // 남은 시간 텍스트 업데이트
            UpdateTimerText();

            // 색상 상태 확인 및 변경
            CheckColorState();
        }
    }

    private void UpdateProgressSlider()
    {
        // 시간 기반 진행도 계산
        progressSlider.value = 1f - (timeRemaining / stageDuration);
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void CheckColorState()
    {
        if (fillImage == null) return;

        float timeRatio = timeRemaining / stageDuration;

        // 위험 구간 확인
        if (timeRatio <= dangerThreshold && !isDangerZone)
        {
            isDangerZone = true;
            fillImage.color = dangerColor;

            // 위험 사운드 재생
           /* if(audioSource != null && warningSound != null && !isWarningPlayed)
            {
                audioSource.PlayOneShot(warningSound);
                isWarningPlayed = true;
            }*/
        }
        // 경고 구간 확인
        else if (timeRatio <= warningThreshold && timeRatio > dangerThreshold)
        {
            fillImage.color = warningColor;
        }
        else if (timeRatio > warningThreshold)
        {
            isDangerZone = false;
            isWarningPlayed = false;
            fillImage.color = normalColor;
        }
    }

    private void UpdatePulseEffect()
    {
        if (fillImage == null) return;

        // 펄스 효과 (알파값 변경)
        Color currentColor = fillImage.color;
        float alpha = pulseMinAlpha + Mathf.PingPong(Time.time * pulseSpeed, 1f - pulseMinAlpha);
        fillImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

        // Handle 이미지가 있다면 동일한 효과 적용
        if (handleImage != null)
        {
            Color handleColor = handleImage.color;
            handleImage.color = new Color(handleColor.r, handleColor.g, handleColor.b, alpha);
        }
    }

    /*private void CheckTickSound()
    {
        if (!playTickEverySecond || audioSource == null || tickSound == null) return;

        float currentSecond = Mathf.Floor(timeRemaining);
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            audioSource.PlayOneShot(tickSound, 0.5f);
        }
    }*/

    public void GameOver(bool isSuccess)
    {
        isGameOver = true;

        if (isSuccess)
        {
            Debug.Log("Stage Clear! Time remaining: " + timeRemaining.ToString("F1"));
            // 스테이지 클리어 처리
        }
        else
        {
            Debug.Log("Time Over! Stage Failed.");
            // 게임 오버 처리
        }

        // 타이머 정지
        StopAllCoroutines();
    }
}
