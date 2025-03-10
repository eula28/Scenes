using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using Google;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FirebaseController : MonoBehaviour
{

    // Singleton instance
    private static FirebaseController instance;

    // Public static property to access the instance
    public static FirebaseController Instance
    {
        get { return instance; }
    }

    // UI Panels
    public GameObject welcomePanel, loginPanel, signupPanel, forgetpassPanel, alertPanel;

    // Input Fields
    public TMP_InputField loginEmail, loginPassword, signupUsername, signupEmail, signupPassword, signupConfirmPass, forgetpassEmail;

    // Text Components
    public TMP_Text alertText;

    // Google Sign-In Configuration
    public string GoogleWebAPI = "84969780262-k6bambkpmoebont0uqmht2sopngoeieh.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    // Firebase Variables
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    bool isSignIn = false;

    // Ensure the constructor is private to prevent external instantiation
    private FirebaseController() { }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Singleton instance initialization
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize Google Sign-In Configuration
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check and Fix Firebase Dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Initialize Firebase if dependencies are available
                InitializeFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Open Welcome Panel
    public void OpenWelcomePanel()
    {
        // Show Welcome Panel and hide others
        welcomePanel.SetActive(true);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetpassPanel.SetActive(false);
    }

    // Open Login Panel
    public void OpenLogInPanel()
    {
        // Show Login Panel and hide others
        welcomePanel.SetActive(false);
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgetpassPanel.SetActive(false);
        // Clear login input fields
        loginEmail.text = "";
        loginPassword.text = "";
    }

    // Open SignUp Panel
    public void OpenSignUpPanel()
    {
        // Show SignUp Panel and hide others
        welcomePanel.SetActive(false);
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        forgetpassPanel.SetActive(false);
        // Clear sign up input fields
        signupUsername.text = "";
        signupEmail.text = "";
        signupPassword.text = "";
        signupConfirmPass.text = "";
    }

    // Open Profile Panel
    public void OpenProfilePanel()
    {
        // Show Profile Panel and hide others
        SceneManager.LoadScene("Account");
        // Retrieve and display user data

    }

    // Open Forget Password Panel
    public void OpenForgetPassPanel()
    {
        // Show Forget Password Panel and hide others
        welcomePanel.SetActive(false);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetpassPanel.SetActive(true);
    }

    // Log In User
    public void LogInUser()
    {
        // Check if login fields are empty
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            ShowAlert("Kindly fill out all required fields.");
            return;
        }
        else
        {
            // Attempt to log in user
            SignUserIn(loginEmail.text, loginPassword.text);
        }
    }

    // Sign Up User
    public async void SignUpUser()
    {
        // Check if sign up fields are empty
        if (string.IsNullOrEmpty(signupUsername.text) || string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text) || string.IsNullOrEmpty(signupConfirmPass.text))
        {
            ShowAlert("Kindly fill out all required fields.");
            return;
        }
        // Check if username already exists
        bool usernameExists = await CheckFirestoreDocument(signupUsername.text);
        if (usernameExists)
        {
            ShowAlert("Username already used");
        }
        else
        {
            // Validate password format
            Regex regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^\w\s]).{8,}$");
            if (!regex.IsMatch(signupPassword.text))
            {
                ShowAlert("Password must be at least 8 characters in length, should contain at least one upper case, one lowercase, one digit, and one special character.");
                return;
            }
            else
            {
                // Check if password matches confirm password
                if (signupPassword.text != signupConfirmPass.text)
                {
                    ShowAlert("Password and Confirm Password does not match.");
                    return;
                }
                else
                {
                    // Create user account
                    CreateUser(signupEmail.text, signupPassword.text, signupUsername.text);
                }
            }
        }
    }

    // Forgot Password
    public void ForgetPass()
    {
        // Check if email field is empty
        if (string.IsNullOrEmpty(forgetpassEmail.text))
        {
            ShowAlert("Enter Your Email");
            return;
        }
        else
        {
            // Send password reset email
            ForgetPassword(forgetpassEmail.text);
        }
    }

    // Show Alert Message
    public void ShowAlert(string message)
    {
        alertText.text = "" + message;
        alertPanel.SetActive(true);
    }

    // Close Alert Panel
    public void CloseAlert(string message)
    {
        alertText.text = "";
        alertPanel.SetActive(false);
    }

    // Log Out User
    public void LogOut()
    {
        // Sign out Firebase and Google users
        if (auth != null && auth.CurrentUser != null)
        {
            auth.SignOut();
        }
        if (GoogleSignIn.DefaultInstance != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        SceneManager.LoadScene("Welcome-Screen");
    }

    // Initialize Firebase
    void InitializeFirebase()
    {
        // Initialize Firebase Authentication
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Authentication State Changed Event
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        // Handle authentication state changes
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    // OnDestroy is called when the MonoBehaviour will be destroyed
    void OnDestroy()
    {
        // Remove authentication state changed listener
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    // Create new user with email and password
    void CreateUser(string email, string password, string username)
    {
        // Create user with email and password asynchronously
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                // Handle any errors that occur during the creation process
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        // Show an alert with the error message
                        ShowAlert(GetErrorMessage(errorCode));
                    }
                }
                return;
            }
            else
            {
                // User creation successful
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
                // Create a Firestore document for the user
                CreateFirestoreDocument(result.User.UserId, email, username, "Email and Password");
                CreateFirestoreDocumentDup(result.User.UserId, username, "Email and Password");
                // Open profile panel after successful creation
                FirebaseUser newUser = result.User;
                SendEmailVerification(newUser);
                SceneManager.LoadScene("GenderSelection");
            }
        });
    }

    void SendEmailVerification(FirebaseUser user)
    {
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                return;
            }
            else
            {
                ShowAlert("Email Verification sent. Check your emails.");
                Debug.Log("Email verification sent successfully.");
            }
        });
    }

    // Sign in existing user with email and password
    void SignUserIn(string email, string password)
    {
        // Sign in user with email and password asynchronously
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                // Handle any errors that occur during the sign-in process
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        // Show an alert with the error message
                        ShowAlert(GetErrorMessage(errorCode));
                    }
                }
                return;
            }
            else
            {
                // Sign-in successful
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            }
        });
    }

    // Update is called once per frame
    bool isSigned = false;
    void Update()
    {
        if (isSignIn)
        {
            if (!isSigned)
            {
                isSigned = true;
                user = auth.CurrentUser;
                string emailCurrent = auth.CurrentUser.Email;
                if (emailCurrent == "cityofimustourism@gmail.com")
                {
                    SceneManager.LoadScene("Admin");
                }
                else
                {
                    OpenProfilePanel();
                }
            }
        }
    }

    // Get Error Message based on Authentication Error Code
    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Email already in use";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Email already in use";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid Email";
                break;
            default:
                message = "Encountered Error";
                break;
        }
        return message;
    }

    // Send Password Reset Email
    void ForgetPassword(string forgetpassEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetpassEmail).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowAlert(GetErrorMessage(errorCode));
                    }
                }
            }
            else
            {
                ShowAlert("Email sent. Check your inbox for an email.");
            }
        });
    }

    // Handle Google Sign-In Click
    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    // Handle Google Authentication Finished
    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Google sign-in task faulted: " + task.Exception);
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Google sign-in task canceled.");
        }
        else
        {
            GoogleSignInUser signInUser = task.Result;
            Debug.Log("Google sign-in successful. User ID: " + signInUser.UserId);

            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(signInUser.IdToken, null);
            Debug.Log("Google sign-in credential created.");

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(async signInTask =>
            {
                if (signInTask.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (signInTask.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + signInTask.Exception);
                    return;
                }

                user = auth.CurrentUser;
                if (user != null)
                {
                    bool documentExists = await CheckFirestoreDocumentID(user.UserId);
                    if (documentExists)
                    {
                        Debug.Log("User document exists. Opening profile panel.");
                        OpenProfilePanel();
                    }
                    else
                    {
                        Debug.Log("User document does not exist. Creating Firestore document and navigating to Gender Selection.");
                        CreateFirestoreDocument(user.UserId, user.Email, user.DisplayName, "Google Sign-In");
                        CreateFirestoreDocumentDup(user.UserId, user.DisplayName, "Google Sign-In");
                        SceneManager.LoadScene("GenderSelection");
                    }
                }
            });
        }
    }

    void CreateFirestoreDocumentDup(string userId, string username, string type)
    {
        System.DateTime currentDateTime = System.DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("MM/dd/yyyy");
        Dictionary<string, object> userDocData = new Dictionary<string, object>
        {
            { "username", username },
            { "user type", type },
        };

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("usersPub").Document(userId);

        userDocRef.SetAsync(userDocData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to create Firestore document: " + task.Exception);
                return;
            }

            Debug.Log("Firestore document created successfully for user: " + userId);
        });
    }

    // Create Firestore Document for User
    void CreateFirestoreDocument(string userId, string email, string username, string type)
    {
        System.DateTime currentDateTime = System.DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("MM/dd/yyyy");
        Dictionary<string, object> userDocData = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "user type", type },
            { "gender model", "" },
            { "bday", "" },
            { "gender", "" },
            { "model number", 0 },
            { "discoveries", 0 },
            { "task achieved", 1},
            { "points", 250},
            { "date start", formattedDateTime}
        };

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        userDocRef.SetAsync(userDocData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to create Firestore document: " + task.Exception);
                return;
            }

            Debug.Log("Firestore document created successfully for user: " + userId);

            // Create subcollections for user achievements
            AddMultipleUserAchievements(userId, new List<(string, bool, int, int, int)>
            {
                ("Setup", true, 1, 250, 1),
                ("5Friends", false, 0, 100, 5),
                ("10Friends", false, 0, 125, 10),
                ("15Friends", false, 0, 150, 15),
                ("5AR", false, 0, 100, 5),
                ("10AR", false, 0, 125, 10),
                ("15AR", false, 0, 150, 15)
            });

            // Create subcollections for user targets
            AddMultipleUserTargets(userId, new List<(string, bool)>
            {
                ("Juanito Reyes Remulla Statue", false),
                ("Imus Cathedral - Marker", false),
                ("Imus Cathedral - Heritage Bells", false),
                ("Imus Cathedral - The Diocesan Shrine and Parish of Our Lady of the Pillar", false),
                ("Imus City Plaza - Carabao's Club", false),
                ("Imus City Plaza - Logo", false),
                ("Imus City Plaza - General Licerio Topacio Statue", false),
                ("Pillar Lodge No.3", false),
                ("Tulay Tomas Mascardo", false),
                ("Imus Historical Landmark", false),
                ("Labanan sa Imus", false),
                ("Bridge of Isabel", false)
            });
        });
    }

    public void AddMultipleUserAchievements(string userId, List<(string achievementId, bool achieved, int progress, int points, int criteria)> achievements)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        WriteBatch batch = db.StartBatch();

        foreach (var achievement in achievements)
        {
            Dictionary<string, object> userAchievementData = new Dictionary<string, object>
        {
            { "achieved", achievement.achieved },
            { "progress", achievement.progress },
            { "points", achievement.points },
            { "criteria", achievement.criteria }
        };

            DocumentReference userAchievementDocRef = db.Collection("users").Document(userId).Collection("userAchievements").Document(achievement.achievementId);
            batch.Set(userAchievementDocRef, userAchievementData);
        }

        batch.CommitAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to create user achievement documents: " + task.Exception);
                return;
            }

            Debug.Log("User achievement documents created successfully for user: " + userId);
        });
    }

    public void AddMultipleUserTargets(string userId, List<(string targetId, bool targetFound)> targets)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        WriteBatch batch = db.StartBatch();

        foreach (var target in targets)
        {
            Dictionary<string, object> userTargetData = new Dictionary<string, object>
        {
            { "target found", target.targetFound }
        };

            DocumentReference userTargetDocRef = db.Collection("users").Document(userId).Collection("userTargets").Document(target.targetId);
            batch.Set(userTargetDocRef, userTargetData);
        }

        batch.CommitAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to create user target documents: " + task.Exception);
                return;
            }

            Debug.Log("User target documents created successfully for user: " + userId);
        });
    }


    // Check if Firestore Document exists based on User ID
    async Task<bool> CheckFirestoreDocumentID(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("usersPub").Document(userId);
        DocumentSnapshot doc = await userDocRef.GetSnapshotAsync();
        Debug.Log("Checking Firestore document for user ID: " + userId);
        return doc.Exists;
    }

    // Check if Firestore Document exists based on Username
    static async Task<bool> CheckFirestoreDocument(string Fireusername)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference usersRef = db.Collection("usersPub");

        // Retrieve all documents in the users collection
        QuerySnapshot usersQuery = await usersRef.GetSnapshotAsync();

        // Flag variable to track the existence of the username
        bool usernameExists = false;

        // Check each document in the collection
        foreach (DocumentSnapshot userDoc in usersQuery.Documents)
        {
            // Check if the document contains the username field
            if (userDoc.ContainsField("username"))
            {
                string username = userDoc.GetValue<string>("username");
                if (username == Fireusername)
                {
                    // Username exists in this document
                    usernameExists = true;
                    break; // No need to check further documents
                }
            }
        }

        // Return true if the username exists in any document, false otherwise
        return usernameExists;
    }

    // Retrieve User Data from Firestore
    public async void GetUserData(string userId, Action<string, string, int, int, string, string, string, string> callback)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        DocumentSnapshot doc = await userDocRef.GetSnapshotAsync();
        if (doc.Exists)
        {
            // Access the data from the document
            string email = doc.GetValue<string>("email");
            string username = doc.GetValue<string>("username");
            int discoveries = doc.GetValue<int>("discoveries");
            int task = doc.GetValue<int>("task achieved");
            string datestart = doc.GetValue<string>("date start");
            string gender_model = doc.GetValue<string>("gender model");
            string bday = doc.GetValue<string>("bday");
            string gender = doc.GetValue<string>("gender");

            // Pass the data to the callback function
            callback(email, username, discoveries, task, datestart, gender_model, bday, gender);
        }
        else
        {
            Debug.Log("Document does not exist for user: " + userId);
        }
    }

    public async void GetUserModelGender(string userId, Action<string, int> callback)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        DocumentSnapshot doc = await userDocRef.GetSnapshotAsync();
        if (doc.Exists)
        {
            // Access the data from the document
            string gendermodel = doc.GetValue<string>("gender model");
            int modelnumber = doc.GetValue<int>("model number");

            // Pass the data to the callback function
            callback(gendermodel, modelnumber);
        }
        else
        {
            Debug.Log("Document does not exist for user: " + userId);
        }
    }
}
