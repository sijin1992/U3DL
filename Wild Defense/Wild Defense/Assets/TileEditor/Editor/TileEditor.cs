using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor {
    //是否处于编辑模式
    protected bool editMode = false;
    //受编辑器影响的tile脚本
    protected TileObject tileObject;

    private void OnEnable()
    {
        //获得tile脚本
        tileObject = (TileObject)target;
    }

    //更改场景中的操作
    public void OnSceneGUI()
    {
        if (editMode)//如果在编辑模式
        {
            //取消编辑器的选择功能
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            //在编辑器中显示数据(画出辅助线)
            tileObject.debug = true;
            //获取Input事件
            Event e = Event.current;

            //如果是鼠标左键
            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
            {
                //获取由鼠标位置产生的射线
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                //计算碰撞
                RaycastHit hitinfo;
                if (Physics.Raycast(ray,out hitinfo,2000,tileObject.tileLayer))
                {
                    tileObject.setDataFromPosition(hitinfo.point.x,hitinfo.point.z,tileObject.dataID);
                }
            }
        }
        HandleUtility.Repaint();
    }
    //自定义Inspector窗口的UI
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tile Editor");//显示编辑器名称
        editMode = EditorGUILayout.Toggle("Edit",editMode);//是否启用编辑模式
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);//是否显示帮助信息

        string[] editDataStr = { "Dead","Road","Guard"};
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID,editDataStr);

        EditorGUILayout.Separator();//分隔符
        if (GUILayout.Button("Reset")) //重置按钮
        {
            tileObject.Reset();//初始化
        }
        DrawDefaultInspector();
    }
}
