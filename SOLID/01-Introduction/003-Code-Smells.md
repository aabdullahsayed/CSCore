# Code Smells That Signal SOLID Violations

A "code smell" is a surface indicator of a deeper design problem. You don't need to
name the violated principle to notice the smell — but knowing which principle it maps
to tells you how to fix it.

## 1. God Class / God Object

**Smell:** One class does everything — validates input, talks to the database, formats
output, sends emails.

**Maps to:** SRP violation.

```python
class UserManager:
    def validate_email(self, email): ...
    def save_to_database(self, user): ...
    def send_welcome_email(self, user): ...
    def generate_report(self, users): ...
```

## 2. Shotgun Surgery

**Smell:** A single conceptual change (e.g., "add a new discount type") requires
editing many files/classes scattered across the codebase.

**Maps to:** Often an OCP violation — the discount logic is implemented as a giant
`if/elif` chain duplicated in multiple places instead of being polymorphic.

## 3. Long Parameter Lists / Fat Interfaces

**Smell:** An interface has 15 methods, but any given implementer only really needs 3
of them and stubs out the rest with `raise NotImplementedError`.

**Maps to:** ISP violation.

## 4. Type-Checking / `isinstance` Chains

**Smell:**

```python
if isinstance(shape, Circle):
    area = 3.14 * shape.radius ** 2
elif isinstance(shape, Square):
    area = shape.side ** 2
```

**Maps to:** OCP violation (adding a new shape means editing this function) and often
signals a missing polymorphic method (`shape.area()`).

## 5. Overridden Methods That Do Nothing or Throw

**Smell:**

```python
class Penguin(Bird):
    def fly(self):
        raise NotImplementedError("Penguins can't fly")
```

**Maps to:** Classic LSP violation — the subclass breaks a contract implied by the
parent (every `Bird` can `fly()`).

## 6. `new` Scattered Everywhere / Hard-Coded Concrete Classes

**Smell:**

```python
class OrderService:
    def __init__(self):
        self.db = MySQLDatabase()  # concrete class instantiated directly
```

**Maps to:** DIP violation — high-level `OrderService` is wired directly to a
low-level, concrete detail instead of an abstraction.

## 7. Feature Envy

**Smell:** A method in class `A` mostly calls getters on class `B` and barely touches
its own class's data. Often a sign responsibilities are misplaced (related to SRP).

## 8. Rigid Inheritance for Code Reuse Only

**Smell:** `class ElectricCar(Car)` purely to reuse `Car`'s `start_engine()` method,
even though an electric car has no combustion engine.

**Maps to:** Signals you should favor composition over inheritance — see
`005-Composition-vs-Inheritance.md`.

## Using this list

When reviewing code (yours or a teammate's), scan for these smells first. They are
much easier to spot than reciting principle definitions, and each smell points you
directly at the fix.
