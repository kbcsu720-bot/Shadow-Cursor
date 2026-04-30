using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SkinData
{
    public string skinName;
    public int price;
    public bool isUnlocked;
    public TextMeshProUGUI buttonText;
    public AudioClip skinBGM;
}

public class SkinShopManager : MonoBehaviour
{
    [Header("상단 정보 UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI skinCountText;

    [Header("상점 UI 및 캐릭터 연동")]
    public GameObject shopPanel;
    public GameObject[] mainCharacters; // 상점 열 때 사라질 캐릭터들
    public AudioSource bgmAudioSource;

    [Header("스킨 데이터")]
    public List<SkinData> skinList;

    private int currentGold;
    private int currentEquippedIndex;
    private int totalSkinCount = 5;

    void Start()
    {
        LoadSkinPurchaseData();
        currentEquippedIndex = PlayerPrefs.GetInt("EquippedSkin", 0);

        RefreshUI();
        PlayEquippedSkinBGM();

        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void Update()
    {
        // ESC를 누르면 상점이 닫힘
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopPanel != null && shopPanel.activeSelf)
            {
                CloseShop();
            }
        }
    }

    // ⭐ [추가] 스크립트나 오브젝트가 꺼질 때 캐릭터를 강제로 다시 켭니다.
    // ESC로 닫든, 버튼으로 닫든 '비활성화'되는 순간 무조건 실행됩니다.
    void OnDisable()
    {
        ShowCharacters(true);
    }

    // --- ⭐ 상점 열기/닫기 로직 ---

    public void OpenShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
        ShowCharacters(false); // 캐릭터 숨기기
        RefreshUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        ShowCharacters(true); // 캐릭터 나타나기
        PlayEquippedSkinBGM();
    }

    // 캐릭터들을 끄고 켜는 공통 함수
    void ShowCharacters(bool isShow)
    {
        if (mainCharacters == null) return;

        foreach (GameObject character in mainCharacters)
        {
            if (character != null) character.SetActive(isShow);
        }
    }

    // --- ⭐ 음악 및 데이터 로직 (기존과 동일) ---

    void PlayEquippedSkinBGM()
    {
        if (bgmAudioSource == null) return;
        AudioClip selectedClip = skinList[currentEquippedIndex].skinBGM;

        if (selectedClip != null)
        {
            if (bgmAudioSource.clip != selectedClip)
            {
                bgmAudioSource.clip = selectedClip;
                bgmAudioSource.loop = true;
                bgmAudioSource.Play();
            }
        }
    }

    void LoadSkinPurchaseData()
    {
        skinList[0].isUnlocked = true;
        for (int i = 1; i < skinList.Count; i++)
        {
            int isUnlocked = PlayerPrefs.GetInt("SkinUnlocked_" + i, 0);
            skinList[i].isUnlocked = (isUnlocked == 1);
        }
    }

    public void RefreshUI()
    {
        currentGold = PlayerPrefs.GetInt("TotalGold", 1000);
        int ownedCount = 0;
        foreach (var skin in skinList) { if (skin.isUnlocked) ownedCount++; }

        goldText.text = $"GOLD: {currentGold}G";
        skinCountText.text = $"SKINS: {ownedCount}/{totalSkinCount}";

        for (int i = 0; i < skinList.Count; i++)
        {
            if (skinList[i].buttonText == null) continue;
            if (i == currentEquippedIndex) skinList[i].buttonText.text = "착용중";
            else if (skinList[i].isUnlocked) skinList[i].buttonText.text = "착용";
            else skinList[i].buttonText.text = "구매";
        }
    }

    public void OnClickSkinButton(int index)
    {
        SkinData selected = skinList[index];
        if (selected.isUnlocked)
        {
            if (currentEquippedIndex == index) return;
            EquipSkin(index);
        }
        else
        {
            if (currentGold >= selected.price)
            {
                currentGold -= selected.price;
                selected.isUnlocked = true;
                PlayerPrefs.SetInt("SkinUnlocked_" + index, 1);
                PlayerPrefs.SetInt("TotalGold", currentGold);
                PlayerPrefs.Save();
                EquipSkin(index);
            }
        }
        RefreshUI();
    }

    void EquipSkin(int index)
    {
        currentEquippedIndex = index;
        PlayerPrefs.SetInt("EquippedSkin", index);
        PlayerPrefs.Save();
        PlayEquippedSkinBGM();
    }
}