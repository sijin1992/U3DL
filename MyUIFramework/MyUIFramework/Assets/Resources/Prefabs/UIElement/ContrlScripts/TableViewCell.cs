using UnityEngine;

public class TableViewCell<T> : ViewController//继承ViewController类
{
    //更新cell内容的方法
    public virtual void UpdateContent(T itemData)
    {
        //实际处理运用继承类来实现
    }

    //保持cell对应的列表项目索引
    public int DataIndex { get; set; }

    //获取并设置cell高度的属性
    public float Height
    {
        get { return CachedRectTransform.sizeDelta.y; }
        set
        {
            Vector2 sizeDelta = CachedRectTransform.sizeDelta;
            sizeDelta.y = value;
            CachedRectTransform.sizeDelta = sizeDelta;
        }
    }

    //获取并设置cell上端位置的属性
    public Vector2 Top
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);//1.获取顶点坐标数组,返回结果的顶点索引为:左上角：1  右上角：2  左下角：0  右下角：3
            return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
        }
    }

    //获取并设置cell下端位置的属性
    public Vector2 Bottom
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
        }
    }
}
