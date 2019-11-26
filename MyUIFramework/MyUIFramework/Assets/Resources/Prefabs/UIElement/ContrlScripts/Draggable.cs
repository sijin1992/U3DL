using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Draggable : MonoBehaviour ,IBeginDragHandler,IDragHandler,IEndDragHandler
//实现用于控制拖曳操作的接口
{
    [SerializeField]
    private Vector2 draggingOffset = new Vector2(0.0f, 40.0f);  //拖曳过程中图标的偏量
    private GameObject draggingObject;                          //保持拖曳过程中图标的游戏对象
    private RectTransform canvasRectTransform;                  //保持画布的RectTransform

    private void UpdateDraggingObjectPos(PointerEventData pointerEventData)
    {
        if (draggingObject != null)
        {
            //计算出拖曳过程中的屏幕坐标
            Vector3 screenPos = pointerEventData.position + draggingOffset;
            //将屏幕坐标转换为世界坐标
            Camera camera = pointerEventData.pressEventCamera;
            Vector3 newPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform,screenPos,camera,out newPos))
            {
                //将拖曳过程中的图标位置以世界坐标设置
                draggingObject.transform.position = newPos;
                draggingObject.transform.rotation = canvasRectTransform.rotation;
            }
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if (draggingObject != null)
        {
            Destroy(draggingObject);
        }

        //获取原图标的Image组件
        Image sourceImage = GetComponent<Image>();

        //创建拖曳过程中图标的游戏对象
        draggingObject = new GameObject("DraggingObject");
        //设置为原图标的画布子元素，显示在最前面
        draggingObject.transform.SetParent(sourceImage.canvas.transform);
        draggingObject.transform.SetAsLastSibling();
        draggingObject.transform.localScale = Vector3.one;

        //使用CanvasGroup组件的BlockRaycasts属性
        //让光线投射不被阻隔
        CanvasGroup canvasGroup = draggingObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        //为拖曳过程中图标的游戏对象附加Image组件
        Image draggingImage = draggingObject.AddComponent<Image>();
        //设置为与原图标相同的外观
        draggingImage.sprite = sourceImage.sprite;
        draggingImage.rectTransform.sizeDelta = sourceImage.rectTransform.sizeDelta;
        draggingImage.color = sourceImage.color;
        draggingImage.material = sourceImage.material;

        //保持画布的RectTransform
        canvasRectTransform = draggingImage.canvas.transform as RectTransform;

        //更新拖曳过程中的图标位置
        UpdateDraggingObjectPos(pointerEventData);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        UpdateDraggingObjectPos(pointerEventData);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        Destroy(draggingObject);
    }
}
