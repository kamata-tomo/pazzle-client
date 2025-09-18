using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public Text nameText;
    public Text IdText;
    public Text levelText;
    public Transform titleGridParent; // Grid (3x3) �Ń^�C�g���A�C�R���z�u
    public GameObject titleIconPrefab; // Image�t��Prefab

    private int friendId;
    private ProfileManager manager;

    public void Setup(FriendData data, ProfileManager mgr, TitleIconManager iconMgr)
    {
        friendId = data.Id;
        manager = mgr;

        nameText.text = data.Name;
        IdText.text = $"ID.{data.Id}";
        levelText.text = $"Lv.{data.Level}";

        // �̍��\�����Z�b�g
        foreach (Transform child in titleGridParent) Destroy(child.gameObject);

        var maxRanks = TitleHelper.GetChapterMaxTitles(data.Titles);
        for (int chapter = 1; chapter <= 9; chapter++)
        {
            int rank = maxRanks.ContainsKey(chapter) ? maxRanks[chapter] : 0;
            GameObject obj = Instantiate(titleIconPrefab, titleGridParent);
            var img = obj.GetComponent<Image>();
            img.sprite = rank > 0 ? iconMgr.GetIcon(chapter, rank) : null;
        }
    }

    public void OnSendStamina()
    {
        StartCoroutine(NetworkManager.Instance.ProviderStamina(friendId, (success) =>
        {
            if (success) manager.ReloadFriends();
        }));
    }
}
