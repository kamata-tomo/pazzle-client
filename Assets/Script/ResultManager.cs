using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoTohome()
    {
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

    public void GoToStageSelection()
    {
        Initiate.Fade("StageSelectionScene", Color.black, 0.5f);
    }

    public void ReturnGame()
    {
        Initiate.Fade("SlidePuzzleScene", Color.black, 0.5f);
    }
}
