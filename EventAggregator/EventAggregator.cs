namespace EventAggregator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Event Aggregator implementation.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        /// <summary>
        ///     Lock subscriber dictionary needs for thread safe.
        /// </summary>
        private readonly object lockSubscriberDictionary = new object();

        /// <summary>
        ///     Event subscribers dictionary.
        /// </summary>
        private readonly Dictionary<Type, List<WeakReference>> eventSubscribers = new Dictionary<Type, List<WeakReference>>();

        /// <summary>
        /// Method for publish event
        /// </summary>
        /// <typeparam name="TEventType">event type</typeparam>
        /// <param name="eventToPublish">event to publish</param>
        public void PublishEvent<TEventType>(TEventType eventToPublish)
        {
            var subsriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TEventType));

            var subscribers = this.GetSubscriberList(subsriberType);

            List<WeakReference> subsribersToBeRemoved = new List<WeakReference>();

            foreach (var weakSubsriber in subscribers)
            {
                if (weakSubsriber.IsAlive)
                {
                    var subscriber = (ISubscriber<TEventType>)weakSubsriber.Target;

                    this.InvokeSubscriberEvent(eventToPublish, subscriber);
                }
                else
                {
                    subsribersToBeRemoved.Add(weakSubsriber);
                }
            }

            if (subsribersToBeRemoved.Any())
            {
                lock (this.lockSubscriberDictionary)
                {
                    foreach (var remove in subsribersToBeRemoved)
                    {
                        subscribers.Remove(remove);
                    }
                }
            }
        }

        /// <summary>
        /// Method for suscriber
        /// </summary>
        /// <param name="subscriber">suscriber object</param>
        public void SubsribeEvent(object subscriber)
        {
            lock (this.lockSubscriberDictionary)
            {
                var subsriberTypes = subscriber.GetType().GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));

                var weakReference = new WeakReference(subscriber);

                foreach (var subsriberType in subsriberTypes)
                {
                    var subscribers = this.GetSubscriberList(subsriberType);

                    subscribers.Add(weakReference);
                }
            }
        }

        /// <summary>
        ///     Get the subscriber list.
        /// </summary>
        /// <param name="subsriberType">
        ///     Subscriber type.
        /// </param>
        /// <returns>
        ///     Subscribers list.
        /// </returns>
        private List<WeakReference> GetSubscriberList(Type subsriberType)
        {
            List<WeakReference> subsribersList;

            lock (this.lockSubscriberDictionary)
            {
                var found = this.eventSubscribers.TryGetValue(subsriberType, out subsribersList);

                if (!found)
                {
                    // First time create the list.
                    subsribersList = new List<WeakReference>();

                    this.eventSubscribers.Add(subsriberType, subsribersList);
                }
            }

            return subsribersList;
        }

        /// <summary>
        ///     The invoke subscriber.
        /// </summary>
        /// <param name="eventToPublish">
        ///     Event to publish.
        /// </param>
        /// <param name="subscriber">
        ///     Subscriber object.
        /// </param>
        /// <typeparam name="TEventType">
        ///     Event type
        /// </typeparam>
        private void InvokeSubscriberEvent<TEventType>(TEventType eventToPublish, ISubscriber<TEventType> subscriber)
        {
            // Synchronize the invocation of method
            var syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            syncContext.Post(s => subscriber.OnEventHandler(eventToPublish), null);
        }
    }
}