using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //账号输入框
    private InputField idInput;
    //密码输入框
    private InputField pwInput;
    //登录按钮
    private Button loginBtn;
    //注册按钮
    private Button regBtn;
    //初始化
    public override void OnInit()
    {
        skinPath = "Prefabs/Panels/LoginPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        idInput = skin.transform.Find("IdInput").GetComponent<InputField>();
        pwInput = skin.transform.Find("PwInput").GetComponent<InputField>();
        loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        regBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        //监听
        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }
    //关闭
    public override void OnClose()
    {

    }
    //当按下登录按钮
    public void OnLoginClick()
    {

    }
    //当按下注册按钮
    public void OnRegClick()
    {

    }
}
