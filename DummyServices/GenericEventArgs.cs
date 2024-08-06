namespace SimonV839.DummyServices
{
    // was used for unsolicited sign out but as this should never occur - not used yet
    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        public GenericEventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
