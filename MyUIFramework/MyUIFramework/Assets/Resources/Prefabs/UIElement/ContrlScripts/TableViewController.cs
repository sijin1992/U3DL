using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class TableViewController<T> : ViewController    //继承ViewController类
{
    protected List<T> tableData = new List<T>();    //保持列表项目的数据
    [SerializeField] private RectOffset padding;    //填充滚动内容
    [SerializeField] private float spacingHeight = 4.0f;    //各个cell的间隔

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

    //在示例加载时调用
    protected virtual void Awake()
    {
    }

    //返回列表项目对应的cell高度的方法
    protected virtual float CellHeightAtIndex(int index)
    {
        //通过继承类实现返回实际值的处理
        return 0.0f;
    }

    //更新滚动内容整体高度的方法
    protected void UpdateContentSize()
    {
        //计算出滚动内容的整体高度
        float contentHeight = 0.0f;
        for (int i = 0; i < tableData.Count; i++)
        {
            contentHeight += CellHeightAtIndex(i);
            if (i > 0)
            {
                contentHeight += spacingHeight;
            }
        }

        //设置滚动内容的高度
        Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
        sizeDelta.y = padding.top + contentHeight + padding.bottom;
        CachedScrollRect.content.sizeDelta = sizeDelta;
    }

//*****实现创建Cell的方法和更新Cell内容的方法*****

    [SerializeField] private GameObject cellBase;   //复制的基础cell
    private LinkedList<TableViewCell<T>> cells = new LinkedList<TableViewCell<T>>();    //保持cell

    //加载实例时调用Awake方法之后调用
    protected virtual void Start()
    {
        //使复制的基础cell呈非活动状态
        cellBase.SetActive(false);

        //设置ScrollRect组件的OnValueChanged事件的事件监听器
        CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
    }

    //创建cell的方法
    private TableViewCell<T> CreateCellForIndex(int index)
    {
        //由复制的基础cell创建新cell
        GameObject obj = Instantiate(cellBase) as GameObject;
        obj.SetActive(true);
        TableViewCell<T> cell = obj.GetComponent<TableViewCell<T>>();

        //因为如果替换父元素的话,就会失去比例和尺寸,所以需要先保持变量
        Vector3 scale = cell.transform.localScale;
        Vector2 sizeDelta = cell.CachedRectTransform.sizeDelta;
        Vector2 offsetMin = cell.CachedRectTransform.offsetMin;
        Vector2 offsetMax = cell.CachedRectTransform.offsetMax;

        cell.transform.SetParent(cellBase.transform.parent);

        //设置cell的比例和尺寸
        cell.transform.localScale = scale;
        cell.CachedRectTransform.sizeDelta = sizeDelta;
        cell.CachedRectTransform.offsetMin = offsetMin;
        cell.CachedRectTransform.offsetMax = offsetMax;

        //作为制定索引的列表项目相对应的cell,更新内容
        UpdateCellForIndex(cell, index);

        cells.AddLast(cell);

        return cell;
    }

    //更新cell内容的方法
    private void UpdateCellForIndex(TableViewCell<T> cell,int index)
    {
        //设置与cell相对应的列表项目索引
        cell.DataIndex = index;

        if (cell.DataIndex >= 0 && cell.DataIndex <= tableData.Count - 1)
        {
            //如果有与cell相对应的列表项目，则令cell为活动状态,更新内容，设置高度
            cell.gameObject.SetActive(true);
            cell.UpdateContent(tableData[cell.DataIndex]);
            cell.Height = CellHeightAtIndex(cell.DataIndex);
        }
        else
        {
            //如果没有与cell相对应的列表项目,则cell为非活动状态,不显示
            cell.gameObject.SetActive(false);
        }
    }

//*****实现visibleRect的定义和更新visibleRect的方法*****

    private Rect visibleRect;               //用矩形表示以cell的形式显示列表项目的范围
    [SerializeField] private RectOffset visibleRectPadding; //visibleRect的属性

    //用于更新visibleRect的方法
    private void UpdateVisibleRect()
    {
        //visibleRect的位置是距离滚动内容的基准点的相对位置
        visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
        visibleRect.y = - CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

        //visibleRect的尺寸是滚动视图的尺寸+填充内容的尺寸
        visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
        visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
    }

//*****实现更新TableView显示内容的处理*****

    protected void UpdateContents()
    {
        UpdateContentSize();        //更新滚动内容的尺寸
        UpdateVisibleRect();        //更新visibleRect

        if (cells.Count < 1)
        {
            //如果一个cell都没有的话，就搜索最先进入visibleRect范围内的列表项目
            //创建相应的cell
            Vector2 cellTop = new Vector2(0.0f, -padding.top);
            for (int i = 0; i < tableData.Count; i++)
            {
                float cellHeight = CellHeightAtIndex(i);
                Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                if ((cellTop.y <= visibleRect.y && cellTop.y >= visibleRect.y - visibleRect.height)
                 || (cellBottom.y <= visibleRect.y && cellBottom.y >= visibleRect.y - visibleRect.height))
                {
                    TableViewCell<T> cell = CreateCellForIndex(i);
                    cell.Top = cellTop;
                    break;
                }
                cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
            }

            //如果visibleRect范围内为空的话，则创建cell
            FillVisibleRectWithCells();
        }
        else
        {
            //如果已经有cell的话，从最开始的cell依次设置对应列表项目的索引并修改更新位置和内容
            LinkedListNode<TableViewCell<T>> node = cells.First;
            UpdateCellForIndex(node.Value, node.Value.DataIndex);
            node = node.Next;

            while (node != null)
            {
                UpdateCellForIndex(node.Value, node.Previous.Value.DataIndex + 1);
                node.Value.Top = node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                node = node.Next;
            }

            //如果visibleRect的范围为空的话，则创建cell
            FillVisibleRectWithCells();
        }
    }

    //创建visibleRect范围内可显示的数量的cell的方法
    private void FillVisibleRectWithCells()
    {
        //如果没有cell,则不采取任何操作
        if (cells.Count < 1)
        {
            return;
        }

        //如果显示的最后cell相应的列表项目之后有列表项目
        //并且，该cell进入到visibleRect范围内的话,则创建相应的cell
        TableViewCell<T> lastCell = cells.Last.Value;
        int nextCellDataIndex = lastCell.DataIndex + 1;
        Vector2 nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

        while (nextCellDataIndex < tableData.Count && nextCellTop.y >= visibleRect.y - visibleRect.height)
        {
            TableViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
            cell.Top = nextCellTop;

            lastCell = cell;
            nextCellDataIndex = lastCell.DataIndex + 1;
            nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
        }
    }

//*****实现再次利用cell的处理*****

    private Vector2 prevScrollPos;

    //当滚动滚动视图时调用
    public void OnScrollPosChanged(Vector2 scrollPos)
    {
        //更新visibleRect
        UpdateVisibleRect();
        //根据滚动的方向，再次利用cell,更新显示
        ReuseCells((scrollPos.y < prevScrollPos.y) ? 1 : -1);

        prevScrollPos = scrollPos;
    }

    //再次利用cell,更新显示的方法
    private void ReuseCells(int scrollDirection)
    {
        if (cells.Count < 1)
        {
            return;
        }

        if (scrollDirection > 0)
        {
            //向上滚动时,令超出visibleRect范围之上的cell依次向下移动，更新内容
            TableViewCell<T> firstCell = cells.First.Value;
            while (firstCell.Bottom.y > visibleRect.y)
            {
                TableViewCell<T> lastCell = cells.Last.Value;
                UpdateCellForIndex(firstCell, lastCell.DataIndex + 1);
                firstCell.Top = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

                cells.AddLast(firstCell);
                cells.RemoveFirst();
                firstCell = cells.First.Value;
            }
            //如果visibleRect的范围内为空，则创建cell
            FillVisibleRectWithCells();
        }
        else if (scrollDirection < 0)
        {
            //向下滚动时，令超出visibleRect范围之下的cell依次向上移动，更新内容
            TableViewCell<T> lastCell = cells.Last.Value;
            while (lastCell.Top.y < visibleRect.y - visibleRect.height)
            {
                TableViewCell<T> firstCell = cells.First.Value;
                UpdateCellForIndex(lastCell, firstCell.DataIndex - 1);
                lastCell.Bottom = firstCell.Top + new Vector2(0.0f, spacingHeight);

                cells.AddFirst(lastCell);
                cells.RemoveLast();
                lastCell = cells.Last.Value;
            }
        }
    }
}