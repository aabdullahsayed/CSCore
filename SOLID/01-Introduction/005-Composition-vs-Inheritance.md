# Composition vs Inheritance

"Favor composition over inheritance" is one of the most quoted pieces of OO advice,
and it's deeply tied to LSP and OCP. This file explains why.

## The trap of inheritance-for-reuse

Inheritance is tempting because it gives you code reuse "for free" — subclass
something, get all its methods. But it couples the subclass to the *implementation*
of the parent, not just its interface. That coupling is exactly what causes LSP
violations.

```python
class Bird:
    def fly(self):
        print("Flying!")

class Penguin(Bird):      # Penguin IS-A Bird biologically...
    def fly(self):
        raise Exception("Penguins can't fly!")   # ...but violates the Bird contract
```

The moment you need to override a method to throw an exception or do nothing, that's
a signal the hierarchy doesn't actually model "is-a" correctly for your program's
purposes.

## Composition: build behavior out of parts

Instead of inheriting behavior, *inject* it as a collaborator.

```python
class FlyingAbility:
    def move(self):
        print("Flying!")

class SwimmingAbility:
    def move(self):
        print("Swimming!")

class Bird:
    def __init__(self, movement):
        self.movement = movement   # composed, not inherited

    def move(self):
        self.movement.move()

sparrow = Bird(FlyingAbility())
penguin = Bird(SwimmingAbility())
sparrow.move()   # Flying!
penguin.move()   # Swimming!
```

Now there is no invalid state representable — a `Penguin` was never forced to inherit
a `fly()` method it can't honor.

## When inheritance is still the right tool

Inheritance is appropriate when:

- The subtype is a strict behavioral refinement of the supertype (true LSP
  compliance) — e.g., `SavingsAccount` and `CheckingAccount` both fully honor the
  `Account` contract.
- You want to share a small, stable set of behavior across a *closed*, well-understood
  set of variants (e.g., framework base classes like `unittest.TestCase`).
- You're modeling a genuine type hierarchy where all methods make sense for every
  subtype, with no need to "opt out" of any inherited behavior.

## Rule of thumb

> Use inheritance to model **"is a kind of, and behaves exactly like"**.
> Use composition to model **"is built out of, or has the ability to"**.

If you find yourself overriding a method just to disable it, or writing
`isinstance()` checks to special-case a subclass, that's the signal to refactor
toward composition — which is exactly the pattern behind the **Strategy** and
**Decorator** design patterns covered in `08-Design-Patterns/`.
