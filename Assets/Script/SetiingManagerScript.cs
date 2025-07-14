using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetiingManagerScript : MonoBehaviour
{
    public static string SceneReturnTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnScene()
    {
        Initiate.Fade(SceneReturnTarget, Color.black, 0.5f);
    }
}
