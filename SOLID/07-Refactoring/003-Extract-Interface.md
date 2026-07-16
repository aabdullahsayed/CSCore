# Refactoring Technique: Extract Interface

**Extract Interface** is the mechanical technique behind fixing OCP, LSP, ISP, and DIP
violations: pull out an abstract contract that multiple concrete classes can
implement, or that a high-level module can depend on instead of a concrete class.

## Step-by-step process

1. **Identify the varying behavior** or the concrete dependency you want to decouple
   from.
2. **Define an abstract base class / interface** containing only the methods that
   callers actually need (tie this to ISP — don't just copy every method of the
   concrete class).
3. **Make the concrete class implement the new interface.**
4. **Change callers to depend on the interface type**, not the concrete class.
5. **Verify substitutability** — write at least one alternative implementation (even
   a fake/test double) to confirm the interface is genuinely abstract and not
   secretly tied to one implementation's quirks.

## Example: extracting an interface to fix a DIP violation

**Before:**

```python
class SqlOrderRepository:
    def save(self, order):
        print(f"INSERT INTO orders ... {order}")

class OrderService:
    def __init__(self):
        self.repo = SqlOrderRepository()   # concrete dependency

    def place_order(self, order):
        self.repo.save(order)
```

**After:**

```python
from abc import ABC, abstractmethod

class OrderRepository(ABC):
    @abstractmethod
    def save(self, order): ...

class SqlOrderRepository(OrderRepository):
    def save(self, order):
        print(f"INSERT INTO orders ... {order}")

class InMemoryOrderRepository(OrderRepository):   # proves substitutability
    def __init__(self):
        self.orders = []
    def save(self, order):
        self.orders.append(order)

class OrderService:
    def __init__(self, repo: OrderRepository):
        self.repo = repo

    def place_order(self, order):
        self.repo.save(order)
```

## Extract Interface for OCP: turning an `if/elif` into a Strategy

This is the same technique, applied to a branching function instead of a single
concrete class — see `03-OCP-Open-Closed-Principle/005-Refactoring.md` for a full
walkthrough. The mechanical steps are identical: define the interface, extract each
branch into a class implementing it, then replace the branch with delegation.

## Common mistake: extracting an interface too early or too eagerly

If you extract an interface with exactly one implementation and no plausible second
implementation on the horizon, and no testing need for a fake, you've added
indirection without benefit. A good practical rule: **wait until you have (or clearly
anticipate) a second real implementation, or a genuine need for a test double, before
extracting.** This is sometimes called the "Rule of Three" applied to abstractions.

Continue to `004-Replace-Conditionals.md`.
