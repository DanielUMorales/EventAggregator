namespace EventAggregator
{
    /// <summary>
    /// Event Aggregator interface.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Method for publish event
        /// </summary>
        /// <typeparam name="TEventType">event type</typeparam>
        /// <param name="eventToPublish">event to publish</param>
        void PublishEvent<TEventType>(TEventType eventToPublish);

        /// <summary>
        /// Method for suscriber
        /// </summary>
        /// <param name="subscriber">suscriber object</param>
        void SubsribeEvent(object subscriber);
    }
}