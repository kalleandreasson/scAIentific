using System;

public class AssistantAlreadyExistsException : Exception
{
    public AssistantAlreadyExistsException()
    {
    }

    public AssistantAlreadyExistsException(string message)
        : base(message)
    {
    }

    public AssistantAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
