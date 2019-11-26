using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class NavigationViewController : ViewController //继承ViewController类
{
    private Stack<ViewController> stackedViews = new Stack<ViewController>();   //保持视图层次的堆叠
    private ViewController currentView = null;                                  //保持当前视图

    [SerializeField] private Text titleLael;        //显示导航栏标题的文本
    [SerializeField] private Button backButton;     //导航栏的返回按钮
    [SerializeField] private Text backButtonLabel;  //返回按钮的文本

    //加载实例时调用
    void Awake()
    {
        //设置返回按钮的事件监听器
        backButton.onClick.AddListener(OnPressBackButton);
        //不显示最开始的返回按钮
        backButton.gameObject.SetActive(false);
    }

    //点击返回按钮时调用的方法
    public void OnPressBackButton()
    {
        //返回上一层视图
        Pop();
    }

    //使用户互动有效/无效的方法
    private void EnableInteraction(bool isEnabled)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = isEnabled;
    }

    //执行转换到下一层视图处理的方法
    public void Push(ViewController newView)
    {
        if (currentView == null)
        {
            //最开始的视图无动画显示
            newView.gameObject.SetActive(true);
            currentView = newView;
            return;
        }

        //动画过程中，使用用户的互动无效
        EnableInteraction(false);

        //将当前显示的视图移动到画面左边外侧
        ViewController lastView = currentView;
        stackedViews.Push(lastView);
        Vector2 lastViewPos = lastView.CachedRectTransform.anchoredPosition;
        lastViewPos.x = -this.CachedRectTransform.rect.width;
        lastView.CachedRectTransform.MoveTo(
            lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => 
            {
                //移动结束后，将视图设置为非活动
                lastView.gameObject.SetActive(false);
            }
        );

        //将新视图从界面右边外侧移动到中央
        newView.gameObject.SetActive(true);
        Vector2 newViewPos = newView.CachedRectTransform.anchoredPosition;
        newView.CachedRectTransform.anchoredPosition = new Vector2(this.CachedRectTransform.rect.width, newViewPos.y);
        newViewPos.x = 0.0f;
        newView.CachedRectTransform.MoveTo(
            newViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => 
            {
                //移动结束后，使用户互动为有效
                EnableInteraction(true);
            }
        );

        //将新视图作为当前视图保持，改变导航栏的标题
        currentView = newView;
        titleLael.text = newView.Title;

        //改变返回按钮的标签
        backButtonLabel.text = lastView.Title;
        //将返回按钮设置为活动
        backButton.gameObject.SetActive(true);
    }

    //执行返回上一层视图处理的方法
    public void Pop()
    {
        if (stackedViews.Count < 1)
        {
            //由于没有上一层视图，所以不采取任何操作
            return;
        }

        //动画过程中，使用户互动无效
        EnableInteraction(false);

        //将当前显示的视图移动到画面右边外侧
        ViewController lastView = currentView;
        Vector2 lastViewPos = lastView.CachedRectTransform.anchoredPosition;
        lastViewPos.x = this.CachedRectTransform.rect.width;
        lastView.CachedRectTransform.MoveTo(
            lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () =>
            {
                //移动结束后，将视图设置为非活动
                lastView.gameObject.SetActive(false);
            }
        );

        //将上一层视图从堆叠中还原，由界面左边外侧移动至中央
        ViewController poppedView = stackedViews.Pop();
        poppedView.gameObject.SetActive(true);
        Vector2 poppedViewPos = poppedView.CachedRectTransform.anchoredPosition;
        poppedViewPos.x = 0.0f;
        poppedView.CachedRectTransform.MoveTo(
            poppedViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () =>
            {
                //移动结束后，使用户的互动为有效
                EnableInteraction(true);
            }
        );

        //将由堆叠还原后的视图作为当前视图保持，改变导航栏的标题
        currentView = poppedView;
        titleLael.text = poppedView.Title;

        //当有前一层视图时，修改返回按钮的标签，设置为活动
        if (stackedViews.Count >= 1)
        {
            backButtonLabel.text = stackedViews.Peek().Title;
            backButton.gameObject.SetActive(true);
        }
        else
        {
            backButton.gameObject.SetActive(false);
        }
    }
}