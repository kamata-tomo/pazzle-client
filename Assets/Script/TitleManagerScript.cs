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

        // �Z�[�u�f�[�^�����邩�`�F�b�N
        if (NetworkManager.Instance.LoadUserData())
        {
            // ���[�U�[���݊m�FAPI���Ă�
            StartCoroutine(NetworkManager.Instance.CheckUserExists(exists =>
            {
                if (exists)
                {
                    // ���[�U�[�����݂���̂Ŏ��̃V�[����
                    Initiate.Fade("HomeScenes", Color.black, 0.5f);
                }
                else
                {
                    // ���[�U�[�����݂��Ȃ��̂ŐV�K�쐬��ʂ�
                    RegistUserForm.SetActive(true);
                }
            }));
        }
        else
        {
            // �Z�[�u�f�[�^���Ȃ��̂ŐV�K�쐬��ʂ�
            RegistUserForm.SetActive(true);
        }
    }

    public void NewRegistration()
    {
        se.PlayOneShot(ButtonSoundEffect);
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("���O�����͂���Ă��܂���");
            return;
        }

        // NetworkManager�̓o�^�������Ă�
        StartCoroutine(NetworkManager.Instance.RegistUser(playerName, (bool success) =>
        {
            if (success)
            {
                Debug.Log("���[�U�[�o�^�����I");
                // �o�^��̓��C���V�[���֑J��
                Initiate.Fade("HomeScenes", Color.black, 0.5f);
            }
            else
            {
                Debug.LogError("���[�U�[�o�^���s");
                // ���s���̃G���[���b�Z�[�W��UI�\�����Ă��ǂ�
            }
        }));
    }

}
