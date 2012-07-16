namespace Sample
{
    /// <summary>
    /// Interface for the application service, which can handle multiple commands.
    /// App Server can have any number of application services 
    /// </summary>
    public interface IApplicationService
    {
        void Execute(ICommand cmd);
    }


    public interface IEvent {}

    public interface ICommand {}
    
    public interface IIdentity {}
}