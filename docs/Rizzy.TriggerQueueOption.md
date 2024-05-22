#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## TriggerQueueOption Enum

Determines how events are queued if an event occurs while a request for another event is in flight.

```csharp
public enum TriggerQueueOption
```
### Fields

<a name='Rizzy.TriggerQueueOption.all'></a>

`all` 2

queue all events (issue a request for each event)

<a name='Rizzy.TriggerQueueOption.first'></a>

`first` 0

queue the first event

<a name='Rizzy.TriggerQueueOption.last'></a>

`last` 1

queue the last event (default)

<a name='Rizzy.TriggerQueueOption.none'></a>

`none` 3

do not queue new events

### Remarks
The casing on each of these values matches htmx attributes so that they can be used directly in markup