using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // 소환되자마자 실행되는 부분
void Start()
    {
        int index = PlayerPrefs.GetInt("EquippedSkin", 0);

        // 상점 매니저와 같은 SkinData 리스트를 참조하거나, 
        // 혹은 인게임용 BGM 리스트를 따로 만들어 그 index를 사용하세요.
        // audioSource.clip = inGameBGMList[index];
        // audioSource.Play();

    Destroy(gameObject, 1.0f);
    }
}