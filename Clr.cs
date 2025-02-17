using Python.Runtime;

namespace PythonContainer;

public class Clr : LazyPyObject
{
    public Clr()
        : base("clr")
    {
    }

    public void AddReference(string assemblyName)
    {
        var addReference = GetOrAddCachedAttribute("AddReference");

        var str = new PyString(assemblyName);

        addReference.Invoke([str]);

        CleanUp(str);
    }
}