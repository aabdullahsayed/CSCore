# Design Pattern: Builder

## Intent

Separate the construction of a complex object from its representation, so the same
construction process can create different representations — and so that complex,
multi-step object creation doesn't clutter the object's own constructor.

## The problem: telescoping constructors

```python
class Pizza:
    def __init__(self, size, cheese=True, pepperoni=False, mushrooms=False,
                 olives=False, extra_sauce=False, thin_crust=False):
        # a constructor with 7 parameters, mostly optional, is error-prone to call
        ...
```

Calling `Pizza(12, True, False, True, False, True, False)` is unreadable and easy to
get wrong (which boolean means what?).

## The Builder pattern

```python
class Pizza:
    def __init__(self):
        self.size = None
        self.toppings = []
        self.thin_crust = False

    def __str__(self):
        return f"{self.size}\" pizza, thin_crust={self.thin_crust}, toppings={self.toppings}"


class PizzaBuilder:
    def __init__(self):
        self.pizza = Pizza()

    def set_size(self, size):
        self.pizza.size = size
        return self   # enables method chaining

    def add_topping(self, topping):
        self.pizza.toppings.append(topping)
        return self

    def set_thin_crust(self, thin=True):
        self.pizza.thin_crust = thin
        return self

    def build(self) -> Pizza:
        return self.pizza
```

## Usage

```python
pizza = (
    PizzaBuilder()
    .set_size(12)
    .add_topping("pepperoni")
    .add_topping("mushrooms")
    .set_thin_crust()
    .build()
)
print(pizza)
```

This reads almost like a sentence, and every step is self-documenting — no ambiguous
positional booleans.

## SOLID connections

- **SRP:** `Pizza` only represents pizza data; `PizzaBuilder` owns the *process* of
  assembling one step at a time. Two different responsibilities, two classes.
- **OCP:** adding a new pizza attribute (e.g., `set_gluten_free()`) means adding a new
  builder method — existing client code that doesn't need it is unaffected.

## Director (optional refinement)

For builders with common, reusable construction recipes, a `Director` class can
encapsulate specific build sequences:

```python
class PizzaDirector:
    @staticmethod
    def build_margherita(builder: PizzaBuilder) -> Pizza:
        return builder.set_size(10).add_topping("mozzarella").add_topping("basil").build()

margherita = PizzaDirector.build_margherita(PizzaBuilder())
```

## When to use Builder

Use it when an object has many optional parameters, when construction involves
multiple ordered steps, or when you want to produce different representations
(e.g., a `PizzaBuilder` and a `PastaBuilder` both implementing a common
`MealBuilder` interface) from a similar assembly process. Skip it for simple objects
with 1-3 straightforward constructor parameters.
