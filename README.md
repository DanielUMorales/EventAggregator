# EventAggregator
Simple implementation of Event Aggregator pattern

This is a simple implementation of event aggregator pattern, is based on Prism Framework Event Aggregator, but this version can be used not only in WPF also in Windows Forms, MVC, Console Applications, etc.
For reference see:
https://martinfowler.com/eaaDev/EventAggregator.html

Example of use:
You have to create a simple class for event
    /// <summary>
    /// Event
    /// </summary>
    public class NewEvent
    {
      . . . 
      
Then the publisher, publishs the event as follows:
EventAggregator.PublishEvent(new NewEvent

And finally subscriber have to implement ISubscriber interface
public class Subscriber : ISubscriber<NewEvent>

Subscribe it to Event Aggregator in constructor:
public Subscriber()
{
     . . . 
     EventAggregator.SubsribeEvent(this);
    . . .  
    
And then implement method   OnEventHandler from ISubscriber interface for event processing:
public void OnEventHandler(NewEvent e)
{
   // ToDo: Process event

