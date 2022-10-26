using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;


[SerializeField]
public class LoginAuthInfo
{
    public string UserName;
    public string Password;
}
public partial class ZVerseUILogin : MonoBehaviour
{

    public static ZVerseUILogin Instance;

    public UIPopup uiPopup;

    public GameObject panel;

    private ZVerseNetworkManager manager; // singleton=null in Start/Awake
    private ZVerseNetworkAuth auth;
    

    //public Button quitButton;
    //public Dropdown serverDropdown;


    private AuthState currentAuthState = AuthState.Login_Password;  //当前显示状态

    public GameObject[] modelPanel;         //相关面板

    [Header("登录相关")]     
    public InputField loginUserName;

    public InputField loginPassword;

    public Toggle rememberLoginInfo;

    public InputField loginPhoneNumber;

    public InputField loginPhoneCode;

    public Button loginSendCode;

    [Header("注册相关")]
    public InputField registerUserName;

    public InputField registerPassword;

    public InputField registerConfirmPassword;

    public InputField registerPhoneNumber;

    public InputField registerPhoneCode;

    public Button registerSendCode;

    [HideInInspector] public string phoneVerificationCode;




    public void OnClick_ChangeAuth_Login_Password()
    {
        currentAuthState = AuthState.Login_Password;
    }

    public void OnClick_ChangeAuth_Login_PhoneVerification()
    {
        currentAuthState = AuthState.Login_PhoneVerification;
    }

    public void OnClick_ChangeAuth_Register()
    {
        currentAuthState = AuthState.Register;
    }

    public void OnClick_ChangeAuth_Register_PhoneVerification()
    {
        if(!registerPassword.text.Equals(registerConfirmPassword.text))
        {
            uiPopup.Show("密码不一致！");
            return;
        }
        currentAuthState = AuthState.Register_PhoneVerification;

    }

    /// <summary>
    /// 保存账号密码信息
    /// </summary>
    public void SaveLoginInfo()
    {
        if(rememberLoginInfo.isOn)
        {
            LoginAuthInfo obj = new LoginAuthInfo() { UserName = loginUserName.text, Password = loginPassword.text };
            PlayerPrefs.SetString(CommonConstant.LOGIN_INFO, JsonConvert.SerializeObject(obj));
        }
                
    }


    public void  SendCodeTimer()
    {
        StopCoroutine("CodeTimer");
        loginSendCode.interactable = false;
        registerSendCode.interactable = false;
        StartCoroutine("CodeTimer");
    }

    IEnumerator CodeTimer()
    {
        Text login = loginSendCode.GetComponentInChildren<Text>();
        Text register = registerSendCode.GetComponentInChildren<Text>();
        float _time = 60f;
        login.text = _time.ToString();
        register.text = _time.ToString();
        while(_time>0)
        {
            yield return new WaitForSeconds(1f);
            _time--;
            login.text = _time.ToString();
            register.text = _time.ToString();

        }
        loginSendCode.interactable = true;
        registerSendCode.interactable = true;
        login.text = "发送验证码";
        register.text = "发送验证码";
    }

    /// <summary>
    /// 账号密码登录
    /// </summary>
    public void OnClick_Login_Password()
    {
        Debug.LogError($"调试：账号密码登录,账号：{loginUserName.text}，密码{loginPassword.text}");
        auth.userName = loginUserName.text;
        auth.password = loginPassword.text;
        auth.ClientUserLogin(AuthState.Login_Password); 
        
       
    }

    /// <summary>
    /// 手机验证码登录
    /// </summary>
    public void OnClick_Login_PhoneNumber()
    {
        auth.phoneNumber = loginPhoneNumber.text;
        auth.verificationCode = loginPhoneCode.text;
        auth.ClientUserLogin(AuthState.Login_PhoneVerification);
    }


    /// <summary>
    /// 注册发送手机验证码
    /// </summary>
    public void OnClick_Register_PhoneVerification()
    {

        auth.phoneNumber = registerPhoneNumber.text;
        auth.ClientUserSendVerificatoinCode(AuthState.Register_PhoneVerification);

        
    }

    /// <summary>
    /// 登录发送手机验证码
    /// </summary>
    public void OnClick_LoginPhone_Verification()
    {
        auth.phoneNumber = loginPhoneNumber.text;
        auth.ClientUserSendVerificatoinCode(AuthState.Login_PhoneVerification);
    }



   //public void OnClickLocalVerificationCode()
   //{
   //    if(registerPhoneCode.text.Equals(phoneVerificationCode))
   //    {
   //        RegisterPanel.SetActive(true);
   //        PhoneVerificationPanel.SetActive(false);
   //    }else
   //    {
   //        uiPopup.Show("在验证码错误");
   //    }
   //}

    public void RegisterSuccess(long user_id)
    {
        // uiPopup.Show("生成用户ID: "+user_id);
        uiPopup.Show("注册成功！");
        currentAuthState = AuthState.Login_Password;
    }

    public void OnClick_Register_UserInfo()
    {
        auth.userName = registerUserName.text;
        auth.password = registerPassword.text;
        auth.confirmPassword = registerConfirmPassword.text;
        auth.phoneNumber = registerPhoneNumber.text;
        auth.verificationCode = phoneVerificationCode;
        auth.ClientStartUserRegister();
    }



    private void OnEnable()
    {
        currentAuthState = AuthState.Login_Password;

        if(PlayerPrefs.HasKey(CommonConstant.LOGIN_INFO))
        {
            LoginAuthInfo obj = JsonConvert.DeserializeObject<LoginAuthInfo>(PlayerPrefs.GetString(CommonConstant.LOGIN_INFO));
            loginUserName.text = obj.UserName;
            loginPassword.text = obj.Password;
        }

    }

    private void Awake()
    {
        if(Instance==null)
          Instance = this;
    }


    void Start()
    {

        manager = ZVerseNetworkManager.Instance;
        auth = manager.GetComponent<ZVerseNetworkAuth>();

        // if (NetworkClient.connection == null && !ZVerseGameManager.Instance.server)
        //     manager.StartClient();

        // load last server by name in case order changes some day.
        //if (PlayerPrefs.HasKey("LastServer"))
        //{
        //    string last = PlayerPrefs.GetString("LastServer", "");
        //    serverDropdown.value = manager.serverList.FindIndex(s => s.name == last);
        //}

        
    }




    void OnDestroy()
    {
        // save last server by name in case order changes some day
        // PlayerPrefs.SetString("LastServer", serverDropdown.captionText.text);
    }

    void Update()
    {
        if (ZVerseGameManager.Instance.server)
            return;

        for(int i=0;i<modelPanel.Length;i++)
        {
            if (currentAuthState == (AuthState)i)
                modelPanel[i].SetActive(true);
            else
                modelPanel[i].SetActive(false);
        }




        if (manager.state == ZVerseNetworkState.Offline || manager.state == ZVerseNetworkState.Handshake)
        {
       
            panel.SetActive(true);

           // quitButton.onClick.SetListener(() => { ZVerseNetworkManager.Quit(); });
           //
           // serverDropdown.interactable = !manager.isNetworkActive;
           // serverDropdown.options = manager.serverList.Select(
           //     sv => new Dropdown.OptionData(sv.name)
           // ).ToList();
           // manager.networkAddress = manager.serverList[serverDropdown.value].ip;
        }
        else panel.SetActive(false);
    }
    
}
