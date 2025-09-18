using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Piece;
public static class StageSelectData
{
    public static int SelectedStageId { get; set; }
    public static int shuffle_count { get; set; }
    public static int ChapterNum { get; set; }
    public static int reference_value_1 { get; set; }
    public static int reference_value_2 { get; set; }
    public static int reference_value_3 { get; set; }

}
public class SlidePuzzleSceneDirector : MonoBehaviour
{
    // ピース
    [SerializeField] List<GameObject> pieceList;//空白ピースを除くピースPrefabのリスト
    [SerializeField] GameObject emptyPiece;//空白ピースPrefab
    [SerializeField] GameObject ParentPieces;//ピースをInstantiateする先
    [SerializeField] GameObject StartPiece;//
    [SerializeField] GameObject GoalPiece;//
    [SerializeField] GameObject collectiblePiece;
    [SerializeField] GameObject collectibleParentPiece;
    // ゲームクリア時に表示されるボタン
    [SerializeField]  GameObject buttonRetry;
    [SerializeField] GameObject buttonStart;
    // シャッフル回数
    [SerializeField] Player player;
    [SerializeField] Slider evaluationSlider;
    [SerializeField] Text replaceCountText;
    [SerializeField] Transform markerParent;

    List<GameObject> children;
    private List<StageCellData> OnCellsLoaded;
    private GameObject currentEmptyPiece; // 空白ピースを保持する変数
    private int ReplaceCount;
    private GameObject InstantiatecollectiblePiece;

    void Start()
    {
        StageResultData.Collectible =false;
        int stageId = StageSelectData.SelectedStageId;
        Debug.Log("受け取ったステージID: " + stageId);
        SetupEvaluationUI();
        // API呼び出しでステージデータ取得
        StartCoroutine(NetworkManager.Instance.GetCells(stageId, (cells) =>
        {
            if (cells == null)
            {
                Debug.LogError("ステージデータの取得に失敗しました");
                return;
            }

            OnCellsLoaded = cells;

            // 一旦 ParentPieces 内の子をクリア
            foreach (Transform child in ParentPieces.transform)
            {
                Destroy(child.gameObject);
            }

            // ピース生成
            foreach (StageCellData cell in OnCellsLoaded)
            {
                Vector3 pos = new Vector3(-1.5f + cell.X, 1.5f - cell.Y, 0f);
                GameObject pieceObj;

                if (cell.PieceType == 6)
                {
                    pieceObj = Instantiate(emptyPiece, pos, Quaternion.identity, ParentPieces.transform);
                    currentEmptyPiece = pieceObj;
                }
                else
                {
                    pieceObj = Instantiate(pieceList[cell.PieceType], pos, Quaternion.identity, ParentPieces.transform);
                }

                // collectibles が true の場合、前面に collectiblePiece を生成
                if (cell.Collectibles)
                {
                    InstantiatecollectiblePiece = Instantiate(collectiblePiece, pos, Quaternion.identity, collectibleParentPiece.transform);
                }
            }

            // children リストを更新
            children = new List<GameObject>();
            foreach (Transform child in ParentPieces.transform)
            {
                children.Add(child.gameObject);
            }

            // 指定回数シャッフル
            for (int i = 0; i < StageSelectData.shuffle_count; i++)
            {
                List<GameObject> movablechildren = new List<GameObject>();
                foreach (var item in children)
                {
                    if (GetEmptyPiece(item) != null)
                    {
                        movablechildren.Add(item);
                    }
                }

                if (movablechildren.Count > 0)
                {
                    int rnd = Random.Range(0, movablechildren.Count);
                    GameObject piece = movablechildren[rnd];
                    SwapPiece(piece, currentEmptyPiece, false);
                }
            }

            buttonRetry.SetActive(false);
            buttonStart.SetActive(true);
        }));
    }


    // Update is called once per frame
    void Update()
    {
        buttonRetry.SetActive(player.Fail);
        buttonStart.SetActive(player.ReadyToStart);
        // タッチ処理
        if (Input.GetMouseButton(0))
        {
            // スクリーン座標からワールド座標に変換
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // レイを飛ばす
            RaycastHit2D hit2d = Physics2D.Raycast(worldPoint, Vector2.zero);

            // 当たり判定があった
            if (hit2d && hit2d.collider.gameObject.tag == "piece")
            {
                // ヒットしたゲームオブジェクト
                GameObject hitPiece = hit2d.collider.gameObject;
                // 空白のピースと隣接していればデータが入る
                GameObject emptyPiece = GetEmptyPiece(hitPiece);
                // 選んだピースと空白のピースを入れかえる
                SwapPiece(hitPiece, emptyPiece);


            }
        }
        
    }

    // 引数のピースが空白のピースと隣接していたら空白のピースを返す
    GameObject GetEmptyPiece(GameObject piece)
    {
        if (currentEmptyPiece == null) return null;

        float dist = Vector2.Distance(piece.transform.position, currentEmptyPiece.transform.position);

        if (dist == 1)
        {
            return currentEmptyPiece;
        }
        return null;
    }


    // 2つのピースの位置を入れかえる
    void SwapPiece(GameObject pieceA, GameObject pieceB, bool countMove = true)
    {
        if (pieceA == null || pieceB == null) return;

        if (countMove)
        {
            ReplaceCount++;
            replaceCountText.text = $"Moves:{ReplaceCount}";
            evaluationSlider.value = Mathf.Min(ReplaceCount, evaluationSlider.maxValue);
        }

        Vector2 position = pieceA.transform.position;
        pieceA.transform.position = pieceB.transform.position;
        pieceB.transform.position = position;
    }

    private void SetupEvaluationUI()
    {
        ReplaceCount = 0;

        // Slider セットアップ
        evaluationSlider.minValue = 0;
        evaluationSlider.maxValue = StageSelectData.reference_value_1; // 星1基準を最大値に
        evaluationSlider.value = 0;

        // 基準点の目印をUIに生成
        CreateMarker(StageSelectData.reference_value_3, "★3");
        CreateMarker(StageSelectData.reference_value_2, "★2");
        CreateMarker(StageSelectData.reference_value_1, "★1");
    }

    private void CreateMarker(int value, string label)
    {
        GameObject marker = new GameObject("Marker_" + label);
        marker.transform.SetParent(markerParent, false);

        // RectTransformを追加してサイズを十分に確保
        RectTransform rt = marker.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 0); 

        // Text コンポーネント
        Text txt = marker.AddComponent<Text>();
        txt.text = $"{label} ({value})";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 60; // 大きめの文字サイズ
        txt.alignment = TextAnchor.MiddleCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.color = Color.white;

        // スライダーに応じた位置へ配置
        if (evaluationSlider != null)
        {
            float normalizedValue = Mathf.InverseLerp(
                evaluationSlider.minValue,
                evaluationSlider.maxValue,
                value
            );

            // スライダーの幅からX座標を計算
            RectTransform sliderRT = evaluationSlider.GetComponent<RectTransform>();
            float sliderWidth = sliderRT.rect.width;
            float xPos = (normalizedValue - 0.5f) * sliderWidth;

            rt.anchoredPosition = new Vector2(xPos, 0f);
        }
    }


    private int CalculateEvaluation()
    {
        if (ReplaceCount <= StageSelectData.reference_value_3) return 3;
        if (ReplaceCount <= StageSelectData.reference_value_2) return 2;
        if (ReplaceCount <= StageSelectData.reference_value_1) return 1;
        return 0; // 0=クリアできなかった
    }

    // リトライボタン
    public void OnClickRetry()
    {
        foreach (Transform collectible in collectibleParentPiece.transform)
        {
            collectible.gameObject.SetActive(true);
        }
        player.Retry();
    }

    public void hideCollectible()
    {
        foreach (Transform collectible in collectibleParentPiece.transform)
        {
            collectible.gameObject.SetActive(false);
        }
    }
public void OnClickStart()
    {
        // 子オブジェクトを正しくGameObjectとして扱う
        

        foreach (GameObject piece in children)
        {
            if (piece.CompareTag("piece")) // GetComponent<GameObject>() は不要
            {
                piece.GetComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.1f);
            }
        }
        foreach (Transform collectible in collectibleParentPiece.transform)
        {
            collectible.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            collectible.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        }

        player.gameStart();
    }

    public void ResetCollider()
    {
        GoalPiece.GetComponent<BoxCollider2D>().enabled = true;
        foreach (GameObject piece in children)
        {
            piece.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            piece.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    public void GameClear()
    {
        StageResultData.StageId = StageSelectData.SelectedStageId;
        StageResultData.Evaluation = CalculateEvaluation();
        StageResultData.ChapterNum = StageSelectData.ChapterNum;
        Initiate.Fade("ResultScene", Color.black, 0.5f);
    }
}
