# OCP via the Strategy Pattern

The Strategy pattern is the most direct, general-purpose tool for satisfying OCP. The
discount example in `003-Good-Example.md` *is* the Strategy pattern — this file
generalizes the technique.

## The shape of the pattern

1. Define an interface (abstract base class / protocol) representing "a way of doing
   X."
2. Implement each variant as its own class that satisfies that interface.
3. The context/client class holds a *reference* to the interface, never to a concrete
   variant, and delegates the actual work to whichever strategy it's holding.

```python
from abc import ABC, abstractmethod

class SortStrategy(ABC):
    @abstractmethod
    def sort(self, data: list) -> list: ...

class QuickSort(SortStrategy):
    def sort(self, data):
        # ... quicksort implementation
        return sorted(data)  # simplified

class MergeSort(SortStrategy):
    def sort(self, data):
        # ... merge sort implementation
        return sorted(data)  # simplified

class Sorter:
    def __init__(self, strategy: SortStrategy):
        self.strategy = strategy

    def sort(self, data):
        return self.strategy.sort(data)
```

Swapping algorithms is now a matter of passing a different object in, not editing
`Sorter`:

```python
Sorter(QuickSort()).sort([5, 2, 4])
Sorter(MergeSort()).sort([5, 2, 4])
```

## Real-world use cases

- **Payment processing:** `CreditCardStrategy`, `PayPalStrategy`, `CryptoStrategy` all
  implementing a `PaymentStrategy.pay(amount)` interface.
- **Compression:** `ZipStrategy`, `GzipStrategy`, `Bzip2Strategy` implementing a
  `CompressionStrategy.compress(data)` interface.
- **Validation rules:** a list of `ValidationRule` objects run against input, where
  new business rules are added as new classes rather than new `if` branches.
- **Pricing/discount engines**, **routing algorithms**, **notification channels**
  (email/SMS/push) — anywhere you see "pick one of several interchangeable
  algorithms," Strategy is usually the right OCP tool.

## Strategy vs simple functions

In languages with first-class functions (Python, JavaScript), you can implement
Strategy with plain functions instead of classes when the strategy has no internal
state:

```python
def quicksort(data): return sorted(data)
def mergesort(data): return sorted(data)

class Sorter:
    def __init__(self, strategy_fn):
        self.strategy_fn = strategy_fn
    def sort(self, data):
        return self.strategy_fn(data)
```

Both are valid — use a class-based strategy when the algorithm needs configuration or
internal state; use a plain function when it doesn't.

## Relationship to other patterns

Strategy is closely related to but distinct from:
- **State pattern** (`08-Design-Patterns/009-State.md`) — same structure, but the
  "current strategy" changes over the object's lifetime in response to events.
- **Template Method** (`08-Design-Patterns/010-Template-Method.md`) — uses inheritance
  to vary a *part* of an algorithm rather than swapping the whole algorithm via
  composition.

Continue to `005-Refactoring.md` to practice converting an `if/elif` chain into
Strategy step by step.
