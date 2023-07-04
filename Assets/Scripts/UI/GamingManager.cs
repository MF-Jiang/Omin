using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamingManager : MonoBehaviour
{
    public static GamingManager sharedInstance = null;
    public GameObject winPanel; //ʤ������
    public GameObject losePanel; //ʧ�ܽ���
    public GameObject TimeOutPanel; //��ͣ����
    public GameObject TimeOutBtn; //��ͣ��ť
    public GameObject SetPanel;
    private Scene scene;
    private bool inTimeOut = false;


    private AudioSource aSource;
    public AudioClip winAudio; //ʤ������
    public AudioClip loseAudio; //ʧ������

    private void Awake() //��ʼ�����������
    {
        //PlayerPrefs.DeleteAll();//������ɾ���ؿ������¼

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
        if (Input.GetKeyDown(KeyCode.Escape) && inTimeOut == false) //��ͣ��Ϸ
        {
            TimeOutGame();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inTimeOut == true) //������Ϸ
        {
            ContinueTimeGame();
            return;
        }

        if (scene.name == "Level1") //���浽��һ�أ�����Select�еĵ�һ��
        {
            PlayerPrefs.SetInt("Level1key", 1);

        }
    }

    public void GameOver(bool playerWin) //��Ϸ����
    {
        StartCoroutine(DelayGameOver(playerWin));
    }

    IEnumerator DelayGameOver(bool playerWin) //�ж�ʤ��
    {
        yield return new WaitForSeconds(0.5f);
        if (playerWin)
        {
            if (winAudio != null)
            {
                aSource.clip = winAudio;
                aSource.Play();
            }
            winPanel.SetActive(true); //չʾʤ������

        }
        else
        {
            if (loseAudio != null)
            {
                aSource.clip = loseAudio;
                aSource.Play();
            }
            losePanel.SetActive(true); //չʾʧ�ܽ���
        }
        Time.timeScale = 0;

    }

    public void LoadNextLevel(int index) //����ؿ�
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void QuitGame() //�˳���Ϸ
    {
        Application.Quit();
    }

    public void TimeOutGame() //��ͣ��Ϸ
    {

        inTimeOut = true;

        TimeOutPanel.SetActive(true);

        TimeOutBtn.SetActive(false);

        Time.timeScale = 0;


    }

    public void ContinueTimeGame() //������Ϸ
    {

        inTimeOut = false;
        TimeOutPanel.SetActive(false);
        TimeOutBtn.SetActive(true);
        Time.timeScale = 1;

    }

    public void OpenSetPanel() { //TimeOut���Set
        SetPanel.SetActive(true);

    }

    public void CloseSetPanel() { //Timeout��ر�set
        SetPanel.SetActive(false);

    }


}
