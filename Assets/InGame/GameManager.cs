using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // [새로 추가] 코루틴 사용을 위해 필요합니다!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("★ 인게임 UI ★")]
    public TextMeshProUGUI inGameDistanceText;
    public TextMeshProUGUI inGameTimeText;
    public TextMeshProUGUI inGameGoldText;
    public TextMeshProUGUI startCountdownText; // [새로 추가] 3, 2, 1 카운트다운용 텍스트

    [Header("결과창 UI 구성 요소")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI bestDistanceText;
    public TextMeshProUGUI currentDistanceText;
    public TextMeshProUGUI earnedGoldText;

    [Header("게임 설정")]
    public Transform player;
    public float goldRatio = 0.1f;
    public string mainMenuSceneName = "MainMenu";

    private float startX;
    private bool isGameOver = false;
    private bool isGameStarted = false; // [새로 추가] 게임 시작 여부 확인

    private float playTime = 0f;
    private int currentEarnedGold = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isGameStarted = false; // [새로 추가] 시작은 항상 false
        playTime = 0f;
        currentEarnedGold = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (player != null) startX = player.position.x;

        // [새로 추가] 카운트다운 코루틴 실행
        StartCoroutine(StartCountdownRoutine());
    }

    // [새로 추가] 카운트다운 로직
    IEnumerator StartCountdownRoutine()
    {
        if (startCountdownText != null)
        {
            startCountdownText.gameObject.SetActive(true);

            float timer = 3f;
            while (timer > 0)
            {
                startCountdownText.text = Mathf.CeilToInt(timer).ToString();
                yield return new WaitForSeconds(1f);
                timer -= 1f;
            }

            startCountdownText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            startCountdownText.gameObject.SetActive(false);
        }

        isGameStarted = true; // 드디어 게임 시작!
    }

    // [새로 추가] 외부에서 게임 시작 여부를 확인할 수 있는 함수
    public bool IsGameStarted()
    {
        return isGameStarted;
    }

    void Update()
    {
        // [수정] 게임이 시작되지 않았거나 종료되었다면 로직을 실행하지 않음
        if (!isGameStarted || isGameOver || player == null) return;

        // 1. 거리 계산
        float currentDist = Mathf.Max(0, player.position.x - startX);
        if (inGameDistanceText != null)
        {
            inGameDistanceText.text = $"{Mathf.FloorToInt(currentDist)}m";
        }

        // 2. 시간 계산
        playTime += Time.deltaTime;
        UpdateTimeUI();

        // 3. 실시간 골드 업데이트
        currentEarnedGold = Mathf.FloorToInt(currentDist * goldRatio);
        if (inGameGoldText != null)
        {
            inGameGoldText.text = $"GOLD: {currentEarnedGold}";
        }

        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    void UpdateTimeUI()
    {
        if (inGameTimeText == null) return;
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        inGameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnPlayerDie()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        float distanceValue = (player != null) ? Mathf.Max(0, player.position.x - startX) : 0;
        int finalDistance = Mathf.FloorToInt(distanceValue);
        int earnedGold = Mathf.FloorToInt(finalDistance * goldRatio);

        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        totalGold += earnedGold;
        PlayerPrefs.SetInt("TotalGold", totalGold);

        int bestDistance = PlayerPrefs.GetInt("BestDistance", 0);
        if (finalDistance > bestDistance)
        {
            bestDistance = finalDistance;
            PlayerPrefs.SetInt("BestDistance", bestDistance);
        }
        PlayerPrefs.Save();

        if (bestDistanceText != null) bestDistanceText.text = $"베스트거리: {bestDistance}m";
        if (currentDistanceText != null) currentDistanceText.text = $"이동거리: {finalDistance}m";
        if (earnedGoldText != null) earnedGoldText.text = $"획득 골드: +{earnedGold}G";

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        GameObject realCanvas = GameObject.Find("Canvas");
        if (realCanvas != null)
        {
            Transform realPanel = realCanvas.transform.Find("GameOver_Panel");
            if (realPanel != null) realPanel.gameObject.SetActive(true);
        }
    }

    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void GoToMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene(mainMenuSceneName); }
}