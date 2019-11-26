using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class PagingScrollViewController : ViewController,IBeginDragHandler,IEndDragHandler //继承ViewController类，实现IBeginDragHandler和IEndDragHandler接口
{
    //缓存ScrollRect组件
    private ScrollRect cachedScrollRect;
    public ScrollRect CachedScrollRect
    {
        get
        {
            if (cachedScrollRect == null)
            {
                cachedScrollRect = GetComponent<ScrollRect>();
            }
            return cachedScrollRect;
        }
    }

    private bool isAnimating = false;   //动画中的FLAG
    private Vector2 destPosition;       //最终滚动位置
    private Vector2 initialPosition;    //开始自动滚动时的滚动位置
    private AnimationCurve animationCurve;  //自动滚动的动画曲线
    private int prevPageIndex = 0;      //保持前页索引
    //优化属性
    private Rect currentViewRect;//保持滚动视图的矩形
    //[SerializeField] private PageControl pageControl;//建立关联的页面控制

    //开始拖拽时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        //重置动画中的FLAG
        isAnimating = false;
    }

    //结束拖拽时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();

        //停止滚动视图当前的动作
        CachedScrollRect.StopMovement();

        //根据GridLayoutGroup的cellSize和spacing计算出一页的宽度
        float pageWidth = -(grid.cellSize.x + grid.spacing.x);

        //由当前滚动位置，计算出符合的页索引
        int pageIndex = Mathf.RoundToInt((CachedScrollRect.content.anchoredPosition.x) / pageWidth);
        if(pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 4)
        {
            //当以超过一定速度拖拽时，沿着该方向前进一页
            CachedScrollRect.content.anchoredPosition += new Vector2(eventData.delta.x, 0.0f);
            pageIndex += (int)Mathf.Sign(-eventData.delta.x);
        }

        //如果在起始页或最末页,不能继续滚动
        if (pageIndex < 0 )
        {
            pageIndex = 0;
        }
        else if (pageIndex > grid.transform.childCount - 1)
        {
            pageIndex = grid.transform.childCount - 1;
        }

        prevPageIndex = pageIndex;//保持当前页索引

        //计算最终滚动位置
        float destX = pageIndex * pageWidth;
        destPosition = new Vector2(destX, CachedScrollRect.content.anchoredPosition.y);

        //保持开始时的滚动位置
        initialPosition = CachedScrollRect.content.anchoredPosition;

        //创建以0.3秒速度流程变化的动画曲线
        Keyframe keyFrame1 = new Keyframe(Time.time, 0.0f, 0.0f, 1.0f);
        Keyframe keyFrame2 = new Keyframe(Time.time + 0.3f, 1.0f, 0.0f, 0.0f);
        animationCurve = new AnimationCurve(keyFrame1, keyFrame2);

        //设置动画中的FLAG
        isAnimating = true;

        //更新页数的显示
        //pageControl.SetCurrentPage(pageIndex);
    }

    //每帧Update方法之后调用
    void LateUpdate()
    {
        if (isAnimating)
        {
            if (Time.time >= animationCurve.keys[animationCurve.length - 1].time)
            {
                //当超过动画曲线最后关键帧时，停止动画
                CachedScrollRect.content.anchoredPosition = destPosition;
                isAnimating = false;
                return;
            }

            //由动画曲线计算出当前滚动位置,移动滚动视图
            Vector2 newPosition = initialPosition + (destPosition - initialPosition) * animationCurve.Evaluate(Time.time);
            cachedScrollRect.content.anchoredPosition = newPosition;
        }
    }

    //优化
    //加载实例时，在Awake方法之后调用
    void Start()
    {
        //"ScrollContent"的Padding初始化
        UpdateView();

        //页数
        //pageControl.SetNumberOfPages(5);    //将页数设置为5
        //pageControl.SetCurrentPage(0);      //初始化页面控制的显示
    }

    //每帧调用
    void Update()
    {
        if (CachedRectTransform.rect.width != currentViewRect.width || CachedRectTransform.rect.height != currentViewRect.height)
        {
            //当滚动视图的宽度或高度变化时，更新“ScrollContent”的Padding
            UpdateView();
        }
    }

    //"ScrollContent"的Padding的更新方法
    private void UpdateView()
    {
        //保持滚动视图的矩形
        currentViewRect = CachedRectTransform.rect;

        //由GridLayoutGroup的cellSize计算出“ScrollContent”的Padding并设置
        GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();
        int paddingH = Mathf.RoundToInt((currentViewRect.width - grid.cellSize.x) / 2.0f);
        int paddingV = Mathf.RoundToInt((currentViewRect.height - grid.cellSize.y) / 2.0f);
        grid.padding = new RectOffset(paddingH, paddingH, paddingV, paddingV);
    }
}