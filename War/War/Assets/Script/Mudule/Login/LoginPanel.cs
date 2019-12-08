using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //初始化
    public override void OnInit()
    {
        skinPath = "Prefabs/Panels/LoginPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {

    }
    //关闭
    public override void OnClose()
    {

    }
}
