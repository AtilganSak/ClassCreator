using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class ClassCreatorEditor : EditorWindow
{
    static EditorWindow myEditor;
    
    [MenuItem("Tools/ClassCreater")]
    static void Init()
    {
        myEditor = EditorWindow.GetWindow(typeof(ClassCreatorEditor));
        myEditor.Show();
    }

    string[] monoScriptNames;
    string[] toolBarContents;
    
    int selectedButtonIndex;
    int orderIndex;

    string cacheClassName;
    string cacheTypeName;
    string className;
    string creatingPath;
    string itemName;
    string editorType;
    string fileName;
    string menuName;

    const byte emitMessageLimit = 6;

    bool addOrder;
    bool additionalScriptableObject;
    bool additionalEditor;
    bool generateEditorWindow;

    Rect windoClickArea = new Rect(0, 0, 9999, 9999);
    Vector2 scrollPos;
    GUIContent browsePathGUI;
    Texture2D editorBackgroundTexture;

    List<EditorMessage> editorMessages = new List<EditorMessage>();

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("CreatingPath"))
            creatingPath = PlayerPrefs.GetString("CreatingPath");
        if (PlayerPrefs.HasKey("ClassName"))
            className = PlayerPrefs.GetString("ClassName");
        if (PlayerPrefs.HasKey("TypeName"))
            editorType = PlayerPrefs.GetString("TypeName");

        cacheClassName = className;
        cacheTypeName = editorType;

        monoScriptNames = HelperMethods.GetInhereteds(typeof(MonoScript)).Select(x => x.ToString()).ToArray();

        Texture2D browseIcon = Resources.Load<Texture2D>("Textures/BrowsePathIcon");

        browsePathGUI = new GUIContent(browseIcon, "Browse path for class");

        if (PlayerPrefs.HasKey("ToolBarIndex"))
            selectedButtonIndex = PlayerPrefs.GetInt("ToolBarIndex");

        toolBarContents = new string[3] { "C#", "Editor", "ScriptableObject" };
    }
    private void OnLostFocus()
    {
        PlayerPrefs.SetString("ClassName", className);
        PlayerPrefs.SetString("TypeName", editorType);
        PlayerPrefs.SetString("CreatingPath", creatingPath);

        cacheClassName = className;
        cacheTypeName = editorType;
    }
    private void OnDisable()
    {
        PlayerPrefs.SetString("ClassName", className);
        PlayerPrefs.SetString("TypeName", editorType);
        PlayerPrefs.SetString("CreatingPath", creatingPath);

        cacheClassName = className;
        cacheTypeName = editorType;
    }

    private void OnGUI()
    {
        //if(editorBackgroundTexture == null)
        //    editorBackgroundTexture = Resources.Load<Texture2D>("Textures/EditorBG");
        //GUI.DrawTexture(myEditor.rootVisualElement.contentRect, editorBackgroundTexture, ScaleMode.ScaleAndCrop);

        Event e = Event.current;
        if (e.type == EventType.MouseDown && windoClickArea.Contains(e.mousePosition))
        {
            cacheClassName = className;
            cacheTypeName = editorType;
            GUI.FocusControl("");
        }

        EditorGUI.BeginChangeCheck();
        selectedButtonIndex = GUILayout.Toolbar(selectedButtonIndex, toolBarContents);
        if (EditorGUI.EndChangeCheck())
        {
            PlayerPrefs.SetInt("ToolBarIndex", selectedButtonIndex);
            GUI.FocusControl("");
        }

        DrawByIndex(selectedButtonIndex);

        MessageHandler();
    }
    private void OnProjectChange()
    {
        monoScriptNames = HelperMethods.GetInhereteds(typeof(MonoScript)).Select(x => x.ToString()).ToArray();
    }
    void DrawByIndex(int _index)
    {
        switch (_index)
        {
            case 0:
                DrawCSharpParameters();

                windoClickArea.y = 98f;
                break;
            case 1:
                DrawEditorParameters();

                windoClickArea.y = 177.98f;
                break;
            case 2:
                DrawScriptableObjectParameters();
                break;
        }
    }
    void DrawCSharpParameters()
    {
        EditorGUILayout.BeginVertical("Box");
        DrawSearchableClassNameField(ref className, ref cacheClassName, "Class Name");
        DrawClassPathField();

        if (GUILayout.Button("Generate"))
        {
            if (className != "")
            {
                if (monoScriptNames.Contains(className))
                {
                    EmitMessage("This file already exists! Please enter a different class name!", MessageType.Error);
                    return;
                }

                className = ClassNameUtility.ClearClassName(className);

                string curPath = "Assets/" + creatingPath;

                if (!Directory.Exists(curPath))
                {
                    Directory.CreateDirectory(curPath);
                }

                curPath = curPath + "/" + className + ".cs";

                string template = ClassTemplates.CsharpTemplate;
                template = template.Replace("&class_name&", className);

                CreateScript(curPath, template);
            }
            else
            {
                EmitMessage("Please enter the class name!", MessageType.Warning);
            }
        }

        EditorGUILayout.EndVertical();
    }
    void DrawEditorParameters()
    {
        EditorGUILayout.BeginVertical("Box");

        DrawSearchableClassNameField(ref className, ref cacheClassName, "Class Name");
        DrawSearchableClassNameField(ref editorType, ref cacheTypeName, "Type");
        DrawClassPathField();

        additionalEditor = EditorGUILayout.Toggle("Additional Editor", additionalEditor);
        generateEditorWindow = EditorGUILayout.Toggle("Window", generateEditorWindow);
        if (generateEditorWindow)
        {
            itemName = EditorGUILayout.TextField("Item Name", itemName);
        }

        if (GUILayout.Button("Generate"))
        {
            if (className != "" && editorType != "")
            {
                if (monoScriptNames.Contains(className) && !additionalEditor)
                {
                    if(className == editorType)
                        EmitMessage("Your class name cannot be the same as your type name. Please change your class name or activate the AdditionalEditor option.", MessageType.Error);
                    else
                        EmitMessage("This file already exists! Please enter a different class name!", MessageType.Error);
                    return;
                }

                className = ClassNameUtility.ClearClassName(className);

                if (!generateEditorWindow)
                {
                    if (!monoScriptNames.Contains(editorType))
                    {
                        EmitMessage("Please enter the valid type name!", MessageType.Warning);

                        return;
                    }
                }

                string curPath = "Assets/" + creatingPath;

                if (!Directory.Exists(creatingPath))
                {
                    Directory.CreateDirectory(creatingPath);
                }

                string extension = "";
                if (additionalEditor)
                {
                    if (generateEditorWindow)
                    {
                        if(!className.Contains("EditorWindow"))
                            extension = "EditorWindow";
                    }
                    else
                    {
                        if (!className.Contains("Editor"))
                            extension = "Editor";
                    }
                }
                curPath = curPath + "/" + className + extension + ".cs";

                string template = "";

                if (generateEditorWindow)
                {
                    template = ClassTemplates.EditorWindowTemplate;

                    template = template.Replace("&item_name&", itemName);
                }
                else
                {
                    template = ClassTemplates.EditorTemplate;

                    template = template.Replace("&type&", editorType);
                }
                template = template.Replace("&class_name&", className);

                CreateScript(curPath, template);
            }
            else
            {
                EmitMessage("Please enter the required fields! \n * Class Name \n * Type Name", MessageType.Warning);
            }
        }

        EditorGUILayout.EndVertical();
    }
    void DrawScriptableObjectParameters()
    {
        EditorGUILayout.BeginVertical("Box");

        DrawSearchableClassNameField(ref className, ref cacheClassName, "Class Name");
        fileName = EditorGUILayout.TextField("File Name", fileName);
        menuName = EditorGUILayout.TextField("Menu Name", menuName);
        DrawClassPathField();

        additionalScriptableObject = EditorGUILayout.Toggle("Additional ScriptableObject", additionalScriptableObject);
        addOrder = EditorGUILayout.Toggle("Order", addOrder);
        if (addOrder)
        {
            orderIndex = EditorGUILayout.IntField("Order", orderIndex);
        }

        if (GUILayout.Button("Generate"))
        {
            if (className != "" && fileName != "" && menuName != "")
            {
                if (monoScriptNames.Contains(className))
                {
                    EmitMessage("This file already exists! Please enter a different class name!", MessageType.Error);
                    return;
                }

                className = ClassNameUtility.ClearClassName(className);

                string curPath = "Assets/" + creatingPath;

                if (!Directory.Exists(creatingPath))
                {
                    Directory.CreateDirectory(creatingPath);
                }

                string extension = "";
                if (additionalScriptableObject)
                {
                    if(!className.Contains("ScriptableObject"))
                        extension = "ScriptableObject";
                }
                curPath = curPath + "/" + className + extension + ".cs";

                string template = "";

                template = ClassTemplates.ScriptableObjectTemplate;

                if (addOrder)
                {
                    template = template.Replace("&order&", ", order = " + orderIndex);
                }
                else
                {
                    template = template.Replace("&order&", "");
                }

                template = template.Replace("&file_name&", fileName);
                template = template.Replace("&menu_name&", menuName);
                template = template.Replace("&class_name&", className);

                CreateScript(curPath, template);
            }
            else
            {
                EmitMessage("Please enter the required fields! \n * File Name \n * Menu Name \n * Class Name", MessageType.Warning);
            }
        }

        EditorGUILayout.EndVertical();
    }
    void CreateScript(string _path, string _template)
    {
        if (!File.Exists(_path))
        {
            try
            {
                File.AppendAllText(_path, _template);

                EmitMessage("The script created successfuly.",MessageType.Info);
            }
            catch (System.Exception e)
            {
                EmitMessage(e.Message, MessageType.Error);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    void DrawClassPathField()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 40;
        EditorGUILayout.LabelField("PATH",GUILayout.Width(70));
        EditorGUILayout.LabelField("Assets/", GUILayout.Width(45));
        creatingPath = EditorGUILayout.TextField(creatingPath,GUILayout.Height(21));
        if (GUILayout.Button(browsePathGUI,GUILayout.Width(40)))
        {
            string newPath = HelperMethods.BrowseFolder();
            if (!string.IsNullOrEmpty(newPath))
            {
                GUI.FocusControl("");
                if(!newPath.Contains("Assets/"))
                    creatingPath = newPath;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 150;
    }
    void DrawSearchableClassNameField(ref string _refString, ref string cacheString, string _label)
    {
        _refString = EditorGUILayout.TextField(_label, _refString, GUI.skin.FindStyle("ToolbarSeachTextField"));
        if (!string.Equals(_refString, cacheString) && _refString != "")
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "Box",GUILayout.Height(100));
            if (monoScriptNames != null)
            {
                bool found = false;
                GUI.backgroundColor = Color.cyan;
                for (int i = 0; i < monoScriptNames.Length; i++)
                {
                    if (monoScriptNames[i].ToLower().Contains(_refString.ToLower()))
                    {
                        found = true;
                        if (GUILayout.Button(monoScriptNames[i],EditorStyles.helpBox))
                        {
                            _refString = monoScriptNames[i];
                            cacheString = _refString;

                            EditorGUI.FocusTextInControl("");
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
                if (!found)
                    cacheString = _refString;
            }
            EditorGUILayout.EndScrollView();
        }
    }
    void EmitMessage(string _message, MessageType _messageType = MessageType.None, float _duringTime = 3)
    {
        if (editorMessages.Count < emitMessageLimit)
        {
            editorMessages.Add(
                new EditorMessage
                {
                    Message = _message,
                    MessageType = _messageType,
                    ShowDuring = _duringTime,
                    timer = (float)EditorApplication.timeSinceStartup
                }
            );
        }
    }
    private void MessageHandler()
    {
        for (int i = 0; i < editorMessages.Count; i++)
        {
            EditorGUILayout.HelpBox(editorMessages[i].Message, editorMessages[i].MessageType);
            if (EditorApplication.timeSinceStartup - editorMessages[i].timer > editorMessages[i].ShowDuring)
            {
                editorMessages.RemoveAt(i);
            }
        }
        Repaint();
    }
}
[System.Serializable]
public struct EditorMessage
{
    public string Message;
    public MessageType MessageType;
    public float ShowDuring;
    public float timer { get; set; }
}