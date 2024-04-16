using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Google play manager.
/// create by liufeng on 18-01-11
/// </summary>
public static class GooglePlayManager
{

    public delegate void GPDelegate(bool success, string uname);

    static GPDelegate authenticatingCallback = null;

    /// <summary>
    /// ��ʼ��SDK
    /// </summary>
    public static void Init()
    {
        PlayGamesPlatform.Activate();
    }

    /// <summary>
    /// �Ƿ��¼
    /// </summary>
    /// <returns>The authenticated.</returns>
    public static bool Authenticated()
    {
        return Social.localUser.authenticated;
    }

    /// <summary>
    /// ��¼
    /// </summary>
    /// <returns>The authenticating.</returns>
    /// <param name="cb">�ص�</param>
    public static void Authenticating(GPDelegate cb = null)
    {
        Debug.Log("Google Play Authenticating");
        authenticatingCallback = cb;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Authentication success : " + Social.localUser.userName);
            }
            else
            {
                Debug.Log("Authentication failed");
            }
            if (cb != null) cb(success, Social.localUser.userName);
        });
    }

    /// <summary>
    /// ע��
    /// </summary>
    public static void SignOut()
    {
        //((PlayGamesPlatform)Social.Active).SignOut();
    }


    /// <summary>
    /// �ϴ�����
    /// </summary>
    /// <param name="scores">����.</param>
    /// <param name="lbid">���а�id.</param>
    public static void PostScore(int scores, string lbid)
    {
        if (!Authenticated())
        {
            Debug.Log("û�е�¼");
            return;
        }

        Social.ReportScore(scores, lbid, (bool success) => {
            // handle success or failure
            Debug.Log("post score : " + success);
        });

    }

    /// <summary>
    /// ��ʾ���а�
    /// </summary>
    /// <param name="lbid">���а�id�������ַ�������ʾȫ�����а�</param>
    public static void ShowLeaderboard(string lbid = "")
    {
        if (!Authenticated())
        {
            Debug.Log("û�е�¼");
            return;
        }
        if (lbid == "")
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(lbid);
        }

    }
}