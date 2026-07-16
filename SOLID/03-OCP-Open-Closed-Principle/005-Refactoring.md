# OCP: Refactoring Walkthrough

Starting point: a notification system with a giant `if/elif` chain that has grown
over two years of feature requests.

```python
class NotificationSender:
    def send(self, channel, message, recipient):
        if channel == "email":
            print(f"Emailing {recipient}: {message}")
        elif channel == "sms":
            print(f"Texting {recipient}: {message}")
        elif channel == "push":
            print(f"Push notification to {recipient}: {message}")
        elif channel == "slack":
            print(f"Slack DM to {recipient}: {message}")
        else:
            raise ValueError(f"Unknown channel: {channel}")
```

## Step 1: Identify the axis of variation

The thing that varies is "how the message is delivered." That's the seam for an
interface.

## Step 2: Define the interface

```python
from abc import ABC, abstractmethod

class NotificationChannel(ABC):
    @abstractmethod
    def send(self, message: str, recipient: str) -> None: ...
```

## Step 3: Extract each branch into its own class

```python
class EmailChannel(NotificationChannel):
    def send(self, message, recipient):
        print(f"Emailing {recipient}: {message}")

class SmsChannel(NotificationChannel):
    def send(self, message, recipient):
        print(f"Texting {recipient}: {message}")

class PushChannel(NotificationChannel):
    def send(self, message, recipient):
        print(f"Push notification to {recipient}: {message}")

class SlackChannel(NotificationChannel):
    def send(self, message, recipient):
        print(f"Slack DM to {recipient}: {message}")
```

## Step 4: Replace the branching function with delegation

```python
class NotificationSender:
    def send(self, channel: NotificationChannel, message: str, recipient: str):
        channel.send(message, recipient)
```

## Step 5: Move the "which channel for which name" mapping to one narrow spot

Some lookup is still needed if channels are chosen by string (e.g., from a config
file or user preference) — but now it's an isolated factory/registry, not business
logic mixed with delivery logic:

```python
CHANNEL_REGISTRY = {
    "email": EmailChannel(),
    "sms": SmsChannel(),
    "push": PushChannel(),
    "slack": SlackChannel(),
}

def get_channel(name: str) -> NotificationChannel:
    return CHANNEL_REGISTRY[name]
```

Adding "WhatsApp" now means: write `WhatsAppChannel`, add one line to
`CHANNEL_REGISTRY`. No existing channel class is touched, and `NotificationSender`
never changes again.

## Step 6: Verify the win

- Unit test each channel class independently, with no other channel's code loaded.
- Confirm that adding a new channel required **zero** edits to
  `NotificationSender` and **zero** edits to any existing channel class — only new
  files plus one registry line.

## A note on the registry line

You might notice `CHANNEL_REGISTRY` itself technically gets modified for every new
channel — this is fine and expected. OCP targets *business logic and behavior*, not
configuration/wiring code. Wiring new implementations into a registry, DI container,
or factory is the one acceptable, minimal "modification" every OCP-compliant system
still has, and it's isolated from all real behavior.

Continue to `006-Exercises.md` to practice this pattern yourself.
