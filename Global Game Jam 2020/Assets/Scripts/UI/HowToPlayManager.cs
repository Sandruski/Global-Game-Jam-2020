﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayManager : MonoBehaviour
{
 
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}