using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public Text stageNumberText;
    public GameObject lockIcon;
    public GameObject[] stars;

    private int stageNumber;
    private bool isUnlocked;

    public void Setup(int number, bool unlocked, int starCount)
    {
        stageNumber = number;
        isUnlocked = unlocked;

        stageNumberText.text = number.ToString();
        lockIcon.SetActive(!unlocked);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < starCount);
        }
    }

    public void OnClick()
    {
        if (!isUnlocked) return;

        Debug.Log($"ステージ {stageNumber} 開始");
        // ステージ開始処理（シーン遷移など）
        Initiate.Fade("SlidePuzzleScene", Color.black, 0.5f);
    }
}
