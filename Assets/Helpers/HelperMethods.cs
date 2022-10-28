using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class HelperMethods
{
    public static T BrowseType<T>(string extension = ".")
    {
        string path = EditorUtility.OpenFilePanel("Select Setting", Application.dataPath, extension);
        if (path.Length != 0)
        {
            if (path.Contains("Assets"))
            {
                path = path.Replace('\\', '/');
                path = path.Substring(path.IndexOf("Assets"));
                System.Object setting = AssetDatabase.LoadAssetAtPath(path, typeof(object));
                if (setting != null)
                    return (T)setting;
                else
                    return default;
            }
            else
                return default;
        }
        else
            return default;
    }
    public static string BrowseFolder()
    {
        string path = EditorUtility.SaveFolderPanel("Select Data Path", Application.dataPath, "folder");

        if (path.Length != 0)
        {
            if (path.Contains("Assets/"))
            {
                path = path.Replace('\\', '/');
                path = path.Substring(path.IndexOf("Assets") + 7);
                return path;
            }
            else
                return "Assets/";
        }
        else
            return "";
    }
    //public T GetInhereted<T>(Type baseClass)
    //{
    //    Type[] types = baseClass.Assembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(baseClass)).ToArray();
    //    return (T)Activator.CreateInstance(typeof(T));
    //}
    public static object[] GetInhereteds(Type baseClass)
    {
        object[] objects = null;
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        if (types != null)
        {
            objects = new object[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                objects[i] = (object)types[i];
            }
        }
        return objects;
    }
}