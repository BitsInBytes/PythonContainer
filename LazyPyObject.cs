using Python.Runtime;

namespace PythonContainer;

public class LazyPyObject
{
    private readonly string _mainImport;
    private readonly Dictionary<string, PyObject> _imports = [];
    private readonly Dictionary<string, PyObject> _attributes = [];

    private PyObject MainImport => GetOrAddCachedImport(_mainImport);

    public PyObject PyObject => MainImport;
    public dynamic DynamicObject => MainImport;

    public LazyPyObject(string @namespace)
    {
        _mainImport = @namespace;
    }

    public PyObject GetImport(string @namespace)
        => GetOrAddCachedImport(@namespace);

    public T GetAttribute<T>(string attribute)
        => GetOrAddCachedAttribute(attribute).As<T>();

    public PyObject GetAttribute(string attribute)
        => GetOrAddCachedAttribute(attribute);

    public void InvokeFunction(string attribute, params object[] args)
    {
        var pyArgs = ConvertToPyObjects(args);
        var attr = GetOrAddCachedAttribute(attribute);

        attr.Invoke(pyArgs.ToArray());

        CleanUp(pyArgs);
    }

    public T InvokeFunction<T>(string attribute, params object[] args)
    {
        var pyArgs = ConvertToPyObjects(args);
        var attr = GetOrAddCachedAttribute(attribute);

        var result = attr.Invoke(pyArgs.ToArray());

        CleanUp(pyArgs);
        
        return result.As<T>();
    }

    protected static IEnumerable<PyObject> ConvertToPyObjects(params object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is PyObject pyObj)
                yield return pyObj;
            else if(arg is LazyPyObject lazyObj)
                yield return lazyObj.PyObject;
            else
                yield return PyObject.FromManagedObject(arg);
        }
    }

    protected static void CleanUp(PyObject[] args)
    {
        foreach (var arg in args)
            arg.Dispose();
    }


    protected static void CleanUp(IEnumerable<PyObject> args)
    {
        foreach (var arg in args)
            arg.Dispose();
    }

    protected PyObject GetOrAddCachedAttribute(string attribute)
    {
        if (!_attributes.TryGetValue(attribute, out var obj))
        {
            obj = MainImport.GetAttr(attribute);
            _attributes[attribute] = obj;
        }

        return obj;
    }

    protected PyObject GetOrAddCachedImport(string import)
    {
        if (!_imports.TryGetValue(import, out var obj))
        {
            obj = Py.Import(import);
            _imports[import] = obj;
        }

        return obj;
    }

    public void Dispose()
    {
        foreach (var property in _attributes)
            property.Value.Dispose();

        foreach (var property in _imports)
            property.Value.Dispose();

        GC.SuppressFinalize(this);
    }
}