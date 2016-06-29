using UnityEngine;
using UnityEditor;
using System.IO;

namespace Area730.Notifications
{

    public class NotificationsWindow : EditorWindow
    {

        #region Vars

        [SerializeField]
        private DataHolder      _dataHolder;
        private static GUISkin  _customSkin             = null;
        private Vector2         _scrollPosition         = Vector2.zero;
        private bool            _showHelp               = false;
        private bool            _showNotificationList   = false;
        private bool            _showSettings           = true;

        #endregion

        #region Editor window config

        void OnEnable()
        {
            if (!Directory.Exists(EditorConstants.RESOURCES_PATH))
            {
                Directory.CreateDirectory(EditorConstants.RESOURCES_PATH);
            }

            _dataHolder = AssetDatabase.LoadAssetAtPath(EditorConstants.SAVE_PATH, typeof(DataHolder)) as DataHolder;

            if (_dataHolder == null)
            {
                _dataHolder = CreateInstance<DataHolder>();
                AssetDatabase.CreateAsset(_dataHolder, EditorConstants.SAVE_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            _customSkin = EditorGUIUtility.Load(EditorConstants.EDITOR_SKIN_FILENAME) as GUISkin;
        }

        void OnDestroy()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem(EditorConstants.EDITOR_WINDOW_MENU_NAME)]
        public static void ShowWindow()
        {
            NotificationsWindow window = (NotificationsWindow)EditorWindow.GetWindow(typeof(NotificationsWindow));
            window.title = EditorConstants.EDITOR_WINDOW_TITLE;
            window.Show();
        }

        #endregion

        void OnGUI()
        {
            GUI.skin        = _customSkin;
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUIStyle labelStyle = GUI.skin.GetStyle(EditorConstants.STYLE_TEXT_FIELD_LABEL);

            EditorGUILayout.LabelField("Notification settings", GUI.skin.label);

            // Help section
            _showHelp = EditorGUILayout.Foldout(_showHelp, "Help", EditorUtils.FoldoutStyle());
            if (_showHelp)
            {
                GUIStyle style = GUI.skin.GetStyle("HelpButtons");

                EditorGUILayout.LabelField("Email: support@area730.com", labelStyle);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Docs", style))
                {
                    Help.BrowseURL(EditorConstants.URL_DOCUMENTATION);
                }

                if (GUILayout.Button("Questions?", style))
                {
                    Help.BrowseURL(EditorConstants.URL_QUESTIONFORM);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Small icon generator", GUI.skin.button, GUILayout.Height(35)))
                {
                    Help.BrowseURL(EditorConstants.URL_SMALL_ICON_GEN);
                }

                if (GUILayout.Button("Large icon generator", GUI.skin.button, GUILayout.Height(35)))
                {
                    Help.BrowseURL(EditorConstants.URL_LARGE_ICON_GEN);
                }

            }

            EditorGUILayout.Space();

            // Settings section
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings", EditorUtils.FoldoutStyle());
            if (_showSettings)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Untiy class", "Class that extends com.unity3d.player.UnityPlayerNativeActivity"), labelStyle, GUILayout.Width(100));
                _dataHolder.unityClass = EditorGUILayout.TextField(_dataHolder.unityClass);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // Notification list
            _showNotificationList = EditorGUILayout.Foldout(_showNotificationList, "Notification list", EditorUtils.FoldoutStyle());
            if (_showNotificationList)
            {
                EditorUtils.DrawDataHolder(_dataHolder);
            }

            GUILayout.EndScrollView();

            if(GUI.changed)
            {
                EditorUtility.SetDirty(_dataHolder);
                Repaint();
            }
        }

    }

}