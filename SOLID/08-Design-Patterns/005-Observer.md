# Design Pattern: Observer

## Intent

Define a one-to-many dependency between objects so that when one object (the
**subject**) changes state, all its dependents (**observers**) are notified and
updated automatically — without the subject needing to know any concrete detail about
its observers.

## The problem it solves

Without Observer, a subject that needs to notify multiple interested parties ends up
hardcoding calls to each one:

```python
class Order:
    def mark_shipped(self):
        self.status = "shipped"
        EmailService().send_shipping_notification(self)   # tightly coupled
        SmsService().send_shipping_text(self)              # tightly coupled
        AnalyticsService().track_shipment(self)             # tightly coupled
```

Every new interested party means editing `Order.mark_shipped` — an OCP violation, and
`Order` is now coupled to three unrelated infrastructure classes — a DIP violation
too.

## The pattern

```python
from abc import ABC, abstractmethod

class OrderObserver(ABC):
    @abstractmethod
    def on_shipped(self, order): ...

class EmailNotifier(OrderObserver):
    def on_shipped(self, order):
        print(f"Emailing customer about order {order.id}")

class SmsNotifier(OrderObserver):
    def on_shipped(self, order):
        print(f"Texting customer about order {order.id}")

class AnalyticsTracker(OrderObserver):
    def on_shipped(self, order):
        print(f"Tracking shipment event for order {order.id}")


class Order:
    def __init__(self, id):
        self.id = id
        self.status = "pending"
        self._observers: list[OrderObserver] = []

    def subscribe(self, observer: OrderObserver):
        self._observers.append(observer)

    def mark_shipped(self):
        self.status = "shipped"
        for observer in self._observers:
            observer.on_shipped(self)
```

## Usage

```python
order = Order(id=42)
order.subscribe(EmailNotifier())
order.subscribe(SmsNotifier())
order.subscribe(AnalyticsTracker())

order.mark_shipped()  # notifies all three, automatically
```

Adding a `SlackNotifier` for internal ops alerts means writing one new class and
calling `order.subscribe(SlackNotifier())` — `Order` itself never changes.

## SOLID connections

- **OCP:** new observers plug in without modifying `Order`.
- **DIP:** `Order` depends only on the abstract `OrderObserver`, never on concrete
  `EmailNotifier`/`SmsNotifier` classes.
- **SRP:** `Order` no longer needs to know *how* to email, text, or track — it only
  knows *that* it should announce a shipping event.

## Real-world instances of this pattern

Event listeners in UI frameworks, pub/sub message queues, reactive programming
libraries (RxJS/RxPy), and Python's own `logging` module (handlers subscribe to a
logger) are all applications of Observer at different scales.
