using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScript : MonoBehaviour
{
    [SerializeField] Slider staminaSlider;
    [SerializeField] Text staminaText;

    [SerializeField] Slider experienceSlider;
    [SerializeField] Text experienceText;

    [SerializeField] Text levelText;

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;

    private void Start()
    {
        bgm.Play();
        // ���[�U�[����ǂݍ���ŕ\��
        StartCoroutine(LoadUserData());
        CallStaminaRecovery();
        // �� �z�[���ɖ߂邽�тɏ̍��`�F�b�N�����s
        StartCoroutine(CheckAndRegisterTitles());
        // �X�^�~�i�̒���񕜃`�F�b�N
        InvokeRepeating(nameof(CallStaminaRecovery), 300f, 300f);
    }

    /// <summary>
    /// ���[�U�[���擾
    /// </summary>
    private IEnumerator LoadUserData()
    {
        yield return NetworkManager.Instance.GetUser((user) =>
        {
            if (user != null)
            {
                if (levelText != null)
                    levelText.text = $"{user.Level}";

                int needExp = user.Level * 10;
                if (experienceSlider != null)
                {
                    experienceSlider.maxValue = needExp;
                    experienceSlider.value = user.Experience;
                }
                if (experienceText != null)
                    experienceText.text = $"{user.Experience}/{needExp}";
            }
        });

        // ���[�U�[��{�f�[�^�ǂݍ��݌�ɕK����API�ŃX�^�~�i�ŐV��
        CallStaminaRecovery();
    }


    /// <summary>
    /// �X�^�~�i������API�Ăяo��
    /// </summary>
    private void CallStaminaRecovery()
    {
        StartCoroutine(NetworkManager.Instance.StaminaAutoRecovery((staminaData) =>
        {
            if (staminaData != null)
            {
                Debug.Log(staminaData);
                UpdateStaminaUI(staminaData.CurrentStamina, staminaData.MaxStamina);
            }
        }));
    }

    /// <summary>
    /// �X�^�~�iUI�X�V
    /// </summary>
    private void UpdateStaminaUI(int current, int max)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = max;

            if (current > max)
            {
                // MAX�Œ�
                staminaSlider.value = max;

                // ���[�v���������ꍇ�͂�����
                // staminaSlider.value = current % max;
            }
            else
            {
                staminaSlider.value = current;
            }
        }

        if (staminaText != null)
        {
            staminaText.text = $"{current}/{max}";
        }
    }

    IEnumerator CheckAndRegisterTitles()
    {
        // �X�e�[�W�i�����擾
        yield return NetworkManager.Instance.ShowStages((stages) =>
        {
            if (stages == null) return;

            StartCoroutine(NetworkManager.Instance.GetTitles((titles) =>
            {
                if (titles == null) return;

                HashSet<int> ownedTitleIds = new HashSet<int>();
                foreach (var t in titles) ownedTitleIds.Add(t.Id);

                // Chapter���Ƃɂ܂Ƃ߂�
                var chapters = new Dictionary<int, List<StageData>>();
                foreach (var s in stages)
                {
                    if (!chapters.ContainsKey(s.ChapterNum))
                        chapters[s.ChapterNum] = new List<StageData>();
                    chapters[s.ChapterNum].Add(s);
                }

                foreach (var kv in chapters)
                {
                    int chapter = kv.Key;
                    var stageList = kv.Value;

                    bool allClear = stageList.TrueForAll(s => s.Clear);
                    bool allStar3 = stageList.TrueForAll(s => s.Evaluation == 3);
                    bool allCollectibles = stageList.TrueForAll(s => s.Collectibles == true);

                    if (allClear)
                    {
                        int titleId = 1 + (chapter - 1) * 3;
                        if (!ownedTitleIds.Contains(titleId))
                            StartCoroutine(NetworkManager.Instance.StoreTitle(titleId, null));
                    }
                    if (allStar3)
                    {
                        int titleId = 2 + (chapter - 1) * 3;
                        if (!ownedTitleIds.Contains(titleId))
                            StartCoroutine(NetworkManager.Instance.StoreTitle(titleId, null));
                    }
                    if (allStar3 && allCollectibles)
                    {
                        int titleId = 3 + (chapter - 1) * 3;
                        if (!ownedTitleIds.Contains(titleId))
                            StartCoroutine(NetworkManager.Instance.StoreTitle(titleId, null));
                    }
                }
            }));
        });
    }
    public void GoToSetiing()
    {
        SetiingManagerScript.SceneReturnTarget = SceneManager.GetActiveScene().name;
        Initiate.Fade("SettingScene", Color.black, 0.5f);
    }
    public void GoToStageSelection()
    {
        Initiate.Fade("StageSelectionScene", Color.black, 0.5f);
    }
    public void GoToProfile()
    {
        Initiate.Fade("ProfileScene", Color.black, 0.5f);
    }
}
