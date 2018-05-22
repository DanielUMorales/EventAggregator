namespace EventAggregator
{
    /// <summary>
    /// The Subscriber interface.
    /// </summary>
    /// <typeparam name="TEventType">
    /// Event type
    /// </typeparam>
    public interface ISubscriber<in TEventType>
    {
        /// <summary>
        /// Handler for event
        /// </summary>
        /// <param name="e">event object</param>
        void OnEventHandler(TEventType e);
    }
}