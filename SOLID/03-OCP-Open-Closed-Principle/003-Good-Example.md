# OCP: Good Example

We fix the discount calculator from `002-Bad-Example.md` using polymorphism, so new
discount tiers can be added without touching existing code.

```python
from abc import ABC, abstractmethod

class DiscountStrategy(ABC):
    @abstractmethod
    def apply(self, amount: float) -> float: ...

class RegularDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.95

class PremiumDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.90

class VipDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.80

class DiscountCalculator:
    def calculate(self, strategy: DiscountStrategy, amount: float) -> float:
        return strategy.apply(amount)
```

## Adding a new tier — the whole point of OCP

```python
class StudentDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.85
```

Notice: **zero existing files were modified.** `DiscountCalculator`,
`RegularDiscount`, `PremiumDiscount`, and `VipDiscount` are completely untouched.
`StudentDiscount` is a brand-new file that plugs into the existing system because it
honors the same `DiscountStrategy` interface.

## Usage

```python
calculator = DiscountCalculator()
total = calculator.calculate(VipDiscount(), 100)   # 80.0
total = calculator.calculate(StudentDiscount(), 100)  # 85.0
```

The mapping from "customer type" to "strategy instance" (e.g. a dictionary lookup or a
factory) lives in exactly one place and is the only part of the system that ever needs
a small addition when a new tier launches — everything else is closed.

## What we gained

- **No regression risk** for existing discount tiers when adding a new one.
- **Parallel development:** two engineers can each add a new discount type in separate
  files without any merge conflict.
- **Isolated testing:** `StudentDiscount` can be unit tested completely on its own.
- **This is literally the Strategy pattern** — see `08-Design-Patterns/004-Strategy.md`
  for the formalized version of exactly this technique.

Continue to `004-Strategy-Pattern.md` to see this generalized further.
