using UnityEngine;
using UnityEditor;

#if UNITY_4_6
//在Unity5之前的版本中，AnimatorController类是在UnityEditorInternal命名空间内定义的
using UnityEditorInternal;
#else
using UnityEditor.Animations;
#endif

//用于输入创建动画剪辑名称的对话框的类
public class RenameWindow:EditorWindow
{
    public string CaptionText { get; set; } //对话框的标题
    public string ButtonText { get; set; } //按钮的标记
    public string NewName { get; set; } //被输入的名称
    public System.Action<string> OnClickButtonDelegate { get; set; } //单击按钮执行的委托
    private void OnGUI()
    {
        NewName = EditorGUILayout.TextField(CaptionText,NewName);
        if (GUILayout.Button(ButtonText))
        {
            if (OnClickButtonDelegate != null)
            {
                //单击按钮，将输入的名称传递给预先设置的委托
                OnClickButtonDelegate.Invoke(NewName.Trim());
            }
            Close();
            GUIUtility.ExitGUI();
        }
    }
}

public class NestedAnimationCreator : MonoBehaviour {
    //为Assets菜单中的"Create"添加"NestedAnimation"项目
    [MenuItem("Assets/Create/NestedAnimation")]
    public static void Create()
    {
        //获取在Project面板中选中的动画控制器
        AnimatorController selectedAnimatorController = Selection.activeObject as AnimatorController;
        //如果动画控制器没有被选中，即为错误
        if (selectedAnimatorController == null)
        {
            Debug.LogWarning("No animator controller selected.");
            return;
        }
        //打开输入新建动画剪辑名称的对话框
        RenameWindow renameWindow = EditorWindow.GetWindow<RenameWindow>("Create Nested Animation");
        renameWindow.CaptionText = "New Animation Name";
        renameWindow.NewName = "New Clip";
        renameWindow.ButtonText = "Create";
        //单击对话框按钮时，调用方法的委托
        renameWindow.OnClickButtonDelegate = (string newName) =>
        {
            if (string.IsNullOrEmpty(newName))
            {
                Debug.LogWarning("Invalid name.");
                return;
            }
            //用对话框中输入的名称,创建动画剪辑
            AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(newName);
            //作为所选中的动画控制器的子资源
            //添加已创建的动画剪辑
            AssetDatabase.AddObjectToAsset(animationClip, selectedAnimatorController);
            //重新导入动画控制器并反映出所做的修改
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(selectedAnimatorController));
        };
    }
    //由于作为子资源创建的动画剪辑,不能在Assets菜单→"Delete"中删除
    //所以，在Assets菜单中添加"DeleteSubAsset"项目
    [MenuItem("Assets/DeleteSubAsset")]
    public static void Delete()
    {
        Object[] selectedAssets = Selection.objects;
        if (selectedAssets.Length < 1)
        {
            Debug.LogWarning("No sub asset selected.");
        }

        foreach (Object asset in selectedAssets)
        {
            //若所选对象是子资源的话，则删除
            if (AssetDatabase.IsSubAsset(asset))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                DestroyImmediate(asset, true);
                AssetDatabase.ImportAsset(path);
            }
        }
    }
}