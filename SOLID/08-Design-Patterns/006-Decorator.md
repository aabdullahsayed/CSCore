# Design Pattern: Decorator

## Intent

Attach additional responsibilities to an object dynamically, without modifying its
class or affecting other instances of the same class — an alternative to subclassing
for extending behavior, keeping designs OCP-compliant.

## The problem: combinatorial subclassing

Imagine a `Coffee` with optional add-ons: milk, sugar, whipped cream. Modeling every
combination via subclassing explodes:

```python
class Coffee: ...
class CoffeeWithMilk(Coffee): ...
class CoffeeWithSugar(Coffee): ...
class CoffeeWithMilkAndSugar(Coffee): ...
class CoffeeWithMilkAndSugarAndCream(Coffee): ...
# ... combinatorial explosion
```

## The Decorator pattern

```python
from abc import ABC, abstractmethod

class Beverage(ABC):
    @abstractmethod
    def cost(self) -> float: ...
    @abstractmethod
    def description(self) -> str: ...

class Coffee(Beverage):
    def cost(self): return 2.00
    def description(self): return "Coffee"


class BeverageDecorator(Beverage):
    """Base decorator: wraps a Beverage and honors the same interface."""
    def __init__(self, beverage: Beverage):
        self._beverage = beverage

class MilkDecorator(BeverageDecorator):
    def cost(self): return self._beverage.cost() + 0.50
    def description(self): return self._beverage.description() + " + Milk"

class SugarDecorator(BeverageDecorator):
    def cost(self): return self._beverage.cost() + 0.25
    def description(self): return self._beverage.description() + " + Sugar"

class WhippedCreamDecorator(BeverageDecorator):
    def cost(self): return self._beverage.cost() + 0.75
    def description(self): return self._beverage.description() + " + Whipped Cream"
```

## Usage — combine freely at runtime, no subclass explosion

```python
order = WhippedCreamDecorator(SugarDecorator(MilkDecorator(Coffee())))
print(order.description(), "->", f"${order.cost():.2f}")
# Coffee + Milk + Sugar + Whipped Cream -> $3.50
```

Any combination of add-ons is achieved by *composing* decorators at runtime — no new
class is needed for "Coffee with Milk and Sugar" specifically, because
`MilkDecorator` and `SugarDecorator` can simply be nested around each other.

## SOLID connections

- **OCP:** adding a new add-on (`VanillaDecorator`) is one new class; no existing
  `Beverage` or decorator class is touched.
- **LSP:** every decorator fully honors the `Beverage` interface, so decorated
  beverages are substitutable anywhere a plain `Beverage` is expected — this is
  what makes the nesting/wrapping safe in the first place.

## Real-world instances

Python's built-in `@decorator` function syntax is a language-level cousin of this
pattern (though technically a different mechanism). Java's `java.io` stream classes
(`BufferedInputStream` wrapping `FileInputStream`) are a canonical textbook example of
this exact object-oriented pattern.
