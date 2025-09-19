using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject profilePanel;
    public GameObject friendsPanel;
    public GameObject requestsPanel;
    public GameObject othersPanel;

    [Header("Friend List")]
    public Transform friendListParent;
    public GameObject friendItemPrefab;

    [Header("Friend Requests")]
    public Transform requestListParent;
    public GameObject requestItemPrefab;

    [Header("Other Users")]
    public Transform otherListParent;
    public GameObject otherItemPrefab;

    [Header("Search")]
    public InputField searchInput;

    [Header("Title Icons")]
    public TitleIconManager iconManager;

    [Header("Exp Bar")]
    [SerializeField] Slider experienceSlider;
    [SerializeField] Text experienceText;

    [Header("Profile UI")]
    public Text userNameText;
    public Text userLevelText;
    public Transform myTitlesGridParent;
    public GameObject titleIconPrefab;

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource se;
    [SerializeField] AudioClip ButtonSoundEffect;

    [Header("Change Name Window")]
    public GameObject changeNameWindow;
    public InputField inputFieldNewName;

    private void Start()
    {
        ShowProfile();
        bgm.Play();
    }

    // ==== �p�l���؂�ւ� ====
    public void ShowProfile()
    {
        se.PlayOneShot(ButtonSoundEffect);
        profilePanel.SetActive(true);
        friendsPanel.SetActive(false);
        requestsPanel.SetActive(false);
        othersPanel.SetActive(false);

        StartCoroutine(LoadMyProfile());
    }

    public void ShowFriends()
    {
        se.PlayOneShot(ButtonSoundEffect);
        profilePanel.SetActive(false);
        friendsPanel.SetActive(true);
        requestsPanel.SetActive(false);
        othersPanel.SetActive(false);

        StartCoroutine(LoadFriends());
    }

    public void ShowRequests()
    {
        se.PlayOneShot(ButtonSoundEffect);
        profilePanel.SetActive(false);
        friendsPanel.SetActive(false);
        requestsPanel.SetActive(true);
        othersPanel.SetActive(false);

        StartCoroutine(LoadRequests());
    }

    public void ShowOthers()
    {
        se.PlayOneShot(ButtonSoundEffect);
        profilePanel.SetActive(false);
        friendsPanel.SetActive(false);
        requestsPanel.SetActive(false);
        othersPanel.SetActive(true);

        StartCoroutine(LoadOthers());
    }

    // ==== �����̃v���t�B�[�����[�h ====
    private IEnumerator LoadMyProfile()
    {
        // --- ���[�U�[��{���擾 ---
        yield return NetworkManager.Instance.GetUser((user) =>
        {
            if (user == null) return;

            // ���O�E���x���\��
            userNameText.text = user.Name;
            userLevelText.text = $"Lv.{user.Level}";

            // �o���l�o�[�X�V
            UpdateExpBar(user);
        });

        // --- �̍��f�[�^�擾 ---
        yield return NetworkManager.Instance.GetTitles((titles) =>
        {
            if (titles == null) return;

            // �\�����Z�b�g
            foreach (Transform child in myTitlesGridParent)
                Destroy(child.gameObject);

            // Chapter���Ƃ̍ő�]�����Z�o
            var maxRanks = TitleHelper.GetChapterMaxTitles(titles);

            // Chapter1�`9��K���\���i���擾�͋�j
            for (int chapter = 1; chapter <= 9; chapter++)
            {
                int rank = maxRanks.ContainsKey(chapter) ? maxRanks[chapter] : 0;

                GameObject obj = Instantiate(titleIconPrefab, myTitlesGridParent);
                var img = obj.GetComponent<Image>();

                if (rank > 0)
                    img.sprite = iconManager.GetIcon(chapter, rank);
                else
                    img.sprite = null; // ���擾�͋�
            }
        });
    }

    // ==== ���O�ύXUI���� ====
    public void OnClickChangeName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        changeNameWindow.SetActive(true); // �E�B���h�E�\��
    }

    public void OnClickCloseChangeName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        changeNameWindow.SetActive(false); // �E�B���h�E��\��
    }

    public void OnClickUpdateName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        string newName = inputFieldNewName.text.Trim();
        if (string.IsNullOrEmpty(newName)) return;

        // experience �� item_quantity �͍X�V���Ȃ��̂� null ��n��
        StartCoroutine(NetworkManager.Instance.UpdateUser(newName, null, null, (response) =>
        {
            if (response != null)
            {
                // ����������v���t�B�[�����ēǂݍ���
                StartCoroutine(LoadMyProfile());
                changeNameWindow.SetActive(false);
            }
            else
            {
                Debug.LogError("���O�̍X�V�Ɏ��s���܂���");
            }
        }));
    }


    // ==== ���X�g���[�h ====
    private IEnumerator LoadFriends()
    {
        foreach (Transform child in friendListParent) Destroy(child.gameObject);

        yield return NetworkManager.Instance.GetFriends((friends) =>
        {
            if (friends == null) return;
            foreach (var f in friends)
            {
                GameObject obj = Instantiate(friendItemPrefab, friendListParent);
                obj.GetComponent<FriendItem>().Setup(f, this, iconManager);
            }
        });
    }

    private IEnumerator LoadRequests()
    {
        foreach (Transform child in requestListParent) Destroy(child.gameObject);

        yield return NetworkManager.Instance.GetFriendRequests((json) =>
        {
            if (string.IsNullOrEmpty(json)) return;
            var requests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FriendRequestData>>(json);
            foreach (var r in requests)
            {
                GameObject obj = Instantiate(requestItemPrefab, requestListParent);
                obj.GetComponent<FriendRequestItem>().Setup(r, this, iconManager);
            }
        });
    }

    private IEnumerator LoadOthers()
    {
        foreach (Transform child in otherListParent) Destroy(child.gameObject);

        yield return NetworkManager.Instance.GetAllOtherUsers((users) =>
        {
            if (users == null) return;
            foreach (var u in users)
            {
                GameObject obj = Instantiate(otherItemPrefab, otherListParent);
                obj.GetComponent<OtherUserItem>().Setup(u, this, iconManager);
            }
        });
    }

    // ==== �������� ====
    public void OnSearchUser()
    {
        se.PlayOneShot(ButtonSoundEffect);
        int id;
        if (!int.TryParse(searchInput.text, out id)) return;
        StartCoroutine(SearchUserById(id));
    }

    private IEnumerator SearchUserById(int id)
    {
        foreach (Transform child in otherListParent) Destroy(child.gameObject);

        yield return NetworkManager.Instance.GetUserById(id, (user) =>
        {
            if (user == null) return;
            GameObject obj = Instantiate(otherItemPrefab, otherListParent);
            obj.GetComponent<OtherUserItem>().Setup(user, this, iconManager);
        });
    }

    // ==== �o���l�o�[�X�V ====
    public void UpdateExpBar(ShowUserResponse user)
    {
        int needExp = user.Level * 10;
        if (experienceSlider != null)
        {
            experienceSlider.maxValue = needExp;
            experienceSlider.value = user.Experience;
        }
        if (experienceText != null)
            experienceText.text = $"{user.Experience}/{needExp}";
    }

    // ==== �ēǂݍ��ݗp ====
    public void ReloadFriends() => StartCoroutine(LoadFriends());
    public void ReloadRequests() => StartCoroutine(LoadRequests());
    public void ReloadOthers() => StartCoroutine(LoadOthers());

    // ==== �߂�{�^�� ====
    public void ReturnHome()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }
}
