namespace PythonContainer;

public class Sys : LazyPyObject
{
    public Sys()
        : base("sys")
    {
    }

    public string Version => InvokeProperty<string>("version");

    public void AddPythonFileDirectory(string pathToPythonFiles)
    {
        DynamicObject.path.append(pathToPythonFiles);
    }
}