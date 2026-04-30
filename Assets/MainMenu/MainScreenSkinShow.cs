using UnityEngine;

public class MainScreenSkinShow : MonoBehaviour
{
    [Header("스킨 설정")]
    public int skinIndex; // 0: 기본, 1: 수녀, 2: 냥캣 (상점과 인덱스 통일)

    void OnEnable()
    {
        // 0번(기본)은 항상 보여주고, 나머지는 구매 여부를 확인합니다.
        if (skinIndex == 0)
        {
            gameObject.SetActive(true);
            return;
        }

        // 사물함(PlayerPrefs)에서 이 스킨의 잠금해제 여부를 가져옵니다.
        // 상점에서 썼던 키 이름("SkinUnlocked_" + index)과 똑같이 써야 합니다!
        int isUnlocked = PlayerPrefs.GetInt("SkinUnlocked_" + skinIndex, 0);

        if (isUnlocked == 1)
        {
            // 구매했으면 보임
            gameObject.SetActive(true);
        }
        else
        {
            // 구매 안 했으면 안 보임
            gameObject.SetActive(false);
        }
    }
}