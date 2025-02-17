PythonContainer is a basic wrapper & DI container around the PythonNet package that:
* Allows you to interact with PythonNet in a more object oriented way, using LazyPyObject instead of PyObject
* Exposes itself as a self contained DI container to help quickly resolve objects to make complex stacks easier to maintain

LazyPyObject is a basic wrapper around the PyObject that:
* Is lazy so DI won't ever crash until the object is invoked after parameters have been resolved
* Comes with built in caching to cache disposable internal objects (like attributes / properties pass throughs)
* Contain built in clean up using IDisposable so clean up is all done for you
