using System.Collections.Generic;

public static class TitleHelper
{
    // titles ���� Chapter���Ƃ̍ő僉���N���Z�o
    public static Dictionary<int, int> GetChapterMaxTitles(List<TitleData> titles)
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
