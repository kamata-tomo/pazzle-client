using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Piece;

public class SlidePuzzleSceneDirector : MonoBehaviour
{
    // ピース
    [SerializeField] List<GameObject> pieceList;
    [SerializeField] GameObject ParentPieces;
    [SerializeField] GameObject StartPiece;
    [SerializeField] GameObject GoalPiece;
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
        buttonRetry.SetActive(false);
        buttonStart.SetActive(true);
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
            List<GameObject> movablechildren = new List<GameObject>();

            // 0番と隣接するピースをリストに追加
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
                // 0番のピースと隣接していればデータが入る
                GameObject emptyPiece = GetEmptyPiece(hitPiece);
                // 選んだピースと0番のピースを入れかえる
                SwapPiece(hitPiece, emptyPiece);


            }
        }
        
    }

    // 引数のピースが0番のピースと隣接していたら0番のピースを返す
    GameObject GetEmptyPiece(GameObject piece)
    {
        // 2点間の距離を代入
        float dist =
            Vector2.Distance(piece.transform.position, children[0].transform.position);

        // 距離が1なら0番のピースを返す（2個以上離れていたり、斜めの場合は1より大きい距離になる）
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
