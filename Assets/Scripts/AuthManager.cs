using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Firestore;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    // Các biến Firebase
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    // Các biến đăng nhập
    [Header("Đăng nhập")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    // Các biến đăng ký
    [Header("Đăng ký")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    void Awake()
    {
        // Kiểm tra các phụ thuộc cần thiết của Firebase có tồn tại trên hệ thống hay không
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Nếu tồn tại, khởi tạo FireAuth 
                Debug.Log("khoi tao");
                auth = FirebaseAuth.DefaultInstance;
                FirestoreManager.Instance.db = FirebaseFirestore.DefaultInstance;
            }
            else
            {
                Debug.LogError("Không thể giải quyết được tất cả các phụ thuộc của Firebase: " + dependencyStatus);
            }
        });

        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Hàm cho nút đăng nhập
    public void LoginButton()
    {
        // Gọi coroutine đăng nhập và truyền email và mật khẩu
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    // Hàm cho nút đăng ký
    public void RegisterButton()
    {
        // Gọi coroutine đăng ký và truyền email, mật khẩu và tên người dùng
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string email, string password)
    {
        // Gọi hàm đăng nhập của Firebase và truyền email và mật khẩu
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        // Chờ cho đến khi nhiệm vụ hoàn thành
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // Nếu có lỗi, xử lý lỗi
            Debug.LogWarning(message: $"Đăng nhập thất bại với lỗi {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Đăng nhập thất bại!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Thiếu Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Thiếu Mật khẩu";
                    break;
                case AuthError.WrongPassword:
                    message = "Mật khẩu không đúng";
                    break;
                case AuthError.InvalidEmail:
                    message = "Email không hợp lệ";
                    break;
                case AuthError.UserNotFound:
                    message = "Tài khoản không tồn tại";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // Người dùng đã đăng nhập thành công
            // Lấy kết quả
            User = LoginTask.Result.User;
            Debug.LogFormat("Người dùng đăng nhập thành công: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Đăng nhập thành công\n" +
                                    "Wait to load data from firestore";
            // Tải cảnh chính
            // yield return new WaitForSeconds(2);
            yield return StartCoroutine(FirestoreManager.Instance.GetUser());
            UILoginManager.instance.PlayModeScreen();

        }
    }

    private IEnumerator Register(string email, string password, string username)
    {
        if (username == "")
        {
            // Nếu trường tên người dùng không có giá trị, hiển thị cảnh báo
            warningRegisterText.text = "Thiếu tên người dùng";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            // Nếu mật khẩu không khớp, hiển thị cảnh báo
            warningRegisterText.text = "Mật khẩu không khớp!";
        }
        else
        {
            // Gọi hàm đăng ký của Firebase và truyền email và mật khẩu
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            // Chờ cho đến khi nhiệm vụ hoàn thành
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                // Nếu có lỗi, xử lý lỗi
                Debug.LogWarning(message: $"Đăng ký thất bại với lỗi {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Đăng ký thất bại!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Thiếu Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Thiếu Mật khẩu";
                        break;
                    case AuthError.WeakPassword:
                        message = "Mật khẩu yếu";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email đã được sử dụng";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                // Người dùng đã được tạo
                // Lấy kết quả
                User = RegisterTask.Result.User;

                if (User != null)
                {

                    // Tạo hồ sơ người dùng và đặt tên người dùng
                    UserProfile profile = new UserProfile { DisplayName = username };

                    // Gọi hàm cập nhật hồ sơ người dùng của Firebase và truyền hồ sơ với tên người dùng
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    // Chờ cho đến khi nhiệm vụ hoàn thành
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                    yield return StartCoroutine(FirestoreManager.Instance.SaveRegistrationInfo(password));
                    if (ProfileTask.Exception != null)
                    {
                        // Nếu có lỗi, xử lý lỗi
                        Debug.LogWarning(message: $"Cập nhật hồ sơ thất bại với lỗi {ProfileTask.Exception}");
                        warningRegisterText.text = "Thiết lập tên người dùng thất bại!";
                    }
                    else
                    {
                        // Quay trở lại màn hình đăng nhập
                        UILoginManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

}
