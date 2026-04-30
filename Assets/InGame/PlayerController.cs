using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float baseForwardSpeed = 5f;
    public float verticalSpeed = 8f;
    public float smoothness = 5f;

    [Header("★ 난이도 상승 설정 ★")]
    public int speedUpDistance = 100;
    public float speedIncreaseAmount = 1f;

    private float currentForwardSpeed;

    [Header("사망 경계 설정")]
    public float yBoundary = 10f;
    public float invincibleTime = 1.5f;

    [Header("★ 사운드 설정 ★")]
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private bool isMovingUp = false;
    private Camera mainCamera;
    private float startTime;
    private float startX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.1f;
        rb.freezeRotation = true;
        mainCamera = Camera.main;

        // [수정] startTime은 실제 게임이 시작될 때 갱신하도록 아래에서 처리합니다.
        startX = transform.position.x;
        currentForwardSpeed = baseForwardSpeed;
    }

    void Update()
    {
        // [추가] 게임이 시작되지 않았다면 아무것도 하지 않음 (입력 차단)
        if (GameManager.Instance != null && !GameManager.Instance.IsGameStarted())
        {
            // 게임이 시작되기 전까지 startTime을 계속 현재 시간으로 초기화해서 
            // 시작 직후에 무적 시간이 정확히 적용되게 합니다.
            startTime = Time.time;
            return;
        }

        // 스페이스바를 누를 때마다 방향 전환 (토글 방식)
        if (Input.GetKeyDown(KeyCode.Space)) isMovingUp = !isMovingUp;

        if (Time.time - startTime > invincibleTime)
        {
            CheckOutOfBounds();
        }

        UpdateDifficulty();
    }

    void FixedUpdate()
    {
        // [추가] 게임이 시작되지 않았다면 물리 이동도 중단
        if (GameManager.Instance != null && !GameManager.Instance.IsGameStarted())
        {
            rb.linearVelocity = Vector2.zero; // 멈춤
            return;
        }

        float targetYVelocity = isMovingUp ? verticalSpeed : -verticalSpeed;
        float newY = Mathf.Lerp(rb.linearVelocity.y, targetYVelocity, Time.fixedDeltaTime * smoothness);

        rb.linearVelocity = new Vector2(currentForwardSpeed, newY);

        if (transform.childCount > 0)
        {
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0f);
        }
    }

    void UpdateDifficulty()
    {
        float distance = Mathf.Max(0, transform.position.x - startX);
        int level = Mathf.FloorToInt(distance / speedUpDistance);
        currentForwardSpeed = baseForwardSpeed + (level * speedIncreaseAmount);
    }

    void CheckOutOfBounds()
    {
        if (Mathf.Abs(transform.position.y) > yBoundary) Die();

        if (mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);
            if (screenPos.x < -0.5f) Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - startTime < invincibleTime) return;
        if (collision.gameObject.CompareTag("Obstacle")) Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - startTime < invincibleTime) return;
        if (other.CompareTag("Obstacle")) Die();
    }

    void Die()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, mainCamera.transform.position);
        }

        if (GameManager.Instance != null) GameManager.Instance.OnPlayerDie();

        gameObject.SetActive(false);
    }
}