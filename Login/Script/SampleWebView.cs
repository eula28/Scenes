using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Android;

public class SampleWebView : MonoBehaviour
{
    public string Url;
    public Text status;
    WebViewObject webViewObject;

    [System.Obsolete]
    IEnumerator Start()
    {
        // Check and request permissions
        if (Application.platform == RuntimePlatform.Android)
        {
            string[] permissions = {
                "android.permission.INTERNET",
                "android.permission.ACCESS_NETWORK_STATE",
                "android.permission.ACCESS_FINE_LOCATION",
                "android.permission.ACCESS_COARSE_LOCATION"
            };

            foreach (string permission in permissions)
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                    yield return new WaitForSeconds(1);  // Wait for permission dialog
                }
            }
        }

        webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                status.text = msg;
                status.GetComponent<Animation>().Play();
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                status.text = msg;
                status.GetComponent<Animation>().Play();
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || !UNITY_ANDROID
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
                // Add the JavaScript code to override getCurrentPosition method
                webViewObject.EvaluateJS(@"
                    navigator.geolocation.getCurrentPosition = function(success, error, options) {
                        var defaultOpts = { enableHighAccuracy: true, timeout: Infinity, maximumAge: 0 };
                        options = Object.assign(defaultOpts, options);
                        window.Unity.call('navigator.geolocation.getCurrentPosition');
                    };
                ");

                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            },
            enableWKWebView: true);
        
        webViewObject.SetMargins(2, 0, 2, Screen.height / 7);
        webViewObject.SetVisibility(true);

#if !UNITY_WEBPLAYER
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts) {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://")) {  // for Android
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
                } else {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html") {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
        webViewObject.EvaluateJS(
            "parent.$(function() {" +
            "   window.Unity = {" +
            "       call:function(msg) {" +
            "           parent.unityWebView.sendMessage('WebViewObject', msg)" +
            "       }" +
            "   };" +
            "});");
#endif
        yield break;
    }
}
