# Refactoring Technique: Replace Conditional with Polymorphism

This is the formal name (from Martin Fowler's refactoring catalog) for the
transformation used throughout `03-OCP-Open-Closed-Principle/`. It deserves its own
treatment because it's arguably the single highest-leverage refactoring for SOLID
compliance.

## When to apply it

Apply this when you see a conditional (`if/elif` or `switch`) that:

- branches on the *type* or *category* of an object, and
- that same type/category shows up in **multiple different functions** across the
  codebase (not just once).

If the conditional appears in exactly one place and is unlikely to grow, a simple
dictionary lookup or even leaving the `if/elif` alone may be perfectly fine — see the
"Rule of Three" note in the previous file.

## The transformation, step by step

**Before:**

```python
def shipping_cost(order):
    if order.type == "standard":
        return order.weight * 1.0
    elif order.type == "express":
        return order.weight * 2.5
    elif order.type == "overnight":
        return order.weight * 5.0

def delivery_estimate(order):
    if order.type == "standard":
        return "5-7 days"
    elif order.type == "express":
        return "2-3 days"
    elif order.type == "overnight":
        return "next day"
```

Notice the same `order.type` conditional is duplicated across two functions — a sign
this logic belongs on the type itself, not scattered across free functions.

**Step 1 — create a class hierarchy for the varying concept:**

```python
from abc import ABC, abstractmethod

class ShippingMethod(ABC):
    @abstractmethod
    def cost(self, weight): ...
    @abstractmethod
    def estimate(self): ...

class StandardShipping(ShippingMethod):
    def cost(self, weight): return weight * 1.0
    def estimate(self): return "5-7 days"

class ExpressShipping(ShippingMethod):
    def cost(self, weight): return weight * 2.5
    def estimate(self): return "2-3 days"

class OvernightShipping(ShippingMethod):
    def cost(self, weight): return weight * 5.0
    def estimate(self): return "next day"
```

**Step 2 — replace both conditional functions with delegation:**

```python
def shipping_cost(order):
    return order.shipping_method.cost(order.weight)

def delivery_estimate(order):
    return order.shipping_method.estimate()
```

Every future shipping method (e.g., "international") is now added as **one new
class**, and both `shipping_cost` and `delivery_estimate` automatically support it —
no conditional to update anywhere, and the two facts about each shipping method
(`cost` formula and `estimate` text) live next to each other instead of scattered
across the file.

## Why this technique also improves cohesion

Notice that before the refactor, "everything StandardShipping does" was scattered
across N different functions. After the refactor, it's colocated in one
`StandardShipping` class — a nice side effect of Replace Conditional with
Polymorphism is that it often *also* improves cohesion (SRP) for free.

Continue to `005-Replace-Inheritance-with-Composition.md`.
