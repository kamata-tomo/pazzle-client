using UnityEngine;
using UnityEngine.UI;

public class OtherUserItem : MonoBehaviour
{
    public Text nameText;
    public Text IdText;
    public Text levelText;
    public Button requestButton;
    public Transform titleGridParent;
    public GameObject titleIconPrefab;
    [SerializeField] AudioClip ButtonSoundEffect;


    private int userId;
    private ProfileManager manager;

    public void Setup(OtherUserData data, ProfileManager mgr, TitleIconManager iconMgr)
    {
        userId = data.Id;
        manager = mgr;

        nameText.text = data.Name;
        IdText.text = $"ID.{data.Id}";
        levelText.text = $"Lv.{data.Level}";

        foreach (Transform child in titleGridParent) Destroy(child.gameObject);

        var maxRanks = TitleHelper.GetChapterMaxTitles(data.Titles);
        for (int chapter = 1; chapter <= 9; chapter++)
        {
            int rank = maxRanks.ContainsKey(chapter) ? maxRanks[chapter] : 0;
            GameObject obj = Instantiate(titleIconPrefab, titleGridParent);
            var img = obj.GetComponent<Image>();
            img.sprite = rank > 0 ? iconMgr.GetIcon(chapter, rank) : null;
        }

        if (data.IsFriend || data.IsRequestSent || data.IsRequestReceived)
            requestButton.interactable = false;
        else
            requestButton.interactable = true;
    }

    public void OnRequest()
    {
        GameObject.Find("SE").GetComponent<AudioSource>().PlayOneShot(ButtonSoundEffect);
        StartCoroutine(NetworkManager.Instance.StoreFriendRequest(userId, (success) =>
        {
            if (success) manager.ReloadOthers();
        }));
    }
}
