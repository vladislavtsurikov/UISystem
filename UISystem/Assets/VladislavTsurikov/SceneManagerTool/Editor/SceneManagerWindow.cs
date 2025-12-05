#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneUtility.Editor;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public enum SettingsType
    {
        Scenes,
        Settings
    }

    public class SceneManagerWindow : BaseWindow<SceneManagerWindow>
    {
        private const string WINDOW_TITLE = "Scene Manager";
        public const string k_WindowMenuPath = "Window/Vladislav Tsurikov/Scene Manager";
        private SettingsType _settingsType = SettingsType.Scenes;
        private Vector2 _windowScrollPos;

        protected override void OnEnable()
        {
            base.OnEnable();

            EditorApplication.modifierKeysChanged += Repaint;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EditorApplication.modifierKeysChanged -= Repaint;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            OnMainGUI();

            SceneManagerData.MaskAsDirty();
        }

        [MenuItem(k_WindowMenuPath, false, 0)]
        public static void Open() => OpenWindow(WINDOW_TITLE);

        private void OnMainGUI()
        {
            EditorGUI.BeginChangeCheck();

            _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            if (SceneManagerData.Instance.Profile == null)
            {
                EditorGUI.BeginDisabledGroup(true);
                DrawSettingsTypes();
                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);

                    Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                    rect.width -= 60;
                    SceneManagerData.Instance.Profile = (Profile)CustomEditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), null,
                        SceneManagerData.Instance.Profile, typeof(Profile));

                    var clickRect = new Rect(rect.x + rect.width + 2, rect.y, 60, rect.height);

                    if (CustomEditorGUI.ClickButton(clickRect, "Create", ButtonStyle.Add))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            SceneManagerData.Instance.Profile = Profile.CreateProfile();
                        };
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (SceneManagerData.Instance.EnableSceneManager == false)
                {
                    Rect backgroundRect = EditorGUILayout.GetControlRect(true, 30);

                    Rect settingsRect = CustomEditorGUILayout.ScreenRect;
                    settingsRect.width = 170;
                    settingsRect.height = 30;
                    settingsRect.y = backgroundRect.y;
                    settingsRect.y += 5;
                    settingsRect.x = CustomEditorGUILayout.ScreenRect.width / 2 - settingsRect.width / 2;

                    if (CustomEditorGUI.ClickButton(settingsRect, "Enable Scene Manager", ButtonStyle.Add))
                    {
                        SceneManagerData.Instance.EnableSceneManager = true;
                    }

                    GUILayout.Space(5);

                    CustomEditorGUILayout.HelpBox(
                        "If you enable Scene Manager, this will remove your added scenes in Build Settings and will only add added automatically scenes to Scene Manager in Build Settings.");
                }
                else
                {
                    DrawSettingsTypes();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);

                        GUILayout.BeginVertical();
                        {
                            if (_settingsType == SettingsType.Scenes)
                            {
                                SceneManagerData.Instance.Profile.BuildSceneCollectionStackEditor.OnTabStackGUI();

                                SceneManagerData.Instance.Profile.BuildSceneCollectionStackEditor
                                    .DrawSelectedSettings();
                            }
                            else
                            {
                                SceneManagerData.Instance.Profile =
                                    (Profile)CustomEditorGUILayout.ObjectField(new GUIContent("Profile"),
                                        SceneManagerData.Instance.Profile, typeof(Profile));
                                SceneManagerData.Instance.EnableSceneManager =
                                    CustomEditorGUILayout.Toggle(new GUIContent("Enable Scene Manager"),
                                        SceneManagerData.Instance.EnableSceneManager);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(SceneManagerData.Instance);
                if (SceneManagerData.Instance.Profile != null)
                {
                    ScenesInBuildUtility.Setup(SceneManagerData.Instance.GetAllScenePaths());
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        public void DrawSettingsTypes()
        {
            Color initialColor = GUI.color;

            Rect backgroundRect = EditorGUILayout.GetControlRect(true, 60);

            GUI.color = new Color().From256(48, 48, 48);
            GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);
            GUI.color = initialColor;

            Rect rectPlay = backgroundRect;
            rectPlay = EditorGUI.IndentedRect(rectPlay);
            rectPlay.height = 40;
            rectPlay.width = 50;
            rectPlay.x += 15;
            rectPlay.y += 10;

            if (CustomEditorGUI.ClickButton(rectPlay, "Play"))
            {
                SceneManagerEditorUtility.EnterPlaymode();
            }

            Rect settingsRect = CustomEditorGUILayout.ScreenRect;
            settingsRect.y = rectPlay.y;
            settingsRect.y += 5;
            settingsRect.x = CustomEditorGUILayout.ScreenRect.width / 2;
            settingsRect.width = 70;
            settingsRect.height = 30;

            if (CustomEditorGUI.RectTab(settingsRect, "Settings", _settingsType == SettingsType.Settings))
            {
                _settingsType = SettingsType.Settings;
            }

            settingsRect.x -= settingsRect.width + 3;

            if (CustomEditorGUI.RectTab(settingsRect, "Scenes", _settingsType == SettingsType.Scenes))
            {
                _settingsType = SettingsType.Scenes;
            }
        }
    }
}
#endif
