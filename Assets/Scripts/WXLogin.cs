using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WXLogin : MonoBehaviour
{
    public Button button_login, button_quit;
    public Image image_head;
    public Text text_username, text_log;

    public GameObject go_lobby;

    private AndroidJavaClass jc = null;
    private AndroidJavaObject jo = null;

    private string APPID = "wx709390eb635c5a74";
    private string SECRET = "3a380be5f64df7cbe3f6ea4b925b51c1";


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WXLogin Start Begin");
        button_login.onClick.AddListener(Onbutton_login);
        button_quit.onClick.AddListener(() => { Application.Quit(); });

        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log("WXLogin Start End");
    }

    void Onbutton_login()
    {
        Debug.Log("Login Button Click!");
        jo.Call("Login");
    }



    public void WXLoginCallBack(string str)
    {
        if (str != "用户取消" && str != "用户拒绝" && str != "其他错误")
        {

            Debug.Log("微信登录成功，code是：" + str);
            text_log.text += "微信登录成功，code是：" + str + "\r\n";
            StartCoroutine(GetWXData(str));//获得微信数据
        }
        else
        {
            Debug.Log("微信登录失败，code是：" + str);
        }
    }
    public IEnumerator GetWXData(string code)
    {
        string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + APPID + "&secret=" + SECRET + "&code=" + code + "&grant_type=authorization_code";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isDone && request.error == null)
        {
            WXData wxdata = JsonUtility.FromJson<WXData>(request.downloadHandler.text);

            text_log.text += "wxdata:" + wxdata.access_token + "\r\n" + "wxdata.openid:" + wxdata.openid + "\r\n";

            //开始获得微信用户信息
            StartCoroutine(GetWXUserInfo(wxdata));
        }

    }

    public IEnumerator GetWXUserInfo(WXData wxdata)
    {
        if (wxdata != null)
        {
            string url_getuser = "https://api.weixin.qq.com/sns/userinfo?access_token=" + wxdata.access_token + "&openid=" + wxdata.openid;
            UnityWebRequest request = UnityWebRequest.Get(url_getuser);
            yield return request.SendWebRequest();
            if (request.isDone && request.error == null)
            {
                WXUserInfo wxuserinfo = JsonUtility.FromJson<WXUserInfo>(request.downloadHandler.text);

                text_username.text = wxuserinfo.nickname;
                text_log.text += "\r\n" + "姓名：" + wxuserinfo.nickname + "  性别：" + wxuserinfo.sex.ToString() + "  国家：" + wxuserinfo.country;
                go_lobby.SetActive(true);

                //开始获得用户头像
                StartCoroutine(GetHeadImage(wxuserinfo));
            }
        }


    }


    public IEnumerator GetHeadImage(WXUserInfo wxuserinfo)
    {
        if (wxuserinfo != null)
        {
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(wxuserinfo.headimgurl))
            {
                yield return req.SendWebRequest();
                if (req.isDone && req.error == null)
                {

                    Texture2D texture2d = (req.downloadHandler as DownloadHandlerTexture).texture;
                    Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    image_head.sprite = sprite;

                    go_lobby.SetActive(true);
                }
                else
                {
                    Debug.Log("下载出错" + req.responseCode + "," + req.error);
                    text_log.text += "\r\n" + "下载出错" + req.responseCode + "," + req.error;

                }
            }
        }

    }


}

public class WXData
{
    public string access_token;//接口调用凭证
    public string expires_in;//access_token 接口调用凭证超时时间，单位（秒）
    public string refresh_token;//用户刷新 access_token
    public string openid;//授权用户唯一标识
    public string scope;//用户授权的作用域，使用逗号（,）分隔
}

public class WXUserInfo
{
    public string openid;//用户的标识，对当前开发者帐号唯一
    public string nickname;//用户昵称
    public int sex;//用户性别，1 (0)为男性，2(1) 为女性,
    public string province;//用户个人资料填写的省份
    public string city;//用户个人资料填写的城市
    public string country;//国家，如中国为 CN
    public string headimgurl;//用户头像，最后一个数值代表正方形头像大小（有 0、46、64、96、132 数值可选，0 代表 640*640 正方形头像），用户没有头像时该项为空
    public string[] privilege;//用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
    public string unionid;//用户统一标识。针对一个微信开放平台帐号下的应用，同一用户的 unionid 是唯一的。
}
