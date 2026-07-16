# SRP: Definition

> "A class should have only one reason to change."
> — Robert C. Martin

## Unpacking "reason to change"

The tricky part of SRP is that "one reason to change" doesn't mean "one method" — a
class can have many methods and still satisfy SRP, as long as all those methods serve
a single actor or purpose. Uncle Bob later refined this with the phrase:

> "A module should be responsible to one, and only one, actor."

An **actor** here means a person or group of people who could request a change: the
finance team, the DBA, the UX designer, the compliance officer, etc. If two different
actors could each independently demand a change to the same class, that class has more
than one responsibility and should be split.

## Why "responsibility" is about actors, not method count

```python
class Employee:
    def calculate_pay(self):       # Finance/payroll team cares about this
        ...

    def save(self):                # DBA / infrastructure team cares about this
        ...

    def report_hours(self):        # HR team cares about this
        ...
```

Even though this is "one class," it answers to three different actors. A change to
tax law affects `calculate_pay`, a change to the database schema affects `save`, and a
change to HR reporting requirements affects `report_hours`. Three unrelated teams can
now each cause an unrelated regression in the same file.

## SRP at different scales

SRP applies fractally:

- **Function level:** a function should do one logical thing.
- **Class level:** a class should serve one actor.
- **Module/package level:** a module should encapsulate one area of concern.
- **Microservice level:** a service should own one bounded context.

## What SRP is *not*

- It does **not** mean "a class should have only one method."
- It does **not** mean "a class should be tiny." A class can have 20 methods and still
  be SRP-compliant if all 20 exist to serve the same responsibility (e.g., a `Matrix`
  class with many math operations, all serving "matrix math").
- It is **not** about DRY (duplication). SRP is about cohesion of *purpose*, not about
  avoiding repeated code.

## Signal to watch for

If you can only describe a class's job using "and" ("this class validates orders
**and** saves them **and** emails a receipt"), it likely violates SRP.

Continue to `002-Bad-Example.md` to see this play out in code.
