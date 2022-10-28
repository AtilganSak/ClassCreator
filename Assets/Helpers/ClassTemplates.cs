using System;

public class ClassTemplates
{
    static string DefaultNamespaces =
        "using System.Collections;" + Environment.NewLine +
        "using System.Collections.Generic;" + Environment.NewLine +
        "using UnityEngine;";

    public static string CsharpTemplate =
        DefaultNamespaces + Environment.NewLine +
        "public class &class_name& : MonoBehaviour" + Environment.NewLine +
        "{" + Environment.NewLine +
        "   void Start()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       " + Environment.NewLine +
        "   }" + Environment.NewLine +
        "   void Update()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       " + Environment.NewLine +
        "   }" + Environment.NewLine +
        "}";
    public static string EditorTemplate =
        DefaultNamespaces + Environment.NewLine +
        "using UnityEditor;" + Environment.NewLine +
        "[CustomEditor(typeof(&type&))]" + Environment.NewLine +
        "public class &class_name& : Editor" + Environment.NewLine +
        "{" + Environment.NewLine +
        "   private void OnEnable()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       " + Environment.NewLine +
        "   }" + Environment.NewLine +
        "   public override void OnInspectorGUI()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       base.OnInspectorGUI();" + Environment.NewLine +
        "   }" + Environment.NewLine +
        "}";
    public static string EditorWindowTemplate =
        DefaultNamespaces + Environment.NewLine +
        "using UnityEditor;" + Environment.NewLine +
        "public class &class_name& : EditorWindow" + Environment.NewLine +
        "{" + Environment.NewLine +
        "   [MenuItem(\"&item_name&\")]" + Environment.NewLine +
        "   static void Init()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       EditorWindow.GetWindow(typeof(&class_name&)).Show();" + Environment.NewLine +
        "   }" + Environment.NewLine +
        "   private void OnEnable()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       " + Environment.NewLine +
        "   }" + Environment.NewLine +
        "   private void OnGUI()" + Environment.NewLine +
        "   {" + Environment.NewLine +
        "       " + Environment.NewLine +
        "   }" + Environment.NewLine +
        "}";
    public static string ScriptableObjectTemplate =
        DefaultNamespaces + Environment.NewLine +
        "[CreateAssetMenu(fileName = \"&file_name&\", menuName = \"&menu_name&\"&order&)]" + Environment.NewLine +
        "public class &class_name& : ScriptableObject" + Environment.NewLine +
        "{" + Environment.NewLine +
        "       " + Environment.NewLine +
        "}";
}
