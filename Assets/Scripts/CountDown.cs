using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{
    public GameObject finishMenu;
    public GameObject restartMenu;

    public int countDownTime;
    public Text countDownDisplay;
    public Text curTimeText;
    public Text curSpeedText;
    public Text bestLapTimeText;
    public Text[] lapTimeText;
    public int lap;

    public Transform cam;

    public bool check;
    public bool checkRestart;

    float curTime;
    float bestLapTime;

    private void Start()
    {
        StartCoroutine(CountDownToStart());
        check = true;
        BestLapTimeSet();

        finishMenu.SetActive(false);
        restartMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (checkRestart == false)
            {
                checkRestart = true;
                restartMenu.SetActive(true);
            }
            else if(checkRestart == true)
            {
                checkRestart = false;
                restartMenu.SetActive(false);
            }
        }
    }



    void BestLapTimeSet()
    {
        bestLapTime = PlayerPrefs.GetFloat("BestLap");
        bestLapTimeText.text = string.Format("Best{0:00}:{1:00.00}",
                   (int)(bestLapTime / 60 % 60), bestLapTime % 60);

        if (bestLapTime == 0)
        {
            bestLapTimeText.text = "Best    -";
        }
    }

    public void LapTime()
    {
        if (lap == 3)
        {
            cam.parent = null;
            StopCoroutine(Timer());
            finishMenu.SetActive(true);

            if (curTime < bestLapTime | bestLapTime == 0)
            {
                bestLapTimeText.gameObject.SetActive(false);
                bestLapTimeText.text = string.Format("Best{0:00}:{1:00.00}", 
                    (int)(curTime / 60 % 60), curTime % 60);
                bestLapTimeText.gameObject.SetActive(true);

                PlayerPrefs.SetFloat("BestLap", curTime);
            }
        }

        lapTimeText[lap-1].gameObject.SetActive(false);
        lapTimeText[lap-1].text = string.Format("{0:00}:{1:00.00}",
            (int)(curTime / 60 % 60), curTime % 60);
        lapTimeText[lap-1].gameObject.SetActive(true);
    }


    IEnumerator CountDownToStart()
    {
        countDownDisplay.gameObject.SetActive(true);
        while (countDownTime > 0)
        {
            countDownDisplay.text = countDownTime.ToString();

            yield return new WaitForSeconds(1f);

            countDownTime--;
        }

        countDownDisplay.text = "GO";

        yield return new WaitForSeconds(1f);

        countDownDisplay.gameObject.SetActive(false);

        StartCoroutine(Timer());

    }

    IEnumerator Timer()
    {
        while (true)
        {
            curTime += Time.deltaTime;

            curTimeText.text = string.Format("{0:00}:{1:00.00}", (int)(curTime / 60 % 60),curTime%60);
            yield return null;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Start");
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Main");
    }
   
}
