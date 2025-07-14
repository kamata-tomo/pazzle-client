using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Text chapterLabel;
    public GameObject stageButtonPrefab;
    public Transform gridParent;
    public Button prevButton;
    public Button nextButton;

    private int currentChapter = 1;
    private const int stagesPerChapter = 9;
    private const int maxChapters = 10; // 最大チャプター数

    void Start()
    {
        prevButton.onClick.AddListener(() => ChangeChapter(-1));
        nextButton.onClick.AddListener(() => ChangeChapter(1));

        UpdateStageGrid();
    }

    void ChangeChapter(int delta)
    {
        currentChapter = Mathf.Clamp(currentChapter + delta, 1, maxChapters);
        UpdateStageGrid();
    }

    void UpdateStageGrid()
    {
        chapterLabel.text = $"チャプター {currentChapter}";

        // 古いボタンを削除
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        int startStageNumber = (currentChapter - 1) * stagesPerChapter + 1;

        for (int i = 0; i < stagesPerChapter; i++)
        {
            int stageNumber = startStageNumber + i;
            GameObject buttonObj = Instantiate(stageButtonPrefab, gridParent);
            StageButton btn = buttonObj.GetComponent<StageButton>();

            bool unlocked = PlayerPrefs.GetInt($"Stage{stageNumber}_Unlocked", 0) == 1 || stageNumber == 1;
            int starCount = PlayerPrefs.GetInt($"Stage{stageNumber}_Stars", 0);
            btn.Setup(stageNumber, unlocked, starCount);
        }

        prevButton.interactable = currentChapter > 1;
        nextButton.interactable = currentChapter < maxChapters;
    }

    public void GoToSetiing()
    {
        SetiingManagerScript.SceneReturnTarget = SceneManager.GetActiveScene().name;
        Initiate.Fade("SettingScene", Color.black, 0.5f);
    }

    public void ReturnHome()
    {
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }
}

