using Python.Runtime;

namespace PythonWrapper;

public class LazyPyObject
{
    private readonly Lazy<PyObject> _module;
    private readonly Dictionary<string, PyObject> _properties = new();

    public LazyPyObject(string @namespace)
    {
        _module = new Lazy<PyObject>(() => Py.Import(@namespace));
    }

    public T GetProperty<T>(string property)
        => GetCachedProperty(property).As<T>();

    private PyObject GetCachedProperty(string property)
    {
        if (!_properties.TryGetValue(property, out var obj))
        {
            obj = _module.Value.GetAttr(property);
            _properties[property] = obj;
        }

        return obj;
    }

    public void Dispose()
    {
        foreach (var property in _properties)
            property.Value.Dispose();

        _module.Value?.Dispose();

        GC.SuppressFinalize(this);
    }
}