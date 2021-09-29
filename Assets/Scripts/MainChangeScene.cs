using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainChangeScene : MonoBehaviour
{
    public void ChangeSceneMain()
    {
        switch (this.gameObject.name)
        {
            case "Stage1Btn":
                SceneManager.LoadScene("Stage1");
                break;
            case "Stage2Btn":
                SceneManager.LoadScene("Stage2");
                break;
            case "Stage3Btn":
                SceneManager.LoadScene("Stage3");
                break;
            case "TrainingBtn":
                SceneManager.LoadScene("Training");
                break;
            case "BackBtn":
                SceneManager.LoadScene("Start");
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
