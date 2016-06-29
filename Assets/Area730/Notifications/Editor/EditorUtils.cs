using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace Area730.Notifications
{
    /// <summary>
    /// Some utils to draw notification editor window
    /// </summary>
    public class EditorUtils
    {

        /// <summary>
        /// Android allowed filename regex
        /// </summary>
        private static Regex regex = new Regex("^[a-z0-9_.]+$");

        /// <summary>
        /// Draws the dataholder editor ui
        /// </summary>
        /// <param name="dataHolder">Dataholder to draw</param>
        public static void DrawDataHolder(DataHolder dataHolder)
        {
            EditorGUILayout.Space();

            foreach (var notif in dataHolder.notifications)
            {
                DrawNotification(notif);

                GUIStyle deleteBtnStyle = GUI.skin.GetStyle(EditorConstants.STYLE_DELETE_BTN);

                if (GUILayout.Button("Delete", deleteBtnStyle, GUILayout.Width(100)))
                {
                    dataHolder.notifications.Remove(notif);
                    GUI.changed = true;
                    break;
                }

                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add Notification"))
            {
                dataHolder.notifications.Add(new NotificationInstance());
            }
        }

        /// <summary>
        /// Draws the notification editor ui
        /// </summary>
        /// <param name="notif">Notification to draw</param>
        public static void DrawNotification(NotificationInstance notif)
        {
            GUIStyle labelStyle = GUI.skin.GetStyle(EditorConstants.STYLE_TEXT_FIELD_LABEL);

            // draw headline
            EditorGUILayout.LabelField(EditorConstants.NOTIFICATION_HEADLINE, GUI.skin.label);


            DrawTextField(labelStyle, "Name", ref notif.name);

            DrawIntField(labelStyle, "ID", "Notification id", ref notif.id);

            HandleIconField(labelStyle, "Small icon", ref notif.smallIcon);
            HandleIconField(labelStyle, "Large icon", ref notif.largeIcon);


            DrawTextField(labelStyle, "Title", ref notif.title);
            if (notif.title == null || notif.title.Length == 0)
            {
                EditorGUILayout.HelpBox("Titile can't be empty", MessageType.Error);
            }
            DrawTextField(labelStyle, "Body", ref notif.body);
            if (notif.body == null || notif.body.Length == 0)
            {
                EditorGUILayout.HelpBox("Body can't be empty", MessageType.Error);
            }
            DrawTextField(labelStyle, "Ticker", ref notif.ticker);

            DrawTextField(labelStyle, "Group", ref notif.group);
            DrawTextField(labelStyle, "Sort key", ref notif.sortKey);

            DrawToggleField(labelStyle, "Auto Cancel", EditorConstants.TOOLTIP_AUTO_CANCEL, ref notif.autoCancel);
            DrawToggleField(labelStyle, "Is repeating?", EditorConstants.TOOLTIP_IS_REPEATING, ref notif.isRepeating);

            // If Repeating
            if (notif.isRepeating)
            {
                EditorGUILayout.LabelField(new GUIContent("Interval:", EditorConstants.TOOLTIP_INTERVAL), labelStyle, GUILayout.Width(100));

                EditorGUI.indentLevel += 2; ;
                DrawIntField(labelStyle, "Hours", "", ref notif.intervalHours, GUILayout.Width(100));
                DrawIntField(labelStyle, "Minutes", "", ref notif.intervalMinutes, GUILayout.Width(100));
                DrawIntField(labelStyle, "Seconds", "", ref notif.intervalSeconds, GUILayout.Width(100));
                EditorGUI.indentLevel -= 2;
            }

            // Delay
            EditorGUILayout.LabelField(new GUIContent("Delay", EditorConstants.TOOLTIP_DELAY), labelStyle, GUILayout.Width(100));

            EditorGUI.indentLevel += 2;
            DrawIntField(labelStyle, "Hours", "Hours before the norification will be shown", ref notif.delayHours, GUILayout.Width(100));
            DrawIntField(labelStyle, "Minutes", "Minutes before the norification will be shown", ref notif.delayMinutes, GUILayout.Width(100));
            DrawIntField(labelStyle, "Seconds", "Seconds before the norification will be shown", ref notif.delaySeconds, GUILayout.Width(100));
            EditorGUI.indentLevel -= 2;

            //
            DrawToggleField(labelStyle, "Alert once", EditorConstants.TOOLTIP_ALERT_ONCE, ref notif.alertOnce);
            DrawToggleField(labelStyle, "Default Sound", EditorConstants.TOOLTIP_DEFAULT_SOUND, ref notif.defaultSound);

            if (!notif.defaultSound)
            {
                HandleAudioField(labelStyle, "Sound clp", ref notif.soundFile);
            }

            DrawToggleField(labelStyle, "Default Vibrate", EditorConstants.TOOLTIP_VIBRATE, ref notif.defaultVibrate);

            if (!notif.defaultVibrate)
            {
                EditorGUILayout.LabelField(new GUIContent("Vibro pattern", EditorConstants.TOOLTIP_VIBRO_PATTERN), labelStyle, GUILayout.Width(100));

                // vibration pattern list
                EditorGUI.indentLevel += 2;
                for (int i = 0; i < notif.vibroPattern.Count; ++i)
                {

                    EditorGUILayout.BeginHorizontal();
                    GUIContent cont;
                    if (i % 2 == 0)
                    {
                        cont = new GUIContent("Pause", "Miliseconds before next vibration");
                    }
                    else
                    {
                        cont = new GUIContent("On", "Length of vebration in miliseconds");
                    }
                    EditorGUILayout.LabelField(cont, labelStyle, GUILayout.Width(100));
                    notif.vibroPattern[i] = EditorGUILayout.IntField((int)notif.vibroPattern[i], GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }

                // Add/Remove buttons
                GUIStyle smallBtnStyle = EditorStyles.miniButtonLeft;
                smallBtnStyle.margin.left = 40;

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", smallBtnStyle, miniButtonWidth))
                {
                    notif.vibroPattern.Add(1000);
                }
                if (GUILayout.Button("-", EditorStyles.miniButtonRight, miniButtonWidth))
                {
                    // remove last if the list is not empty
                    int elemCount = notif.vibroPattern.Count;
                    if (elemCount > 0)
                    {
                        notif.vibroPattern.RemoveAt(elemCount - 1);
                    }

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUI.indentLevel -= 2;
              
            }

            DrawIntField(labelStyle, "Number", EditorConstants.TOOLTIP_NUMBER, ref notif.number);
            DrawToggleField(labelStyle, "Has color?", "Color in the circle", ref notif.hasColor);
            if (notif.hasColor)
            {
                EditorGUI.indentLevel += 2;
                DrawColorField(labelStyle, "Color", ref notif.color, GUILayout.Width(100));
                EditorGUI.indentLevel -= 2;
            }
            
        }

        /// <summary>
        /// Draws the icon field and copies the icon file into res/drawable folder
        /// </summary>
        private static void HandleIconField(GUIStyle style, string title, ref Texture2D icon)
        {
            Texture2D oldIcon = icon;

            DrawObjectField(style, title, ref icon);

            if (oldIcon != icon)
            {
                if (icon == null)
                {
                    return;
                }

                if (!Directory.Exists(EditorConstants.PATH_TO_DRAWABLE))
                {
                    EditorUtility.DisplayDialog("ERROR", "Path " + EditorConstants.PATH_TO_DRAWABLE + " doesn't exist. Changes weren't applied", "OK");
                    // revert changes
                    icon = oldIcon;
                    return;
                }

                string newIconPath          = AssetDatabase.GetAssetPath(icon);
                string newIconFileName      = Path.GetFileName(newIconPath);
                string ext                  = Path.GetExtension(newIconPath);
                string androidNewIconPath   = Path.Combine(EditorConstants.PATH_TO_DRAWABLE, newIconFileName);

                // check is filename is allowed
                if (!regex.IsMatch(newIconFileName))
                {
                    EditorUtility.DisplayDialog("ERROR", "New texture wasn't applied. Android allows only names that contain [a-z0-9_.]. Please rename your file and try again.", "OK");
                    // revert changes
                    icon = oldIcon;
                    return;
                }

                // check if the extension is correct
                if (!(ext.Equals(EditorConstants.EXT_JPG) || ext.Equals(EditorConstants.EXT_PNG) || ext.Equals(EditorConstants.EXT_JPEG)))
                {
                    EditorUtility.DisplayDialog("ERROR", "Wrong texture extension. Texture extension should be .jpg or .png. New texture wasn't applied.", "OK");
                    // revert changes
                    icon = oldIcon;
                    return;
                }

                // copy if file not exist
                if (!File.Exists(androidNewIconPath))
                {
                    File.Copy(newIconPath, androidNewIconPath);
                    AssetDatabase.Refresh();
                }

                // link icon to file from res/drawable
                Texture2D tex = AssetDatabase.LoadAssetAtPath(androidNewIconPath, typeof(Texture2D)) as Texture2D;
                icon = tex;
            }
        }

        /// <summary>
        /// Draws audio field and copies the clip into res/raw folder
        /// </summary>
        private static void HandleAudioField(GUIStyle style, string title, ref AudioClip clip)
        {
            AudioClip oldClip = clip;

            // draw audio field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sound file", style, GUILayout.Width(100));
            clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(AudioClip), false);
            EditorGUILayout.EndHorizontal();

            if (oldClip != clip)
            {
                if (clip == null)
                {
                    return;
                }

                if (!Directory.Exists(EditorConstants.PATH_TO_RAW))
                {
                    EditorUtility.DisplayDialog("ERROR", "Path " + EditorConstants.PATH_TO_RAW + " doesn't exist. Changes weren't applied", "OK");
                    // revert changes
                    clip = oldClip;
                    return;
                }

                string newClipPath          = AssetDatabase.GetAssetPath(clip);
                string newClipFileName      = Path.GetFileName(newClipPath);
                string androidNewClipPath   = Path.Combine(EditorConstants.PATH_TO_RAW, newClipFileName);

                // check is filename is allowed
                if (!regex.IsMatch(newClipFileName))
                {
                    EditorUtility.DisplayDialog("ERROR", "New sound wasn't applied. Android allows only names that contain [a-z0-9_.]. Please rename your file and try again.", "OK");
                    // revert changes
                    clip = oldClip;
                    return;
                }

                // copy if file not exist
                if (!File.Exists(androidNewClipPath))
                {
                    File.Copy(newClipPath, androidNewClipPath);
                    AssetDatabase.Refresh();
                }

                // link clip to file in res/raw folder
                AudioClip rawClip = AssetDatabase.LoadAssetAtPath(androidNewClipPath, typeof(AudioClip)) as AudioClip;
                clip = rawClip;
            }
        }

        //==================================================================================

        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

        /// <summary>
        /// Returns the GUIStyle for Foldout element
        /// </summary>
        /// <returns></returns>
        public static GUIStyle FoldoutStyle()
        {
            GUIStyle labelStyle = GUI.skin.customStyles[2];
            GUIStyle s = EditorStyles.foldout;
            s.font = labelStyle.font;
            s.fontSize = 12;
            return s;
        }

        public static void DrawTextField(GUIStyle style, string label, ref string var)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, style, GUILayout.Width(100));
            var = EditorGUILayout.TextField(var);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawObjectField(GUIStyle style, string label, ref Texture2D var)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, style, GUILayout.Width(100));
            var = (Texture2D)EditorGUILayout.ObjectField(var, typeof(Texture2D), false);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawColorField(GUIStyle style, string label, ref Color var, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, style, GUILayout.Width(100));
            var = EditorGUILayout.ColorField(var, options);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawToggleField(GUIStyle style, string label, string tooltip, ref bool var)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, tooltip), style, GUILayout.Width(100));
            var = EditorGUILayout.Toggle(var);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawIntField(GUIStyle style, string label, string tooltip, ref int var, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, tooltip), style, GUILayout.Width(100));
            var = EditorGUILayout.IntField(var, options);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawLongField(GUIStyle style, string label, string tooltip, ref long var, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, tooltip), style, GUILayout.Width(100));
            var = EditorGUILayout.IntField((int)var, options);
            EditorGUILayout.EndHorizontal();
        }


    }
}
