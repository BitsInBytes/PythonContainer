namespace PythonWrapper;

public class Sys : LazyPyObject
{
    public Sys()
        : base("sys")
    {
    }

    public string Version => GetProperty<string>("version");
}