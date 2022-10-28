public class ClassNameUtility
{
    public static bool forceCapitalLetter;

    public static string ClearClassName(string _name)
    {
        string clearName = _name.Replace(" ", "_");
        clearName = clearName.Replace("-", "_");
        if (forceCapitalLetter)
        {
            if (_name.Length > 1)
            {
                char firstChar = char.ToUpper(_name[0]);
                clearName = firstChar + clearName.Substring(0);
            }
        }

        return clearName;
    }
}
