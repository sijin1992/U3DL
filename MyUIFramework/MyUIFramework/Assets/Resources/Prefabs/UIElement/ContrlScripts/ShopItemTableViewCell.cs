using UnityEngine;
using UnityEngine.UI;

//定义列表项目的数据类
public class ShopItemData
{
    public string iconName;     //图标名
    public string name;         //产品名
    public int price;           //价格
    public string description;  //说明
}

//继承TableViewCell<T>类
public class ShopItemTableViewCell : TableViewCell<ShopItemData> {
    [SerializeField] private Image iconImage;   //显示图标的图像
    [SerializeField] private Text nameLabel;    //显示产品名的文本
    [SerializeField] private Text priceLabel;   //显示价格的文本

    //覆盖更新cell内容的方法
    public override void UpdateContent(ShopItemData itemData)
    {
        nameLabel.text = itemData.name;
        priceLabel.text = itemData.price.ToString();

        //指定精灵表单名称与精灵名称，修改图标精灵
        iconImage.sprite = SpriteSheetManager.GetSpriteByName("UIAtlas", itemData.iconName);
    }
}
