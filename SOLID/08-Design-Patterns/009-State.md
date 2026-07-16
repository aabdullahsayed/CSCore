# Design Pattern: State

## Intent

Allow an object to alter its behavior when its internal state changes — the object
will appear to change its class. Structurally very similar to Strategy, but focused on
managing state *transitions* over an object's lifecycle.

## The problem: state machines built from booleans and flags

```python
class Order:
    def __init__(self):
        self.is_paid = False
        self.is_shipped = False
        self.is_delivered = False
        self.is_cancelled = False

    def ship(self):
        if not self.is_paid:
            raise Exception("Cannot ship unpaid order")
        if self.is_cancelled:
            raise Exception("Cannot ship cancelled order")
        self.is_shipped = True
```

As more states and transitions are added, these boolean flags and their guard
conditions multiply combinatorially, and every method needs updated
`if`-guards for every new state — an OCP violation, and increasingly unreadable.

## The State pattern

```python
from abc import ABC, abstractmethod

class OrderState(ABC):
    @abstractmethod
    def pay(self, order): ...
    @abstractmethod
    def ship(self, order): ...
    @abstractmethod
    def cancel(self, order): ...


class PendingState(OrderState):
    def pay(self, order):
        print("Payment received.")
        order.state = PaidState()
    def ship(self, order):
        print("Cannot ship — payment not received yet.")
    def cancel(self, order):
        print("Order cancelled.")
        order.state = CancelledState()

class PaidState(OrderState):
    def pay(self, order):
        print("Already paid.")
    def ship(self, order):
        print("Order shipped.")
        order.state = ShippedState()
    def cancel(self, order):
        print("Order cancelled, refund issued.")
        order.state = CancelledState()

class ShippedState(OrderState):
    def pay(self, order):
        print("Already paid.")
    def ship(self, order):
        print("Already shipped.")
    def cancel(self, order):
        print("Cannot cancel — already shipped.")

class CancelledState(OrderState):
    def pay(self, order):
        print("Cannot pay — order cancelled.")
    def ship(self, order):
        print("Cannot ship — order cancelled.")
    def cancel(self, order):
        print("Already cancelled.")


class Order:
    def __init__(self):
        self.state: OrderState = PendingState()

    def pay(self): self.state.pay(self)
    def ship(self): self.state.ship(self)
    def cancel(self): self.state.cancel(self)
```

## Usage

```python
order = Order()
order.ship()   # Cannot ship — payment not received yet.
order.pay()    # Payment received.
order.ship()   # Order shipped.
order.cancel() # Cannot cancel — already shipped.
```

Every state's valid transitions and messages live together in one class — no giant
method with a wall of boolean-flag `if` guards.

## SOLID connections

- **OCP:** adding a `ReturnedState` for a return/refund workflow means writing one new
  class; existing states are untouched.
- **SRP:** each state class only knows about its own valid transitions.
- **Same structure as Strategy**, different intent: State represents an evolving
  lifecycle where the current implementation changes itself in response to events;
  Strategy represents an algorithm choice made once by the client.
