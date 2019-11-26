using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PageControl : MonoBehaviour {

    [SerializeField] private Toggle indicatorBase;  //页面指示器复制源
    private List<Toggle> indicators = new List<Toggle>();   //保持页面指示器

    //加载实例时调用
    void Awake()
    {
        //令页面指示器复制源处于非活动状态
        indicatorBase.gameObject.SetActive(false);
    }

    //设置页数的方法
    public void SetNumberOfPages(int number)
    {
        if (indicators.Count < number)
        {
            //当页面指示器的数字少于指定的页数时，通过页面指示器复制源创建新页面指示器
            for (int i = indicators.Count; i < number; i++)
            {
                Toggle indicator = Instantiate(indicatorBase) as Toggle;
                indicator.gameObject.SetActive(true);
                indicator.transform.SetParent(indicatorBase.transform.parent);
                indicator.transform.localScale = indicatorBase.transform.localScale;
                indicator.isOn = false;
                indicators.Add(indicator);
            }
        }
        else if(indicators.Count > number)
        {
            //当页面指示器的数字超过指定页数时,删除
            for (int i = indicators.Count - 1; i >= number; i--)
            {
                Destroy(indicators[i].gameObject);
                indicators.RemoveAt(i);
            }
        }
    }

    //当前页的设置方法
    public void SetCurrentPage(int index)
    {
        if (index >= 0 && index <= indicators.Count - 1)
        {
            //由于设置了开关组，令与指定页相对应的页面指示器为ON。
            //所以其他的指示器自动为OFF
            indicators[index].isOn = true;
        }
    }
}
