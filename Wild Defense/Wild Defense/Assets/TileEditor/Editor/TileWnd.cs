using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileWnd : UnityEditor.EditorWindow //必须继承EditorWindow
{
    //tile脚本
    protected static TileObject tileObject;

    //添加菜单栏选项
    [MenuItem("Tools/Tile Window")]
    static void Create()
    {
        EditorWindow.GetWindow(typeof(TileWnd));
        //在场景中选中TileObject脚本实例
        if (Selection.activeTransform != null)
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
    }
    //当更新选中新物体
    void OnSelectionChange()
    {
        if (Selection.activeTransform != null)
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
    }

    //显示窗口UI，大部分UI函数都在GUILayout和EditorGUILayout内
    void OnGUI()
    {
        if (tileObject == null)
            return;
        //显示编辑器名称
        GUILayout.Label("Tile Editor");
        //在工程目录读取一张贴图
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GUI/butPlayer1.png");
        //将贴图显示在窗口内
        GUILayout.Label(tex);
        //是否显示TileObject帮助信息
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);
        //切换TileObject的数据
        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);
        EditorGUILayout.Separator();//分隔符
        if (GUILayout.Button("Reset"))//重置按钮
        {
            tileObject.Reset();//初始化
        }
    }
}
