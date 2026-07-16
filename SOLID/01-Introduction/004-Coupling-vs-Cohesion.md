# Coupling vs Cohesion

These two concepts are the "physics" underneath all five SOLID principles. Every
principle is really just a specific strategy for **increasing cohesion** and
**decreasing coupling**.

## Cohesion — how focused is a single module?

Cohesion measures how closely the responsibilities inside one class or module relate
to each other. **High cohesion** = everything in the class exists to serve one clear
purpose.

```python
# LOW cohesion: unrelated responsibilities crammed together
class Utils:
    def format_date(self, d): ...
    def calculate_tax(self, amount): ...
    def send_email(self, to, body): ...
    def parse_csv(self, path): ...
```

```python
# HIGH cohesion: each class has one job
class DateFormatter:
    def format(self, d): ...

class TaxCalculator:
    def calculate(self, amount): ...
```

High cohesion is the direct goal of the **Single Responsibility Principle**.

## Coupling — how entangled are two modules?

Coupling measures how much one module knows about, or depends on, the internal
details of another. **Low coupling** = modules interact through small, stable
interfaces and don't need to know each other's internals.

```python
# TIGHT coupling: OrderService knows the concrete MySQL class
class OrderService:
    def __init__(self):
        self.db = MySQLDatabase()
```

```python
# LOOSE coupling: OrderService only knows an abstraction
class OrderService:
    def __init__(self, db: Database):  # Database is an interface/ABC
        self.db = db
```

Loose coupling is the direct goal of the **Dependency Inversion Principle**, and is
also what makes the **Open/Closed** and **Liskov Substitution** principles achievable
in practice — you can only swap implementations safely if the coupling is loose to
begin with.

## Why they trade off together, not against each other

A common misconception is that cohesion and coupling are two ends of the same dial.
They are not — you want **high cohesion AND low coupling simultaneously**:

| | Low coupling | High coupling |
|---|---|---|
| **High cohesion** | Ideal: focused classes, clean interfaces between them | Focused classes but tangled dependencies |
| **Low cohesion** | Independent classes, but each is a junk drawer | Worst of both worlds |

## A quick mental test

Ask two questions about any class:

1. **Cohesion check:** "If I described this class's job in one sentence, would I need
   to use the word 'and'?" If yes, it's probably doing too much.
2. **Coupling check:** "If I swapped out one of this class's dependencies for a
   different implementation, how many other files would I need to touch?" The more
   files, the tighter the coupling.

These two questions will get you 80% of the way to spotting SOLID violations without
memorizing formal definitions.
