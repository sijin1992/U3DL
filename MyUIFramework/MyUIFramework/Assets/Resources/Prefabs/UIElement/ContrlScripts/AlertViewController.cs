using UnityEngine;
using UnityEngine.UI;

//定义类，指定警告框的显示选项
public class AlertViewOptions
{
    public string cancelButtonTitle;        //取消按钮的标题
    public System.Action cancelButtonDelegate;  //按下取消按钮时执行的委托
    public string okButtonTitle;                //确定按钮标题
    public System.Action okButtonDelegate;      //按下确定按钮时执行的委托
}

public class AlertViewController : ViewController   //继承ViewController类
{
    [SerializeField] Text titleLabel;           //显示标题的文本
    [SerializeField] Text messageLabel;         //显示消息的文本
    [SerializeField] Button cancelButton;       //取消按钮
    [SerializeField] Text cancelButtonLabel;    //显示取消按钮标题的文本
    [SerializeField] Button okButton;           //确定按钮
    [SerializeField] Text okButtonLabel;        //显示确定按钮标题的文本

    private static GameObject prefab = null;    //保持警告框的预设
    private System.Action cancelButtonDelegate; //保持按下取消按钮时执行的委托
    private System.Action okButtonDelegate;     //保持按下确定按钮时执行的委托

    //显示警告框的static方法
    public static AlertViewController Show(string title , string message , AlertViewOptions options=null)
    {
        if (prefab == null)
        {
            //读取预设
            prefab = Resources.Load("Prefabs/UIElement/AlertView") as GameObject;
        }

        //将预设实例化，显示警告框
        GameObject obj = Instantiate(prefab) as GameObject;
        AlertViewController alertView = obj.GetComponent<AlertViewController>();
        alertView.UpdateContent(title, message, options);

        return alertView;
    }

    //更新警告框内容的方法
    public void UpdateContent(string title , string message , AlertViewOptions options=null)
    {
        //设置标题和消息
        titleLabel.text = title;
        messageLabel.text = message;

        if (options != null)
        {
            //指定了显示选项的情况下，结合选项内容显示/不显示按钮
            cancelButton.transform.parent.gameObject.SetActive(
                options.cancelButtonTitle != null || options.okButtonTitle != null
            );

            cancelButton.gameObject.SetActive(options.cancelButtonTitle != null);
            cancelButtonLabel.text = options.cancelButtonTitle ?? "";
            cancelButtonDelegate = options.cancelButtonDelegate;

            okButton.gameObject.SetActive(options.okButtonTitle != null);
            okButtonLabel.text = options.okButtonTitle ?? "";
            okButtonDelegate = options.okButtonDelegate;
        }
        else
        {
            //没有指定显示选项的情况下，显示默认按钮
            cancelButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);
            okButtonLabel.text = "OK";
        }
    }

    //关闭警告框的方法
    public void Dismiss()
    {
        Destroy(gameObject);
    }

    //按下取消按钮时调用的方法
    public void OnPressCancelButton()
    {
        if (cancelButtonDelegate != null)
        {
            cancelButtonDelegate.Invoke();
        }
        Dismiss();
    }

    //按下确定按钮时调用的方法
    public void OnPressOKButton()
    {
        if (okButtonDelegate != null)
        {
            okButtonDelegate.Invoke();
        }
        Dismiss();
    }
}
