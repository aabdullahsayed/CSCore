# DIP: Definition

> "High-level modules should not depend on low-level modules. Both should depend on
> abstractions. Abstractions should not depend on details. Details should depend on
> abstractions."
> — Robert C. Martin

## Two separate claims bundled into one principle

1. **Direction of dependency:** high-level policy code (business logic) should not
   directly depend on low-level implementation details (databases, file systems,
   third-party APIs). Instead, both should depend on a shared abstraction
   (interface).
2. **The "inversion":** normally you'd think low-level modules are the "foundation"
   that high-level modules are built on top of, so dependencies point downward, from
   high-level to low-level. DIP *inverts* this — the abstraction is defined by (and
   owned by) the high-level module, and the low-level module depends on/implements
   that abstraction instead.

## A minimal illustration of the inversion

**Without DIP** — high-level depends directly on low-level:

```python
class MySQLDatabase:
    def save(self, data):
        print(f"Saving to MySQL: {data}")

class OrderService:               # high-level policy
    def __init__(self):
        self.db = MySQLDatabase()  # directly depends on a low-level detail
    def place_order(self, order):
        self.db.save(order)
```

`OrderService` (business logic) is tightly coupled to `MySQLDatabase` (an
infrastructure detail). If you switch databases, or want to unit test `OrderService`
without a real database, you're stuck.

**With DIP** — both depend on an abstraction:

```python
from abc import ABC, abstractmethod

class Database(ABC):                    # the abstraction, owned by the high-level layer
    @abstractmethod
    def save(self, data): ...

class MySQLDatabase(Database):           # low-level detail, implements the abstraction
    def save(self, data):
        print(f"Saving to MySQL: {data}")

class OrderService:                      # high-level policy
    def __init__(self, db: Database):    # depends only on the abstraction
        self.db = db
    def place_order(self, order):
        self.db.save(order)
```

Now `MySQLDatabase` depends on `Database` (an abstraction), and `OrderService` also
depends only on `Database` — neither depends on the other directly. This is the
"inversion": the low-level `MySQLDatabase` now has to conform to an interface shaped
by the needs of the high-level `OrderService`, not the other way around.

## DIP is not the same as Dependency Injection

This is a very common confusion:

- **Dependency Inversion Principle (DIP)** is a *design principle*: depend on
  abstractions, not concretions.
- **Dependency Injection (DI)** is a *technique*: passing dependencies into a class
  (via constructor, setter, etc.) instead of having the class construct them itself.

DI is the most common *mechanism* used to satisfy DIP (as shown above — `OrderService`
receives its `Database` via the constructor), but you could satisfy DIP without a DI
framework, and you could use dependency injection while still violating DIP (e.g.
injecting a concrete `MySQLDatabase` type instead of the abstract `Database`).

See `004-Constructor-Injection.md` and `005-Manual-Dependency-Injection.md` for more.

Continue to `002-High-vs-Low-Level.md`.
