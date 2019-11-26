using UnityEngine;

[RequireComponent(typeof(RectTransform))] //需要Rect Transform组件
public class ViewController : MonoBehaviour {
    //缓存RectTransform组件
    private RectTransform cachedRectTransform;
    public RectTransform CachedRectTransform
    {
        get
        {
            if (cachedRectTransform == null)
            {
                cachedRectTransform = GetComponent<RectTransform>();
            }
            return cachedRectTransform;
        }
    }
    //获取、设置视图名称的属性
    public virtual string Title { get { return ""; } set { } }
}
