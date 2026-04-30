using UnityEngine;
using UnityEngine.SceneManagement; // 씬을 이동할 때 꼭 필요한 라이브러리입니다.

public class MainMenu : MonoBehaviour
{
    // 유니티 인스펙터 창에서 인게임 씬의 이름을 정확히 적어줄 칸입니다.
    public string inGameSceneName = "InGame";

    // [Game Start] 버튼에 연결할 함수
    public void GameStart()
    {
        Debug.Log("게임을 시작합니다!");
        SceneManager.LoadScene(inGameSceneName); // 설정한 이름의 씬을 불러옵니다.
    }

    // [Skin Shop] 버튼에 연결할 함수 (나중에 구현)
    public void OpenShop()
    {
        Debug.Log("스킨 상점은 아직 준비 중입니다!");
    }

    // [End] 버튼에 연결할 함수
    public void GameExit()
    {
        Debug.Log("게임을 종료합니다.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 테스트할 때 꺼짐
#else
            Application.Quit(); // 실제 게임 빌드에서 꺼짐
#endif
    }
}