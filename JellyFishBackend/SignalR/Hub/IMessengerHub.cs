namespace JellyFishBackend.SignalR.Hub
{
    /// <summary>
    /// Defines Methods that should invoked to client
    /// Direction is 'from server/hub to client'
    /// The advantage of this strongly typed client method defines is: you cant missspell them 
    /// </summary>
    public interface IMessengerClient : IStronglyTypedSignalRClient
    {
        public Task Test();
    }
}
