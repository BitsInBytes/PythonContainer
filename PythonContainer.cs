using Python.Runtime;
using static Python.Runtime.Py;
using Microsoft.Extensions.DependencyInjection;

namespace PythonWrapper;

public class PythonContainer
    : IDisposable
{
    private readonly GILState _state;
    private readonly ServiceCollection _services;
    private readonly ServiceProvider _provider;

    public PythonContainer(string pathToPythonDll, bool allowThreads)
    {
        Runtime.PythonDLL = pathToPythonDll;

        PythonEngine.Initialize();

        if (allowThreads)
            PythonEngine.BeginAllowThreads();

        _state = Py.GIL();

        _services = new ServiceCollection();
        _provider = _services.BuildServiceProvider();

        ConfigureContainer((services, provider) =>
        {
            services.AddSingleton(this);
        });
    }

    public void ConfigureContainer(Action<ServiceCollection, ServiceProvider> configure)
    {
        configure(_services, _provider);
    }

    public T Resolve<T>() where T : notnull
    {
        var service = _provider.GetService<T>();

        if (service != null)
            return service;

        return ActivatorUtilities.CreateInstance<T>(_provider);
    }

    public void Dispose()
    {
        //PythonEngine.Shutdown(); // UnsafeBinaryFormatterSerialization exception

        _provider.Dispose();
        _state.Dispose();
        
        GC.SuppressFinalize(this);
    }
}