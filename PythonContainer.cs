using Python.Runtime;
using static Python.Runtime.Py;
using Microsoft.Extensions.DependencyInjection;

namespace PythonContainer;

public class PythonContainer
    : IDisposable
{
    private readonly GILState _state;
    private readonly ServiceCollection _services;
    private readonly Lazy<ServiceProvider> _provider;

    public PythonContainer(string pathToPythonDll, bool allowThreads)
    {
        Runtime.PythonDLL = pathToPythonDll;

        PythonEngine.Initialize();

        if (allowThreads)
            PythonEngine.BeginAllowThreads();

        _state = Py.GIL();

        _services = new ServiceCollection();

        ConfigureContainer((services) =>
        {
            services.AddSingleton(this);
            services.AddSingleton<Sys>();
            services.AddSingleton<Clr>();
        });

        _provider = new Lazy<ServiceProvider>(() => _services.BuildServiceProvider());
    }

    public void AddPythonFileDirectory(string pathToPythonFiles)
        => Resolve<Sys>().AddPythonFileDirectory(pathToPythonFiles);

    public void AddReference(Type type)
        => Resolve<Clr>().AddReference(type.Assembly.GetName().Name ?? throw new InvalidDataException("Assembly from type has no name"));

    public void ConfigureContainer(Action<ServiceCollection> configure)
    {
        configure(_services);
    }

    public T Resolve<T>() where T : notnull
    {
        var service = _provider.Value.GetService<T>();

        if (service != null)
            return service;

        throw new NotImplementedException($"{nameof(T)}");
    }

    public void Dispose()
    {
        //PythonEngine.Shutdown(); // UnsafeBinaryFormatterSerialization exception

        _provider.Value.Dispose();
        _state.Dispose();
        
        GC.SuppressFinalize(this);
    }
}