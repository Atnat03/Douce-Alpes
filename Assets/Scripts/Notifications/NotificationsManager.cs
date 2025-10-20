using System;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class NotificationsManager : MonoBehaviour
{
    [SerializeField] private AndroidNotifications androidNotifications;

    private void Start()
    {
#if UNITY_ANDROID
        androidNotifications.RequestAuthorization();
        androidNotifications.RegisterNotificationChannel();
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
            androidNotifications.SendNotification(
                "Test immédiat 🧪",
                "Notification test après 5 secondes",
                10
            );
#endif
        }
    }

}
