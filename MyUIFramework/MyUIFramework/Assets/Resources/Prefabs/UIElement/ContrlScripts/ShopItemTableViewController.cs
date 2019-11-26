using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ShopItemTableViewController : TableViewController<ShopItemData>
//继承TableViewController<T>类
{
    //读取列表项目数据的方法
    private void LoadData()
    {
        //通常数据是由数据源获取的，但在这里使用硬编码来定义
        tableData = new List<ShopItemData>()
        {
            new ShopItemData { iconName = "tabbar_icon1" , name = "Home1" , price = 100 , description = "*."},
            new ShopItemData { iconName = "tabbar_icon1_on" , name = "Home2" , price = 200 , description = "**."},
            new ShopItemData { iconName = "tabbar_icon2" , name = "Equipment1" , price = 300 , description = "***."},
            new ShopItemData { iconName = "tabbar_icon2_on" , name = "Equipment2" , price = 400 , description = "****."},
            new ShopItemData { iconName = "tabbar_icon3" , name = "Gun1" , price = 500 , description = "*****."},
            new ShopItemData { iconName = "tabbar_icon3_on" , name = "Gun2" , price = 600 , description = "******."},
            new ShopItemData { iconName = "tabbar_icon4" , name = "Shop1" , price = 700 , description = "*******."},
            new ShopItemData { iconName = "tabbar_icon4_on" , name = "Shop2" , price = 800 , description = "********."},
            new ShopItemData { iconName = "tabbar_icon5" , name = "Option1" , price = 900 , description = "*********."},
            new ShopItemData { iconName = "tabbar_icon5_on" , name = "Option2" , price = 1000 , description = "**********."},
            new ShopItemData { iconName = "tabbar_icon1" , name = "Home1" , price = 100 , description = "*."},
            new ShopItemData { iconName = "tabbar_icon1_on" , name = "Home2" , price = 200 , description = "**."},
            new ShopItemData { iconName = "tabbar_icon2" , name = "Equipment1" , price = 300 , description = "***."},
            new ShopItemData { iconName = "tabbar_icon2_on" , name = "Equipment2" , price = 400 , description = "****."},
            new ShopItemData { iconName = "tabbar_icon3" , name = "Gun1" , price = 500 , description = "*****."},
            new ShopItemData { iconName = "tabbar_icon3_on" , name = "Gun2" , price = 600 , description = "******."},
            new ShopItemData { iconName = "tabbar_icon4" , name = "Shop1" , price = 700 , description = "*******."},
            new ShopItemData { iconName = "tabbar_icon4_on" , name = "Shop2" , price = 800 , description = "********."},
            new ShopItemData { iconName = "tabbar_icon5" , name = "Option1" , price = 900 , description = "*********."},
            new ShopItemData { iconName = "tabbar_icon5_on" , name = "Option2" , price = 1000 , description = "**********."},
        };

        //更新滚动内容的大小
        UpdateContents();
    }

    //返回与列表项目相对应的cell高度的方法
    protected override float CellHeightAtIndex(int index)
    {
        if (index >= 0 && index <= tableData.Count - 1)
        {
            if (tableData[index].price >= 1000)
            {
                //如果产品价格超过1000，返回显示cell的高度
                return 200.0f;
            }
            if (tableData[index].price >= 700)
            {
                return 150.0f;
            }
        }
        return 100.0f;
    }

    //加载实例时调用
    protected override void Awake()
    {
        //调用基类Awake方法
        base.Awake();

        //缓存图标精灵表单中所包含的精灵
        SpriteSheetManager.Load("UIAtlas"); 
    }

    //保持导航视图
    [SerializeField] private NavigationViewController navigationView;
    //返回视图的标题
    public override string Title { get { return "SHOP"; } }

    //加载实例时，在Awake方法之后调用
    protected override void Start()
    {
        //调用基类Start方法
        base.Start();

        //读取列表项目的数据
        LoadData();


        if (navigationView != null)
        {
            //设置为导航视图的起始视图
            navigationView.Push(this);
        }
    }

    //保持详细产品界面
    [SerializeField] private ShopDetailViewController detailView;

    //选择cell时调用的方法
    public void OnPressCell(ShopItemTableViewCell cell)
    {
        if (navigationView != null)
        {
            //从被选中的cell中获取产品数据，更新详细产品界面的内容
            detailView.UpdateContent(tableData[cell.DataIndex]);
            //转换至详细产品界面
            navigationView.Push(detailView);
        }
    }
}
