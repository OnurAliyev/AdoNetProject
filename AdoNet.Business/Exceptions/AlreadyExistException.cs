namespace AdoNet.Business.Exceptions;

public class AlreadyExistException:Exception
{
    public AlreadyExistException(string message) : base(message) { }
}
