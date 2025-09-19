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

    // ==== パネル切り替え ====
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

    // ==== 自分のプロフィールロード ====
    private IEnumerator LoadMyProfile()
    {
        // --- ユーザー基本情報取得 ---
        yield return NetworkManager.Instance.GetUser((user) =>
        {
            if (user == null) return;

            // 名前・レベル表示
            userNameText.text = user.Name;
            userLevelText.text = $"Lv.{user.Level}";

            // 経験値バー更新
            UpdateExpBar(user);
        });

        // --- 称号データ取得 ---
        yield return NetworkManager.Instance.GetTitles((titles) =>
        {
            if (titles == null) return;

            // 表示リセット
            foreach (Transform child in myTitlesGridParent)
                Destroy(child.gameObject);

            // Chapterごとの最大評価を算出
            var maxRanks = TitleHelper.GetChapterMaxTitles(titles);

            // Chapter1～9を必ず表示（未取得は空）
            for (int chapter = 1; chapter <= 9; chapter++)
            {
                int rank = maxRanks.ContainsKey(chapter) ? maxRanks[chapter] : 0;

                GameObject obj = Instantiate(titleIconPrefab, myTitlesGridParent);
                var img = obj.GetComponent<Image>();

                if (rank > 0)
                    img.sprite = iconManager.GetIcon(chapter, rank);
                else
                    img.sprite = null; // 未取得は空
            }
        });
    }

    // ==== 名前変更UI制御 ====
    public void OnClickChangeName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        changeNameWindow.SetActive(true); // ウィンドウ表示
    }

    public void OnClickCloseChangeName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        changeNameWindow.SetActive(false); // ウィンドウ非表示
    }

    public void OnClickUpdateName()
    {
        se.PlayOneShot(ButtonSoundEffect);
        string newName = inputFieldNewName.text.Trim();
        if (string.IsNullOrEmpty(newName)) return;

        // experience と item_quantity は更新しないので null を渡す
        StartCoroutine(NetworkManager.Instance.UpdateUser(newName, null, null, (response) =>
        {
            if (response != null)
            {
                // 成功したらプロフィールを再読み込み
                StartCoroutine(LoadMyProfile());
                changeNameWindow.SetActive(false);
            }
            else
            {
                Debug.LogError("名前の更新に失敗しました");
            }
        }));
    }


    // ==== リストロード ====
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

    // ==== 検索処理 ====
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

    // ==== 経験値バー更新 ====
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

    // ==== 再読み込み用 ====
    public void ReloadFriends() => StartCoroutine(LoadFriends());
    public void ReloadRequests() => StartCoroutine(LoadRequests());
    public void ReloadOthers() => StartCoroutine(LoadOthers());

    // ==== 戻るボタン ====
    public void ReturnHome()
    {
        se.PlayOneShot(ButtonSoundEffect);
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }
}
