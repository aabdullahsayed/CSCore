# Design Pattern: Strategy

Strategy is covered in depth in `03-OCP-Open-Closed-Principle/004-Strategy-Pattern.md`
as the primary implementation vehicle for OCP. This file gives the formal GoF
definition and a second worked example for reinforcement.

## Formal intent

Define a family of algorithms, encapsulate each one, and make them interchangeable.
Strategy lets the algorithm vary independently from the clients that use it.

## Structure

- **Strategy** (interface): declares a common operation for all supported algorithms.
- **ConcreteStrategy** (one class per algorithm): implements the operation.
- **Context**: holds a reference to a Strategy object and delegates to it, without
  knowing which concrete strategy it holds.

## Worked example: route calculation

```python
from abc import ABC, abstractmethod

class RouteStrategy(ABC):
    @abstractmethod
    def build_route(self, start, end) -> str: ...

class DrivingRoute(RouteStrategy):
    def build_route(self, start, end):
        return f"Driving route from {start} to {end}"

class WalkingRoute(RouteStrategy):
    def build_route(self, start, end):
        return f"Walking route from {start} to {end}"

class PublicTransitRoute(RouteStrategy):
    def build_route(self, start, end):
        return f"Transit route from {start} to {end}"


class Navigator:
    def __init__(self, strategy: RouteStrategy):
        self.strategy = strategy

    def set_strategy(self, strategy: RouteStrategy):
        self.strategy = strategy   # can swap strategies at runtime

    def navigate(self, start, end):
        return self.strategy.build_route(start, end)
```

```python
nav = Navigator(DrivingRoute())
print(nav.navigate("Home", "Work"))

nav.set_strategy(WalkingRoute())   # swap algorithm at runtime
print(nav.navigate("Home", "Work"))
```

## Strategy vs hardcoded conditionals — the direct OCP payoff

Adding `BikeRoute` requires writing one new class and zero edits to `Navigator` — this
is the exact mechanism that makes OCP achievable in real code.

## Strategy vs State pattern (see `009-State.md`)

Structurally nearly identical — both hold a reference to an interchangeable interface
implementation. The difference is *intent*: Strategy is chosen once by the client for
a specific call/algorithm choice; State represents an object transitioning through a
lifecycle where the "current strategy" changes automatically in response to events
(e.g., an order moving from `Pending` → `Shipped` → `Delivered`).
