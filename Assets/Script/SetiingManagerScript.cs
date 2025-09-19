using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetiingManagerScript : MonoBehaviour
{
    public static string SceneReturnTarget;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [SerializeField] AudioSource Bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;

    private const string MASTER_KEY = "Master";
    private const string BGM_KEY = "BGMVolume";
    private const string SE_KEY = "SEVolume";

    private void Start()
    {
        Bgm.Play();
        // 値をロードしてスライダーとミキサーに反映
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 0f);
        float se = PlayerPrefs.GetFloat(SE_KEY, 0f);

        masterSlider.value = master;
        bgmSlider.value = bgm;
        seSlider.value = se;

        audioMixer.SetFloat("Master", master);
        audioMixer.SetFloat("BGMVolume", bgm);
        audioMixer.SetFloat("SEVolume", se);

        // スライダー操作イベントを登録
        masterSlider.onValueChanged.AddListener(masterSet);
        bgmSlider.onValueChanged.AddListener(bgmSet);
        seSlider.onValueChanged.AddListener(seSet);
    }

    public void ReturnScene()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade(SceneReturnTarget, Color.black, 0.5f);
        PlayerPrefs.Save();
    }

    private void masterSet(float value)
    {
        audioMixer.SetFloat("Master", value);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
        PlayerPrefs.Save();
    }

    private void bgmSet(float value)
    {
        audioMixer.SetFloat("BGM", value);
        PlayerPrefs.SetFloat(BGM_KEY, value);
        PlayerPrefs.Save();
    }

    private void seSet(float value)
    {
        audioMixer.SetFloat("SE", value);
        PlayerPrefs.SetFloat(SE_KEY, value);
        PlayerPrefs.Save();
    }
}
