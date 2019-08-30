using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
#if UNITY_4_6
//Unity 4.6.x中AnimatorController类是在UnityEditorInternal命名空间中定义的
using UnityEditorInternal;
#else
using UnityEditor.Animations;
#endif
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SpriteAnimationCreator : MonoBehaviour {
    //定义默认的帧间隔
    private static float defaultInterval = 0.1f;
    //为Asset菜单->"Create"追加"Sprite Animation"项目
    [MenuItem("Assets/Create/Sprite Animation")]
    public static void Create()
    {
        //获取Project菜单选中的精灵
        List<Sprite> selectedSprites = new List<Sprite>(Selection.GetFiltered(typeof(Sprite), SelectionMode.DeepAssets).OfType<Sprite>());
        //如果选中了纹理,则获取其中的精灵
        Object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        foreach (Object texture in selectedTextures)
        {
            selectedSprites.AddRange(AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture)).OfType<Sprite>());
        }

        //如果没有选中精灵则报错
        if (selectedSprites.Count < 1)
        {
            Debug.LogWarning("No sprite selected");
            return;
        }

        //按照精灵末尾的连续编号进行排序
        string suffixPattern = "_?([0-9]+)$";
        selectedSprites.Sort((Sprite _1, Sprite _2) =>
        {
            Match match1 = Regex.Match(_1.name, suffixPattern);
            Match match2 = Regex.Match(_2.name, suffixPattern);
            if (match1.Success && match2.Success)
            {
                return (int.Parse(match1.Groups[1].Value) - int.Parse(match2.Groups[1].Value));
            }
            else
            {
                return _1.name.CompareTo(_2.name);
            }
        });

        //后续将资源保存在第一个精灵所在的文件夹
        string baseDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedSprites[0]));
        //将第一个精灵的不含连续编号的名称设置为动画的名称
        string baseName = Regex.Replace(selectedSprites[0].name, suffixPattern, "");
        if (string.IsNullOrEmpty(baseName))
        {
            baseName = selectedSprites[0].name;
        }

        //没有画布的话创建一个画布
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.layer = LayerMask.NameToLayer("UI");
        }

        //创建图像
        GameObject obj = new GameObject(baseName);
        obj.transform.parent = canvas.transform;
        obj.transform.localPosition = Vector3.zero;

        Image image = obj.AddComponent<Image>();
        image.sprite = (Sprite)selectedSprites[0];
        image.SetNativeSize();

        //附加Animator组件
        Animator animator = obj.AddComponent<Animator>();

        //创建动画剪辑
        AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(baseName);

#if UNITY_4_6
        //Unity 4.6.x中将动画类型设置为ModelImpoterAnimationType.Generic
        AnimationUtility.SetAnimationType(animationClip,ModelmporterAnimationType.Generic);
#endif
        //使用EditorCurveBinding,将关键帧和图像的Sprite属性进行关联
        EditorCurveBinding editorCurveBinding = new EditorCurveBinding();
        editorCurveBinding.type = typeof(Image);
        editorCurveBinding.path = "";
        editorCurveBinding.propertyName = "m_Sprite";

        //创建关键帧，数量为所选中的精灵数量，为各关键帧分配精灵
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[selectedSprites.Count];
        for (int i = 0; i < selectedSprites.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = i * defaultInterval;
            keyFrames[i].value = selectedSprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(animationClip,editorCurveBinding,keyFrames);

        //由于Loop Time属性无法直接从脚本中进行设置，因此使用SerializedProperty进行设置（该方法在Unity将来的版本中可能无法使用）
        SerializedObject serializedAnimationClip = new SerializedObject(animationClip);
        SerializedProperty serializedAnimationClipSettings = serializedAnimationClip.FindProperty("m_AnimationClipSettings");
        serializedAnimationClipSettings.FindPropertyRelative("m_LoopTime").boolValue = true;
        serializedAnimationClip.ApplyModifiedProperties();

        //将创建的动画剪辑作为资源保存
        SaveAsset(animationClip,baseDir + "/" + baseName + ".anim");

        //创建动画控制器
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPathWithClip(baseDir + "/" + baseName + ".controller", animationClip);
        animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorController;
    }

    private static void SaveAsset(Object obj,string path)
    {
        Object existingAsset = AssetDatabase.LoadMainAssetAtPath(path);
        if (existingAsset != null)
        {
            EditorUtility.CopySerialized(obj, existingAsset);
            AssetDatabase.SaveAssets();
        }else
        {
            AssetDatabase.CreateAsset(obj, path);
        }
    }
}
