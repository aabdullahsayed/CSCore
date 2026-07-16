# Practice Problem: Notification System

## The problem

Design (from scratch) a notification system for an app that needs to send order
updates via email, SMS, and push notification — with the expectation that more
channels (WhatsApp, Slack) will be added later, and that some users want to receive
updates via multiple channels simultaneously.

## Requirements

1. Adding a new channel must not require modifying any existing class (OCP).
2. The core `OrderUpdateNotifier` must not directly depend on any concrete channel
   implementation (DIP).
3. Each channel's formatting/sending logic must be self-contained (SRP).
4. A user's channel preferences should be easy to represent and combine.

## Suggested solution

```python
from abc import ABC, abstractmethod

class NotificationChannel(ABC):
    @abstractmethod
    def send(self, recipient: str, message: str) -> None: ...

class EmailChannel(NotificationChannel):
    def send(self, recipient, message):
        print(f"[Email to {recipient}] {message}")

class SmsChannel(NotificationChannel):
    def send(self, recipient, message):
        print(f"[SMS to {recipient}] {message}")

class PushChannel(NotificationChannel):
    def send(self, recipient, message):
        print(f"[Push to {recipient}] {message}")


class UserNotificationPreference:
    def __init__(self, recipient: str, channels: list[NotificationChannel]):
        self.recipient = recipient
        self.channels = channels


class OrderUpdateNotifier:
    def notify(self, preference: UserNotificationPreference, message: str):
        for channel in preference.channels:
            channel.send(preference.recipient, message)
```

## Usage

```python
alice_prefs = UserNotificationPreference(
    recipient="alice@example.com",
    channels=[EmailChannel(), PushChannel()],
)

notifier = OrderUpdateNotifier()
notifier.notify(alice_prefs, "Your order has shipped!")
```

## Extension exercise

Add a `WhatsAppChannel` and confirm you did not need to touch
`OrderUpdateNotifier`, `UserNotificationPreference`, or any existing channel class —
only a new file and one line wiring it into a user's preference list.
