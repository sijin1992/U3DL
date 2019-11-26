using UnityEngine;
using UnityEngine.UI;

public class ShopDetailViewController : ViewController  //继承ViewController类
{
    //保持导航视图
    [SerializeField] private NavigationViewController navigationView;

    [SerializeField] private Image iconImage;       //显示产品图标的图像
    [SerializeField] private Text nameLabel; //显示产品名称的文本

    private ShopItemData itemData;                  //保持产品数据

    //返回视图标题
    public override string Title
    {
        get { return (itemData != null) ? itemData.name : ""; }
    }

    //更新详细产品界面内容的方法
    public void UpdateContent(ShopItemData itemData)
    {
        //保持产品数据
        this.itemData = itemData;

        iconImage.sprite = SpriteSheetManager.GetSpriteByName("UIAtlas", itemData.iconName);
        nameLabel.text = itemData.name;
    }

    //保持确认界面的视图
    [SerializeField] private ShopConfirmationViewController confirmationView;

    //点“BUY”按钮时调用的方法
    public void OnPressBuyButton()
    {
        //更新确认界面的内容
        confirmationView.UpdateContent(itemData);
        //转换至确认界面
        navigationView.Push(confirmationView);
    }
}
