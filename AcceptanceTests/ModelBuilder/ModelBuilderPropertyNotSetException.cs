using System;

namespace AcceptanceTests.ModelBuilder;

public class ModelBuilderPropertyNotSetException : Exception
{
    public ModelBuilderPropertyNotSetException() { }
    public ModelBuilderPropertyNotSetException(string message) : base(message) { }
    public ModelBuilderPropertyNotSetException(string message, Exception inner) : base(message, inner) { }
}
