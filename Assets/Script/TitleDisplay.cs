using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDisplay : MonoBehaviour
{
    public Transform gridParent;
    public GameObject iconPrefab;
    public TitleIconManager iconManager;

    public void ShowTitles(List<TitleData> titles)
    {
        // �ő�l�v�Z
        var maxRanks = GetChapterMaxTitles(titles);

        // �\���N���A
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        // 3�~3�ŕ\�� (Chapter1�`9)
        for (int i = 1; i <= 9; i++)
        {
            int rank = maxRanks.ContainsKey(i) ? maxRanks[i] : 0;

            GameObject obj = Instantiate(iconPrefab, gridParent);
            var img = obj.GetComponent<UnityEngine.UI.Image>();

            if (rank > 0)
                img.sprite = iconManager.GetIcon(i, rank);
            else
                img.sprite = null; // ���擾�Ȃ��\��
        }
    }

    private Dictionary<int, int> GetChapterMaxTitles(List<TitleData> titles)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        foreach (var t in titles)
        {
            int chapter = (t.Id - 1) / 3 + 1;
            int rank = (t.Id - 1) % 3 + 1;
            if (!result.ContainsKey(chapter) || rank > result[chapter])
                result[chapter] = rank;
        }
        return result;
    }
}
