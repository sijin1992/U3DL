using UnityEngine;
using UnityEngine.UI;

public class ShopConfirmationViewController : ViewController
{
    [SerializeField] private Text messageLabel;     //显示消息的文本

    //返回视图标题
    public override string Title { get { return "CONFIRATION"; } }

    //确认界面内容
    public void UpdateContent(ShopItemData itemData)
    {
        messageLabel.text = string.Format("Buy {0} for {1} coins?", itemData.name, itemData.price.ToString());
    }

    //点按“CONFIRM”按钮时调用的方法
    public void OnPressConfirmButton()
    {
        string title = "ARE YOU SURE?";
        string message = messageLabel.text;
        //显示警告框
        AlertViewController.Show(title, message, new AlertViewOptions
        {
            //设置取消按钮的标题和被按下时执行的委托
            cancelButtonTitle = "CANCEL",
            cancelButtonDelegate = () => {Debug.Log("Cancelled.");},
            //设置确定按钮的标题和按下时执行的委托
            okButtonTitle = "BUY",
            okButtonDelegate = () => { Debug.Log("Bought."); },
        });
    }
}
