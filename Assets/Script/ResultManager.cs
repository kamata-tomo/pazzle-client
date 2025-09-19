using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageResultData
{
    public static int StageId { get; set; }
    public static int ChapterNum { get; set; }
    public static int Evaluation { get; set; } // 1,2,3 の星評価
    public static bool Collectible { get; set; }
}

public class ResultManager : MonoBehaviour
{
    [SerializeField] GameObject CLEARtext;
    [SerializeField] GameObject FAILUREtext;
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;

    void Start()
    {
        bgm.Play();
        // クリア情報をサーバーに送信（評価が0なら送らない）
        if (StageResultData.Evaluation > 0)
        {
            CLEARtext.SetActive(true);
            FAILUREtext.SetActive(false);
            StartCoroutine(
                NetworkManager.Instance.ClearStage(
                    StageResultData.StageId,
                    StageResultData.Evaluation,
                    StageResultData.Collectible,
                    (isSuccess) =>
                    {
                        Debug.Log("ClearStage API 呼び出し結果: " + isSuccess);
                    }
                )
            );
            StartCoroutine(NetworkManager.Instance.UpdateUser(null, StageResultData.ChapterNum * 5,null, (isSuccess) =>
            {
                Debug.Log($"{StageResultData.ChapterNum * 5}経験値");
            }));
        }
        else
        {
            CLEARtext.SetActive(false);
            FAILUREtext.SetActive(true);
        }
    }

    public void GoTohome()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

    public void GoToStageSelection()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("StageSelectionScene", Color.black, 0.5f);
    }

    public void ReturnGame()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("SlidePuzzleScene", Color.black, 0.5f);
    }
}
