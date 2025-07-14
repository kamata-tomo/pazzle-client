using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToHomeScene()
    {
        // ランキングシーンにフェード
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

}
