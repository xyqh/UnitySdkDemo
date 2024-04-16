using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooglePlayTest : MonoBehaviour
{

    const int height = 70;
    const int width = 200;

    // Use this for initialization
    void Start()
    {
        GooglePlayManager.Init();
    }

    int high;
    void OnGUI()
    {
        high = 10;

        if (CreateBtn("��¼"))
        {
            GooglePlayManager.Authenticating(AuthenticatingCallBack);
        }

        if (CreateBtn("ע��"))
        {
            GooglePlayManager.SignOut();
        }

        if (CreateBtn("�ϴ�����"))
        {
            GooglePlayManager.PostScore(123, "CgkImoOim_EUEAIQAQ");
        }

        if (CreateBtn("��ʾ���а�"))
        {
            GooglePlayManager.ShowLeaderboard("CgkImoOim_EUEAIQAQ");
        }
    }


    public bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(Screen.width / 2 - width / 2, high, width, height), btnname);
        high += height + 5;
        return b;
    }

    public void AuthenticatingCallBack(bool success, string uname)
    {
        if (success)
        {
            Debug.Log("Authenticating success : " + uname);
            Debug.Log("Authenticating success : " + uname);
        }
        else
        {
            Debug.Log("Authenticating failed  ");
            Debug.Log("Authenticating failed  ");
        }

    }

}