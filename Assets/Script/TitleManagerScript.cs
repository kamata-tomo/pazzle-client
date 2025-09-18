using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleManagerScript : MonoBehaviour
{
    [SerializeField] InputField nameInputField;
    [SerializeField] GameObject RegistUserForm;
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;
    [SerializeField] AudioMixer audioMixer;

    private const string MASTER_KEY = "Master";
    private const string BGM_KEY = "BGMVolume";
    private const string SE_KEY = "SEVolume";

    void Start()
    {
        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat(MASTER_KEY, 0f));
        audioMixer.SetFloat("BGM", PlayerPrefs.GetFloat(BGM_KEY, 0f));
        audioMixer.SetFloat("SE", PlayerPrefs.GetFloat(SE_KEY, 0f));
        RegistUserForm.SetActive(false);
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void login()
    {
        se.PlayOneShot(ButtonSoundEffect);

        // セーブデータがあるかチェック
        if (NetworkManager.Instance.LoadUserData())
        {
            // ユーザー存在確認APIを呼ぶ
            StartCoroutine(NetworkManager.Instance.CheckUserExists(exists =>
            {
                if (exists)
                {
                    // ユーザーが存在するので次のシーンへ
                    Initiate.Fade("HomeScenes", Color.black, 0.5f);
                }
                else
                {
                    // ユーザーが存在しないので新規作成画面へ
                    RegistUserForm.SetActive(true);
                }
            }));
        }
        else
        {
            // セーブデータがないので新規作成画面へ
            RegistUserForm.SetActive(true);
        }
    }

    public void NewRegistration()
    {
        se.PlayOneShot(ButtonSoundEffect);
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("名前が入力されていません");
            return;
        }

        // NetworkManagerの登録処理を呼ぶ
        StartCoroutine(NetworkManager.Instance.RegistUser(playerName, (bool success) =>
        {
            if (success)
            {
                Debug.Log("ユーザー登録成功！");
                // 登録後はメインシーンへ遷移
                Initiate.Fade("HomeScenes", Color.black, 0.5f);
            }
            else
            {
                Debug.LogError("ユーザー登録失敗");
                // 失敗時のエラーメッセージをUI表示しても良い
            }
        }));
    }

}
