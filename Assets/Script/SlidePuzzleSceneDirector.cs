using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlidePuzzleSceneDirector : MonoBehaviour
{
    // ピース
    [SerializeField] List<GameObject> pieceList;
    [SerializeField] GameObject ParentPieces;
    // ゲームクリア時に表示されるボタン
    [SerializeField] public GameObject buttonRetry;
    // シャッフル回数
    [SerializeField] public int shuffleCount;

    List<GameObject> children;




    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if (i != 3 || j != 3)
                {
                    Object.Instantiate(pieceList[Random.Range(0, 6)], new Vector3(-1.5f + i, 1.5f - j, 0f), Quaternion.identity, ParentPieces.transform);
                }
            }
        }
        children = new List<GameObject>();

        foreach (Transform child in ParentPieces.transform)
        {
            children.Add(child.gameObject);
        }
        // 指定回数シャッフル
        for (int i = 0; i < shuffleCount; i++)
        {
            // 0番と隣接するピース
            List<GameObject> movablepieceList = new List<GameObject>();

            // 0番と隣接するピースをリストに追加
            foreach (var item in pieceList)
            {
                if (GetEmptyPiece(item) != null)
                {
                    movablepieceList.Add(item);
                }
            }
            
            // 隣接するピースをランダムで入れかえる
            int rnd = Random.Range(0, movablepieceList.Count);
            GameObject piece = movablepieceList[rnd];
            SwapPiece(piece, pieceList[0]);
        }

        // ボタン非表示
        buttonRetry.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // タッチ処理
        if(Input.GetMouseButtonUp(0))
        {
            // スクリーン座標からワールド座標に変換
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // レイを飛ばす
            RaycastHit2D hit2d = Physics2D.Raycast(worldPoint, Vector2.zero);

            // 当たり判定があった
            if(hit2d)
            {
                // ヒットしたゲームオブジェクト
                GameObject hitPiece = hit2d.collider.gameObject;
                // 0番のピースと隣接していればデータが入る
                GameObject emptyPiece = GetEmptyPiece(hitPiece);
                // 選んだピースと0番のピースを入れかえる
                SwapPiece(hitPiece, emptyPiece);

                // クリア判定
                buttonRetry.SetActive(true);

                // 正解の位置と違うピースを探す
                for (int i = 0; i < pieceList.Count; i++)
                {
                    // 現在のポジション
                    Vector2 position = pieceList[i].transform.position;
                    // 初期位置と違ったらボタンを非表示

                        buttonRetry.SetActive(false);
                    
                }

                // クリア状態
                if(buttonRetry.activeSelf)
                {
                    Debug.Log("クリア！！");
                    buttonRetry.SetActive(true);
                }
            }
        }
        
    }

    // 引数のピースが0番のピースと隣接していたら0番のピースを返す
    GameObject GetEmptyPiece(GameObject piece)
    {
        // 2点間の距離を代入
        float dist =
            Vector2.Distance(piece.transform.position, pieceList[0].transform.position);

        // 距離が1なら0番のピースを返す（2個以上離れていたり、斜めの場合は1より大きい距離になる）
        if (dist == 1)
        {
            return pieceList[0];
        }

        return null;
    }

    // 2つのピースの位置を入れかえる
    void SwapPiece(GameObject pieceA, GameObject pieceB)
    {
        // どちらかがnullなら処理をしない
        if (pieceA == null || pieceB == null)
        {
            return;
        }

        // AとBのポジションを入れかえる
        Vector2 position = pieceA.transform.position;
        pieceA.transform.position = pieceB.transform.position;
        pieceB.transform.position = position;
    }

    // リトライボタン
    public void OnClickRetry()
    {
        SceneManager.LoadScene("SlidePuzzleScene");
    }
}
