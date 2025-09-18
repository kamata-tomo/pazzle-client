using System.Collections.Generic;
using UnityEngine;

public class TitleIconManager : MonoBehaviour
{
    [Header("タイトル画像 [chapter][rank]")]
    public Sprite[][] titleIcons; // 9 x 3

    // Unityエディタでは扱えないので内部的には [chapter][rank] を埋める形にする
    public Sprite[] flatIcons; // サイズ 9*3
    private const int Chapters = 9;

    void Awake()
    {
        titleIcons = new Sprite[Chapters][];
        for (int i = 0; i < Chapters; i++)
        {
            titleIcons[i] = new Sprite[3];
            for (int r = 0; r < 3; r++)
            {
                titleIcons[i][r] = flatIcons[i * 3 + r];
            }
        }
    }

    public Sprite GetIcon(int chapter, int rank)
    {
        if (chapter < 1 || chapter > Chapters) return null;
        if (rank < 1 || rank > 3) return null;
        return titleIcons[chapter - 1][rank - 1];
    }
}
