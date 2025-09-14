using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Piece;

public class SlidePuzzleSceneDirector : MonoBehaviour
{
    // ピース
    [SerializeField] List<GameObject> pieceList;//空白ピースを除くピースPrefabのリスト
    [SerializeField] GameObject emptyPiece;//空白ピースPrefab
    [SerializeField] GameObject ParentPieces;//ピースをInstantiateする先
    [SerializeField] GameObject StartPiece;//
    [SerializeField] GameObject GoalPiece;//
    // ゲームクリア時に表示されるボタン
    [SerializeField]  GameObject buttonRetry;
    [SerializeField] GameObject buttonStart;
    // シャッフル回数
    [SerializeField]  int shuffleCount;
    [SerializeField] Player player;

    List<GameObject> children;




    // Start is called before the first frame update
    void Start()
    {
        int pieceType = 6;//ピース6は空白
        buttonRetry.SetActive(false);
        buttonStart.SetActive(true);
        for (int y = 0; y < 4; y++)
        {
            for(int x = 0; x < 4; x++)
            {
                if (pieceType != 6)
                {
                    Object.Instantiate(pieceList[pieceType], new Vector3(-1.5f + x, 1.5f - y, 0f), Quaternion.identity, ParentPieces.transform);
                }
                else
                {
                    Object.Instantiate(emptyPiece, new Vector3(-1.5f + x, 1.5f - y, 0f), Quaternion.identity, ParentPieces.transform);
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
            // 空白と隣接するピース
            List<GameObject> movablechildren = new List<GameObject>();

            //空白と隣接するピースをリストに追加
            foreach (var item in children)
            {
                if (GetEmptyPiece(item) != null)
                {
                    movablechildren.Add(item);
                }
            }
            
            // 隣接するピースをランダムで入れかえる
            int rnd = Random.Range(0, movablechildren.Count);
            GameObject piece = movablechildren[rnd];
            SwapPiece(piece, children[0]);
        }


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
        // 2点間の距離を代入
        float dist =
            Vector2.Distance(piece.transform.position, children[0].transform.position);

        // 距離が1なら空白のピースを返す（2個以上離れていたり、斜めの場合は1より大きい距離になる）
        if (dist == 1)
        {
            return children[0];
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

        player.Retry();
    }

    public void OnClickStart()
    {

        foreach (GameObject piece in children)
        {
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.1f);
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
        Initiate.Fade("ResultScene", Color.black, 0.5f);
    }
}
