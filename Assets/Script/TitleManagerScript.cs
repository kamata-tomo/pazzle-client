using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleManagerScript : MonoBehaviour
{
    [SerializeField] InputField inputField;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToHomeScene()
    {
        // �����L���O�V�[���Ƀt�F�[�h
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

}
