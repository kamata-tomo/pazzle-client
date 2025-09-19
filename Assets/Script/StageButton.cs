using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] Text stageNumberText;
    [SerializeField] GameObject lockIcon;
    [SerializeField] GameObject[] stars;
    [SerializeField] GameObject collectibleIcon;

    [SerializeField] AudioClip ButtonSoundEffect;

    private int stageNumber;
    private int stageId;
    private bool isUnlocked;
    private int chapterNum;
    private int shuffleCount;
    private int referencevalue1;
    private int referencevalue2;
    private int referencevalue3;

    public void Setup(int number, bool unlocked, int starCount, bool collectible,int chapter_num, int stageId, int shufflecount,int reference_value_1,int reference_value_2, int reference_value_3)
    {
        this.stageNumber = number;
        this.isUnlocked = unlocked;
        this.chapterNum = chapter_num;
        this.stageId = stageId;
        this.shuffleCount = shufflecount;
        this.referencevalue1 = reference_value_1;
        this.referencevalue2 = reference_value_2;
        this.referencevalue3 = reference_value_3;

        stageNumberText.text = number.ToString();
        lockIcon.SetActive(!unlocked);
        collectibleIcon.SetActive(collectible);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < starCount);
        }
    }

    public void OnClick()
    {
        GameObject.Find("SE").GetComponent<AudioSource>().PlayOneShot(ButtonSoundEffect);
        if (!isUnlocked) return;
        NetworkManager.Instance.StaminaAutoRecovery((stamina) => {
            if (stamina != null)
            {
                Debug.Log($"�X�^�~�i: {stamina.CurrentStamina}/{stamina.MaxStamina}");
            }
            else
            {
                Debug.LogError("�X�^�~�i�����񕜂Ɏ��s");
            }
        });
        StartCoroutine(NetworkManager.Instance.StaminaChangesByReason(1, chapterNum,
    (stamina) =>
    {
        // ���펞
        Debug.Log($"�X�^�~�i�X�V: {stamina.CurrentStamina}/{stamina.MaxStamina}");
        Debug.Log($"�X�e�[�W {stageNumber} (ID:{stageId}) �J�n");

        // �I�����ꂽ�X�e�[�WID��ۑ�
        StageSelectData.SelectedStageId = stageId;
        StageSelectData.shuffle_count = shuffleCount;
        StageSelectData.ChapterNum = chapterNum;
        StageSelectData.reference_value_1 = referencevalue1;
        StageSelectData.reference_value_2 = referencevalue2;
        StageSelectData.reference_value_3 = referencevalue3;


        // �X�e�[�W�v���C�V�[���֑J��
        Initiate.Fade("SlidePuzzleScene", Color.black, 0.5f);
    },
    (errorMsg) =>
    {
        // �G���[��
        Debug.LogError($"�G���[: {errorMsg}");
        StageSelectManager stageSelectManager = FindObjectOfType<StageSelectManager>();
        stageSelectManager.LackOfStamina(); 
    }));


    }
}
