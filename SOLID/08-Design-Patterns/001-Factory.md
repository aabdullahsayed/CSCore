# Design Pattern: Factory Method

## Intent

Define an interface for creating an object, but let subclasses (or a dedicated
factory function/class) decide which concrete class to instantiate. This keeps object
*creation* decoupled from object *use*, directly supporting OCP and DIP.

## The problem it solves

```python
class NotificationService:
    def notify(self, channel_type, message):
        if channel_type == "email":
            channel = EmailChannel()
        elif channel_type == "sms":
            channel = SmsChannel()
        # every new channel means editing this method — OCP violation
        channel.send(message)
```

## The pattern

```python
from abc import ABC, abstractmethod

class Channel(ABC):
    @abstractmethod
    def send(self, message): ...

class EmailChannel(Channel):
    def send(self, message): print(f"Email: {message}")

class SmsChannel(Channel):
    def send(self, message): print(f"SMS: {message}")

class ChannelFactory:
    _registry = {"email": EmailChannel, "sms": SmsChannel}

    @classmethod
    def create(cls, channel_type: str) -> Channel:
        if channel_type not in cls._registry:
            raise ValueError(f"Unknown channel: {channel_type}")
        return cls._registry[channel_type]()

    @classmethod
    def register(cls, channel_type: str, channel_cls):
        cls._registry[channel_type] = channel_cls


class NotificationService:
    def notify(self, channel_type, message):
        channel = ChannelFactory.create(channel_type)
        channel.send(message)
```

Adding a new channel now means: write the class, call
`ChannelFactory.register("push", PushChannel)` once — `NotificationService` is never
touched.

## SOLID connections

- **OCP:** new channel types are added without modifying `NotificationService` or the
  factory's `create` logic.
- **DIP:** `NotificationService` depends only on the abstract `Channel`, never on a
  concrete channel class.
- **SRP:** creation logic is isolated in `ChannelFactory`, separate from the
  `NotificationService`'s actual job of orchestrating notification.

## When to use Factory Method vs a plain constructor

Use a factory when:
- object creation requires choosing between multiple concrete classes based on
  runtime input, or
- construction requires nontrivial setup/configuration logic you don't want scattered
  across every call site, or
- you want a single, testable seam where you can substitute a fake object during
  tests.

Skip it when there's exactly one concrete class and no anticipated variants — a plain
constructor call is simpler and equally SOLID-compliant.

See `002-Abstract-Factory.md` for the pattern one level up: factories that produce
*families* of related objects.
