using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piece : MonoBehaviour
{
    public bool top;
    public bool bottom;
    public bool right;
    public bool left;
    public bool start;
    public bool goal;
    public EntryPlace entry; 


    public enum EntryPlace
    {
        TOP,
        BOTTOM,
        RIGHT,
        LEFT
    }

    [SerializeField]
    public Piece(bool Top, bool Bottom, bool Right, bool Left)
    {    
        this.top = Top;
        this.bottom = Bottom;
        this.right = Right;
        this.left = Left;
    }

}
