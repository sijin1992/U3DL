using UnityEngine;

//控制由iTween触发事件的类
public class iTweenEventHandler: MonoBehaviour
{
    //动画每移动一步都调用的回调函数方法
    public System.Action<Vector2> OnUpdateMoveDelegate { get; set; }

    //动画每移动一步都调用的方法
    public void OnUpdateMove(Vector2 value)
    {
        if (OnUpdateMoveDelegate != null)
        {
            OnUpdateMoveDelegate.Invoke(value);
        }
    }

    //终止动画时调用的回调函数方法
    public System.Action OnCompleteDelegate { get; set; }
    
    //动画终止后调用的方法
    public void OnComplete()
    {
        if (OnCompleteDelegate != null)
        {
            OnCompleteDelegate.Invoke();
        }
    }
}

//用于扩展方法的静态类
public static class iTweenUIExtensions {
    //设置事件处理程序的方法，控制由iTween触发的事件
    private static iTweenEventHandler SetUpEventHandler(GameObject targetObj)
    {
        iTweenEventHandler eventHandler = targetObj.GetComponent<iTweenEventHandler>();
        if (eventHandler == null)
        {
            eventHandler = targetObj.AddComponent<iTweenEventHandler>();
        }
        return eventHandler;
    }

    //RectTransform.MoveTo
    //将RectTransform由当前位置移动到指定的动画
    public static void MoveTo(this RectTransform target,Vector2 pos,float time,float delay,iTween.EaseType easeType,System.Action onCompleteDelegate = null)
    {
        //设置事件处理程序，控制由iTween触发的事件
        iTweenEventHandler eventHandler = SetUpEventHandler(target.gameObject);

        //对移动动画时调用的回调函数方法进行设置
        eventHandler.OnUpdateMoveDelegate = (Vector2 value) =>
        {
            //更新RectTransform的位置
            target.anchoredPosition = value;
        };

        //对结束动画时调用的回调函数方法进行设置
        eventHandler.OnCompleteDelegate = onCompleteDelegate;

        //调用iTween的ValueTo方法，开始动画
        iTween.ValueTo(target.gameObject, iTween.Hash(
            "from",target.anchoredPosition,
            "to",pos,
            "time",time,
            "delay",delay,
            "easetype",easeType,
            "onupdate","OnUpdateMove",
            "onupdatetarget",eventHandler.gameObject,
            "oncomplete","OnComplete",
            "oncompletetarget",eventHandler.gameObject
        ));
    }
}