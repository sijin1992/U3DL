using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chapter4 : MonoBehaviour {
    [SerializeField] private Button button;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Slider slider;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputField inputField;
    [SerializeField] private InputField inputField2;

	// Use this for initialization
	void Start () {
        button.onClick.AddListener(OnClickButton);
        toggle.onValueChanged.AddListener(OnToggle);
        slider.onValueChanged.AddListener(OnSlide);
        scrollbar.onValueChanged.AddListener(OnScroll);
        scrollRect.onValueChanged.AddListener(OnScroll);
        inputField.onValueChange.AddListener(OnValueChange);
        inputField.onEndEdit.AddListener(OnSubmit);
        inputField2.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            //如果输入了小写的英文字母的话，自动变成大写
            char ret = addedChar;
            if (addedChar >= 'a' && addedChar <= 'z')
            {
                ret = (char)(addedChar + ('A' - 'a'));
            }
            return ret;
        };
    }

    public void OnClickButton()
    {
        Debug.Log("Button is clicked!");
    }

    public void OnToggle(bool value)
    {
        Debug.Log("Toggle value is" + value.ToString());
    }

    public void OnSlide(float value)
    {
        Debug.Log("Slider value is" + value.ToString());
    }

    public void OnScroll(float value)
    {
        Debug.Log("Scrollbar value is" + value.ToString());
    }

    public void OnScroll(Vector2 position)
    {
        Debug.Log("Scroll position is" + position.ToString());
    }

    public void OnValueChange(string value)
    {
        Debug.Log("Input Field value is" + value);
    }

    public void OnSubmit(string value)
    {
        Debug.Log("Submit value is" + value);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
