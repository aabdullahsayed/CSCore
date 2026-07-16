# OCP: Definition

> "Software entities (classes, modules, functions) should be open for extension, but
> closed for modification."
> — Bertrand Meyer (1988), popularized by Robert C. Martin

## Unpacking "open" and "closed"

- **Open for extension:** you should be able to add new behavior to a system.
- **Closed for modification:** you should be able to add that new behavior **without
  editing existing, already-tested source code**.

At first these sound contradictory — how can you add behavior without touching
anything? The resolution is **polymorphism and abstraction**: you write code against
an interface, and add new behavior by writing a *new class* that implements that
interface, rather than adding a new branch to an existing function.

## The canonical smell OCP fixes

```python
def calculate_area(shape):
    if shape.type == "circle":
        return 3.14 * shape.radius ** 2
    elif shape.type == "square":
        return shape.side ** 2
    # Adding a triangle means opening this function and editing it
```

Every new shape means editing `calculate_area`. That function is never "done" — it's
permanently a modification target, and every edit risks breaking the existing,
already-tested branches.

## The OCP-compliant shape

```python
from abc import ABC, abstractmethod

class Shape(ABC):
    @abstractmethod
    def area(self): ...

class Circle(Shape):
    def __init__(self, radius):
        self.radius = radius
    def area(self):
        return 3.14 * self.radius ** 2

class Square(Shape):
    def __init__(self, side):
        self.side = side
    def area(self):
        return self.side ** 2

def calculate_area(shape: Shape):
    return shape.area()
```

Adding a `Triangle` now means writing a *new class* — `calculate_area` never changes
again, and every previously tested shape is untouched by the new addition.

## Why "closed for modification" matters so much

Every time you edit an existing, working, tested function, you introduce **regression
risk**. OCP-compliant designs let you add features via new code, which:

- Cannot break existing behavior (nothing else changed).
- Doesn't require re-testing or re-reviewing unrelated code paths.
- Can be developed and deployed independently (even by a different team).

## What OCP is *not*

- It doesn't mean *literally never* touching a file again. Bug fixes, and changes to
  requirements for existing behavior, legitimately require modification.
- It doesn't mean wrapping *everything* in abstractions "just in case" — see
  `005-Refactoring.md` and `006-Exercises.md` for guidance on when to apply it.

Continue to `002-Bad-Example.md`.
