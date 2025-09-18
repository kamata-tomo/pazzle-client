using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public EntryPlace playersDirection;
    public float moveSpeed = 5f;//プレイヤーの移動速度
    private Vector2 moveDirection = Vector2.zero;//現在の移動方向
    public bool isMoving = false;//プレイヤーが移動中かどうか
    Animator PlayersAnimator;
    public bool Fail;
    public bool ReadyToStart;
    public SlidePuzzleSceneDirector SlidePuzzleSceneDirector;

    Rigidbody2D rb;
    public enum EntryPlace
    {
        TOP,
        BOTTOM,
        RIGHT,
        LEFT
    }
    void Start()
    {
        this.gameObject.transform.position = new Vector3(1.5f, -2.5f, 0f);         
        rb = GetComponent<Rigidbody2D>();
        PlayersAnimator = GetComponent<Animator>();
        playersDirection = EntryPlace.TOP;
        PlayersAnimator.SetInteger("playersDirection", 0);
        Fail = false;
        ReadyToStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (isMoving)
        {   //移動処理
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        Debug.Log("触れた");
        Piece targetPieceSc = collision.transform.GetComponent<Piece>();
        BoxCollider2D targetPiece = collision.GetComponent<BoxCollider2D>();

        if (isMoving)
        {
            if (targetPiece.tag == "piece")
            {
                Debug.Log($"{targetPiece}コライダー非アクティブ");
                targetPiece.enabled = false;
            }
            if (targetPieceSc.goal)
            {
                isMoving = false;
                SlidePuzzleSceneDirector.GameClear();
                Fail = true;
                Debug.Log("Goal");
                return;
            }
            if (targetPieceSc.collectible)
            {
                SlidePuzzleSceneDirector.hideCollectible();
                StageResultData.Collectible = true;
            }
            if (playersDirection == EntryPlace.TOP && targetPieceSc.bottom)
            {
                targetPieceSc.entry = Piece.EntryPlace.BOTTOM;
                RotationandDirections(targetPieceSc);
            }
            else if (playersDirection == EntryPlace.BOTTOM && targetPieceSc.top)
            {
                targetPieceSc.entry = Piece.EntryPlace.TOP;
                RotationandDirections(targetPieceSc);
            }
            else if (playersDirection == EntryPlace.RIGHT && targetPieceSc.left)
            {
                targetPieceSc.entry = Piece.EntryPlace.LEFT;
                RotationandDirections(targetPieceSc);
            }
            else if (playersDirection == EntryPlace.LEFT && targetPieceSc.right)
            {
                targetPieceSc.entry = Piece.EntryPlace.RIGHT;
                RotationandDirections(targetPieceSc);
            }
            else if(!targetPieceSc.collectible)
            {
                isMoving = false;
                Fail = true;
                Debug.Log("進入方向の誤り");
                return;
            }
        }
    }


    void RotationandDirections(Piece targetPieceSc)
    {
        if (targetPieceSc.top&& targetPieceSc.entry != Piece.EntryPlace.TOP)
        {
            playersDirection = EntryPlace.TOP;
            PlayersAnimator.SetInteger("playersDirection", 0);
            moveDirection = new Vector2(0, 1);
        }
        else if (targetPieceSc.bottom && targetPieceSc.entry != Piece.EntryPlace.BOTTOM)
        {
            playersDirection = EntryPlace.BOTTOM;
            PlayersAnimator.SetInteger("playersDirection", 1);
            moveDirection = new Vector2(0, -1);
        }
        else if (targetPieceSc.left && targetPieceSc.entry != Piece.EntryPlace.LEFT)
        {
            playersDirection = EntryPlace.LEFT;
            PlayersAnimator.SetInteger("playersDirection", 2);
            moveDirection = new Vector2(-1, 0);
        }
        else if (targetPieceSc.right && targetPieceSc.entry != Piece.EntryPlace.RIGHT)
        {
            playersDirection = EntryPlace.RIGHT;
            PlayersAnimator.SetInteger("playersDirection", 3);
            moveDirection = new Vector2(1, 0);
        }
    }
    public void Retry()
    {
        this.gameObject.transform.position = new Vector3(1.5f, -2.5f, 0f);
        playersDirection = EntryPlace.TOP;
        PlayersAnimator.SetInteger("playersDirection", 0);
        StageResultData.Collectible = false;
        Fail = false;
        ReadyToStart = true;
        SlidePuzzleSceneDirector.ResetCollider();
    }
    public void gameStart()
    {
        isMoving = true;
        ReadyToStart = false;
        moveDirection = new Vector2(0, 1);
    }
}
