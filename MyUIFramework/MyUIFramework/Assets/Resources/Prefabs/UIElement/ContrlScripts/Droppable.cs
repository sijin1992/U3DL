using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour ,IDropHandler,IPointerEnterHandler,IPointerExitHandler
//实现控制释放操作的接口
{
    //释放区域中所显示的图标
    [SerializeField] private Image iconImage;
    //释放区域中所显示图标的高亮颜色
    [SerializeField] private Color highlightedColor;
    //保持释放区域中所显示图标的原本颜色
    private Color normalColor;

    //加载实例时，在Awake方法之后调用
    void Start()
    {
        //保持释放区域中所显示图标的原本颜色
        normalColor = iconImage.color;
    }

    //当光标进入区域时调用
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            //如果在拖曳过程中的话，释放区域中所显示的图标颜色变更为高亮颜色
            iconImage.color = highlightedColor;
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //如果时拖曳过程中的话，释放区域中显示的图标颜色还原为原本颜色
        if (pointerEventData.dragging)
        {
            iconImage.color = normalColor;
        }
    }

    public void OnDrop(PointerEventData pointerEventData)
    {
        //获取已拖曳的图标的Image组件
        Image droppedImage = pointerEventData.pointerDrag.GetComponent<Image>();
        //将释放区域中已显示的图标的精灵变为与被释放图标相同的精灵，颜色还原
        iconImage.sprite = droppedImage.sprite;
        iconImage.color = normalColor;
    }
}
