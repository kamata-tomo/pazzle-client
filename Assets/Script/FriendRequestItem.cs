using UnityEngine;
using UnityEngine.UI;

public class FriendRequestItem : MonoBehaviour
{
    public Text nameText;
    public Text IdText;
    public Text levelText;
    public Transform titleGridParent;
    public GameObject titleIconPrefab;

    private int requesterId;
    private ProfileManager manager;

    public void Setup(FriendRequestData data, ProfileManager mgr, TitleIconManager iconMgr)
    {
        requesterId = data.RequestingUser.Id;
        manager = mgr;

        nameText.text = data.RequestingUser.Name;
        IdText.text = $"ID.{data.Id}";
        levelText.text = $"Lv.{data.RequestingUser.Level}";

        foreach (Transform child in titleGridParent) Destroy(child.gameObject);

        var maxRanks = TitleHelper.GetChapterMaxTitles(data.RequestingUser.Titles);
        for (int chapter = 1; chapter <= 9; chapter++)
        {
            int rank = maxRanks.ContainsKey(chapter) ? maxRanks[chapter] : 0;
            GameObject obj = Instantiate(titleIconPrefab, titleGridParent);
            var img = obj.GetComponent<Image>();
            img.sprite = rank > 0 ? iconMgr.GetIcon(chapter, rank) : null;
        }
    }

    public void OnAccept()
    {
        StartCoroutine(NetworkManager.Instance.StoreFriend(requesterId, (success) =>
        {
            if (success) manager.ReloadRequests();
        }));
    }
}
