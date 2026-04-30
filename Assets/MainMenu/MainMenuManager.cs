using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("설정")]
    public string inGameSceneName = "InGame"; // 인게임 씬 이름
    public GameObject skinShopPanel;         // 스킨 상점 패널 (하이어라키에서 드래그)

    void Start()
    {
        // 시작할 때 상점은 꺼두기
        if (skinShopPanel != null) skinShopPanel.SetActive(false);
    }


    // [Game Start] 버튼용
    public void GameStart()
    {
        Debug.Log("게임을 시작합니다!");
        SceneManager.LoadScene(inGameSceneName);
    }

    // [Skin Shop] 버튼용
    public void OpenShop()
    {
        Debug.Log("상점을 엽니다.");
        if (skinShopPanel != null) skinShopPanel.SetActive(true);
    }


    // [Exit] 버튼용
    public void GameExit()
    {
        Debug.Log("게임을 종료합니다.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}