# LSP: The Rectangle/Square Problem

This is the single most famous LSP example, because it shows that **mathematical
"is-a" relationships don't always map to safe "is-a" relationships in code.**

## The intuition trap

Mathematically, "every square is a rectangle" is unambiguously true. So it feels
natural to model it with inheritance:

```python
class Rectangle:
    def __init__(self, width, height):
        self._width = width
        self._height = height

    @property
    def width(self): return self._width

    @width.setter
    def width(self, value): self._width = value

    @property
    def height(self): return self._height

    @height.setter
    def height(self, value): self._height = value

    def area(self):
        return self._width * self._height


class Square(Rectangle):
    def __init__(self, size):
        super().__init__(size, size)

    @Rectangle.width.setter
    def width(self, value):
        self._width = value
        self._height = value   # must keep it a square!

    @Rectangle.height.setter
    def height(self, value):
        self._width = value
        self._height = value
```

## Where it breaks

```python
def test_rectangle_resizing(rect: Rectangle):
    rect.width = 5
    rect.height = 4
    assert rect.area() == 20, "Area should reflect independent width/height"

test_rectangle_resizing(Rectangle(2, 2))  # passes: area = 20
test_rectangle_resizing(Square(2))        # FAILS: area = 16, not 20
```

Any code that trusts "setting width doesn't affect height" (a completely reasonable
assumption for a `Rectangle`) is silently broken when handed a `Square`. The `Square`
subclass **cannot honor the full behavioral contract** of `Rectangle`, even though it
satisfies the *data* relationship perfectly.

## The real lesson

> Inheritance should model **behavioral** substitutability, not just conceptual
> "is-a" categorization from the real world or from mathematics.

`Square` fails LSP here specifically because `Rectangle` exposes **mutable,
independent width and height** as part of its public contract. If `Rectangle` were
immutable (constructed once, never resized), this specific problem disappears
entirely — because the violated behavior (independent mutation) wouldn't exist as
part of the contract in the first place.

## Two valid fixes

**Fix 1 — Make both immutable:**

```python
class Rectangle:
    def __init__(self, width, height):
        self._width = width
        self._height = height
    def area(self):
        return self._width * self._height

class Square(Rectangle):
    def __init__(self, size):
        super().__init__(size, size)
```

Now there's no mutation to violate — `Square` genuinely satisfies every promise
`Rectangle` makes (construct once, ask for area).

**Fix 2 — Don't use inheritance at all; use a shared interface:**

```python
from abc import ABC, abstractmethod

class Shape(ABC):
    @abstractmethod
    def area(self): ...

class Rectangle(Shape):
    def __init__(self, width, height):
        self.width, self.height = width, height
    def area(self):
        return self.width * self.height

class Square(Shape):
    def __init__(self, size):
        self.size = size
    def area(self):
        return self.size ** 2
```

This sidesteps the whole problem — `Square` never claims to *be* a `Rectangle`, so it
never inherits a contract it can't fully honor.

Continue to `003-Bad-Example.md` for another common LSP violation pattern (the "bird
that can't fly").
