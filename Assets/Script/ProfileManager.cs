using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public void ReturnHome()
    {
        Initiate.Fade("HomeScenes", Color.black, 0.5f);
    }

}
