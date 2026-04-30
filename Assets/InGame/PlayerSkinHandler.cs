using UnityEngine;
using System.Collections;

public class PlayerSkinHandler : MonoBehaviour
{
    [Header("필수 컴포넌트 (Sprite_Visual 연결)")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public AudioSource inGameAudioSource;

    [Header("스킨 리스트")]
    public Sprite[] skinSprites;
    public RuntimeAnimatorController[] skinAnimators;
    public AudioClip[] skinBGMs;

    // ⭐ 기준이 될 기본 크기를 저장
    private Vector2 baseWorldSize;
    private bool isSizeInitialized = false;

    void Start()
    {
        // 1. 게임 시작 시, 현재(기본스킨)의 월드 실제 크기를 딱 한 번만 측정합니다.
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            baseWorldSize = spriteRenderer.sprite.bounds.size;
            isSizeInitialized = true;
            Debug.Log($"<color=cyan>[기준 설정]</color> 기본 스킨 월드 크기: {baseWorldSize.x} x {baseWorldSize.y}");
        }

        StartCoroutine(ApplySkinRoutine());
    }

    IEnumerator ApplySkinRoutine()
    {
        // 유니티 시스템 초기화를 위해 한 프레임 대기
        yield return new WaitForEndOfFrame();

        int index = PlayerPrefs.GetInt("EquippedSkin", 0);
        Debug.Log($"<color=yellow>[스킨 적용]</color> 현재 착용 번호: {index}");

        // 2. 음악 교체
        if (inGameAudioSource != null && skinBGMs.Length > index && skinBGMs[index] != null)
        {
            inGameAudioSource.clip = skinBGMs[index];
            inGameAudioSource.Play();
        }

        // 3. 외형 교체 (애니메이터 일시 정지)
        if (animator != null) animator.enabled = false;

        if (spriteRenderer != null && skinSprites.Length > index)
        {
            spriteRenderer.sprite = skinSprites[index];

            // --- ⭐ [핵심] 모든 스킨을 기본 크기에 맞추는 계산식 ---
            if (isSizeInitialized)
            {
                Vector2 newSpriteSize = spriteRenderer.sprite.bounds.size;

                // 기본 크기 ÷ 새 스킨 크기 = 필요한 배율(Scale)
                float scaleX = baseWorldSize.x / newSpriteSize.x;
                float scaleY = baseWorldSize.y / newSpriteSize.y;

                // 그림판(Sprite_Visual)의 크기를 조절하여 겉보기를 일치시킴
                spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                Debug.Log($"[크기 보정] {index}번 스킨 배율 적용: {scaleX}");
            }
        }

        yield return new WaitForEndOfFrame();

        // 4. 애니메이션 활성화
        if (animator != null && skinAnimators.Length > index && skinAnimators[index] != null)
        {
            animator.runtimeAnimatorController = skinAnimators[index];
            animator.enabled = true;
        }
    }
}