# LSP: Definition

> "Objects of a superclass shall be replaceable with objects of its subclasses without
> breaking the application."
> — Barbara Liskov, 1987 (formalized with Jeannette Wing, 1994)

## The formal version

If `S` is a subtype of `T`, then objects of type `T` in a program may be replaced with
objects of type `S` **without altering any of the desirable properties of that
program** (correctness, task performed, etc).

In plain terms: **any code written against a base class/interface must continue to
work correctly if you hand it any subclass instance instead.**

## What "breaking the contract" really means

A subclass violates LSP when it:

1. **Strengthens preconditions** — requires more from the caller than the base class
   promised (e.g., base class accepts any integer, subclass throws on negative
   numbers).
2. **Weakens postconditions** — returns/guarantees less than the base class promised.
3. **Changes expected behavior** — e.g., throws an exception for a method the base
   class documented as always succeeding.
4. **Violates invariants** — breaks a rule the base class guaranteed always holds
   (e.g., "balance is never negative").

## A minimal illustration

```python
class Rectangle:
    def __init__(self, width, height):
        self.width = width
        self.height = height

    def set_width(self, w):
        self.width = w

    def set_height(self, h):
        self.height = h

    def area(self):
        return self.width * self.height


class Square(Rectangle):
    def set_width(self, w):
        self.width = w
        self.height = w   # forces height to match — surprising side effect!

    def set_height(self, h):
        self.width = h
        self.height = h
```

Code written against `Rectangle` reasonably assumes `set_width` only changes width:

```python
def resize(rect: Rectangle):
    rect.set_width(5)
    rect.set_height(4)
    assert rect.area() == 20   # true for Rectangle, FALSE for Square!
```

Passing a `Square` here silently breaks an assumption the calling code relied on —
this is a textbook LSP violation, covered in depth in `002-Rectangle-Square-Problem.md`.

## Why LSP matters more than it seems

LSP is what makes polymorphism *trustworthy*. Without it, every time you see a
function accepting a base type, you'd have to open every possible subclass to check
whether it secretly behaves differently — defeating the entire point of abstraction.
OCP (writing code against interfaces so new types can be added later) is only safe to
rely on **if** LSP holds for every implementation.

Continue to `002-Rectangle-Square-Problem.md` for the deepest dive into the classic
example.
