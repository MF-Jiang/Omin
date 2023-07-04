using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamingManager : MonoBehaviour
{
    public static GamingManager sharedInstance = null;
    public GameObject winPanel; //胜利界面
    public GameObject losePanel; //失败界面
    public GameObject TimeOutPanel; //暂停界面
    public GameObject TimeOutBtn; //暂停按钮
    public GameObject SetPanel;
    private Scene scene;
    private bool inTimeOut = false;


    private AudioSource aSource;
    public AudioClip winAudio; //胜利音乐
    public AudioClip loseAudio; //失败音乐

    private void Awake() //初始化这个管理类
    {
        //PlayerPrefs.DeleteAll();//测试用删除关卡保存记录

        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sharedInstance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        scene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inTimeOut == false) //暂停游戏
        {
            TimeOutGame();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inTimeOut == true) //继续游戏
        {
            ContinueTimeGame();
            return;
        }

        if (scene.name == "Level1") //游玩到第一关，解锁Select中的第一关
        {
            PlayerPrefs.SetInt("Level1key", 1);

        }
    }

    public void GameOver(bool playerWin) //游戏结束
    {
        StartCoroutine(DelayGameOver(playerWin));
    }

    IEnumerator DelayGameOver(bool playerWin) //判断胜负
    {
        yield return new WaitForSeconds(0.5f);
        if (playerWin)
        {
            if (winAudio != null)
            {
                aSource.clip = winAudio;
                aSource.Play();
            }
            winPanel.SetActive(true); //展示胜利界面

        }
        else
        {
            if (loseAudio != null)
            {
                aSource.clip = loseAudio;
                aSource.Play();
            }
            losePanel.SetActive(true); //展示失败界面
        }
        Time.timeScale = 0;

    }

    public void LoadNextLevel(int index) //载入关卡
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void QuitGame() //退出游戏
    {
        Application.Quit();
    }

    public void TimeOutGame() //暂停游戏
    {

        inTimeOut = true;

        TimeOutPanel.SetActive(true);

        TimeOutBtn.SetActive(false);

        Time.timeScale = 0;


    }

    public void ContinueTimeGame() //继续游戏
    {

        inTimeOut = false;
        TimeOutPanel.SetActive(false);
        TimeOutBtn.SetActive(true);
        Time.timeScale = 1;

    }

    public void OpenSetPanel() { //TimeOut后打开Set
        SetPanel.SetActive(true);

    }

    public void CloseSetPanel() { //Timeout后关闭set
        SetPanel.SetActive(false);

    }


}
