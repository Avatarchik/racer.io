
namespace Area730.Notifications
{
    public class EditorConstants
    {

        public const string RESOURCES_PATH          = "Assets/Resources";
        public const string SAVE_PATH               = RESOURCES_PATH + "/NotificationData.asset";
        public const string EDITOR_SKIN_FILENAME    = "EditorStyle.guiskin";
        public const string EDITOR_WINDOW_TITLE     = "Android Local Notifications";
        public const string EDITOR_WINDOW_MENU_NAME = "Window/Android Local Notifications";
        public const string NOTIFICATION_HEADLINE   = "Notification";

        public const string STYLE_DELETE_BTN        = "DeleteButton";
        public const string STYLE_TEXT_FIELD_LABEL  = "TextFieldLabel";

        // Tooltips
        public const string TOOLTIP_AUTO_CANCEL     = "Setting this to true will make it so the notification is automatically canceled when the user clicks it in the panel.";
        public const string TOOLTIP_IS_REPEATING    = "Make the notification appear every interval of time";
        public const string TOOLTIP_ALERT_ONCE      = "Set this flag if you would only like the sound, vibrate and ticker to be played if the notification is not already showing.";
        public const string TOOLTIP_NUMBER          = "Set the large number at the right-hand side of the notification";
        public const string TOOLTIP_DELAY           = "Time before the notification will go off";
        public const string TOOLTIP_INTERVAL        = "Interval of repetition";
        public const string TOOLTIP_DEFAULT_SOUND   = "If enabled - default sound will be used";
        public const string TOOLTIP_VIBRATE         = "If enabled - default vibrate pattern will be used";
        public const string TOOLTIP_VIBRO_PATTERN   = "Vibro pattern for the notification";

        public const string PATH_TO_DRAWABLE        = "Assets/Plugins/Android/Notifications/res/drawable";
        public const string PATH_TO_RAW             = "Assets/Plugins/Android/Notifications/res/raw";

        public const string EXT_JPG                 = ".jpg";
        public const string EXT_JPEG                = ".jpeg";
        public const string EXT_PNG                 = ".png";
        public const string EXT_WAV                 = ".wav";
        public const string EXT_MP3                 = ".mp3";

        public const string PATH_TO_RES             = "Assets/Plugins/Android/Notifications/res";

        public const string URL_DOCUMENTATION       = "https://github.com/Area730/Documentation/wiki/Android-Notifications";
        public const string URL_QUESTIONFORM        = "http://www.emailmeform.com/builder/form/oV8X93wq498NuL7zC";
        public const string URL_SMALL_ICON_GEN      = "http://romannurik.github.io/AndroidAssetStudio/icons-notification.html#source.space.trim=1&source.space.pad=0&name=ic_stat_example";
        public const string URL_LARGE_ICON_GEN      = "http://romannurik.github.io/AndroidAssetStudio/icons-launcher.html";
    }
}
