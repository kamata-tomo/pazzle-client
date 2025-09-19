using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Text chapterLabel;
    public GameObject stageButtonPrefab;
    public Transform gridParent;
    public GameObject lackOfStaminatext;

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;

    private int currentChapter = 1;
    private int maxChapters = 1;

    private List<StageData> allStages = new List<StageData>();

    void Start()
    {
        bgm.Play();
        lackOfStaminatext.SetActive(false);
        // API����X�e�[�W�����擾
        StartCoroutine(NetworkManager.Instance.ShowStages(OnStagesLoaded));
        
    }

    private void OnStagesLoaded(List<StageData> stages)
    {
        if (stages == null || stages.Count == 0)
        {
            Debug.LogError("�X�e�[�W�f�[�^���擾�ł��܂���ł���");
            return;
        }

        allStages = stages;
        maxChapters = stages.Max(s => s.ChapterNum);

        UpdateStageGrid();
    }

    public void ChangeChapter(int delta)
    {

        se.PlayOneShot(ButtonSoundEffect);
        currentChapter = Mathf.Clamp(currentChapter + delta, 1, maxChapters);
        UpdateStageGrid();
    }

    private void UpdateStageGrid()
    {
        chapterLabel.text = $"�`���v�^�[ {currentChapter}";

        // �Â��{�^�����폜
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        // ���݂̃`���v�^�[�̃X�e�[�W�f�[�^�������o
        List<StageData> chapterStages = allStages
            .Where(s => s.ChapterNum == currentChapter)
            .OrderBy(s => s.StageNum)
            .ToList();

        // �A�����b�N����p
        HashSet<int> unlockedStages = new HashSet<int>();

        if (chapterStages.Count > 0)
        {
            // �`���v�^�[���̍ŏ��̃X�e�[�W�̓f�t�H���g�ŃA�����b�N
            unlockedStages.Add(chapterStages.First().StageId);

            for (int i = 0; i < chapterStages.Count; i++)
            {
                StageData stage = chapterStages[i];
                if (stage.Clear)
                {
                    unlockedStages.Add(stage.StageId);
                    // �N���A�ς݂Ȃ玟�̃X�e�[�W���A�����b�N
                    if (i + 1 < chapterStages.Count)
                        unlockedStages.Add(chapterStages[i + 1].StageId);
                }
            }
        }

        // �{�^������
        foreach (StageData stage in chapterStages)
        {
            GameObject buttonObj = Instantiate(stageButtonPrefab, gridParent);
            StageButton btn = buttonObj.GetComponent<StageButton>();

            bool unlocked = unlockedStages.Contains(stage.StageId);
            int starCount = stage.Evaluation.HasValue ? stage.Evaluation.Value : 0;
            bool collectible = stage.Collectibles.HasValue && stage.Collectibles.Value;

            btn.Setup(stage.StageNum, unlocked, starCount, collectible,stage.ChapterNum, stage.StageId, stage.ShuffleCount, stage.reference_value_1,stage.reference_value_2,stage.reference_value_3);
        }
    }

    public void GoToSetiing()
    {
        se.PlayOneShot(ButtonSoundEffect);
        SetiingManagerScript.SceneReturnTarget = SceneManager.GetActiveScene().name;
        Initiate.Fade("SettingScene", Color.black, 0.5f);
    }


    public void ReturnHome()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

    public void LackOfStamina()
    {
        lackOfStaminatext.SetActive(true);
    }

    public void closeLackOfStamina()
    {
        lackOfStaminatext.SetActive(false);
    }
}
