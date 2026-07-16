# Practice Problem: Shapes Area Calculator

## The problem

```python
class AreaCalculator:
    def total_area(self, shapes):
        total = 0
        for shape in shapes:
            if shape["type"] == "circle":
                total += 3.14159 * shape["radius"] ** 2
            elif shape["type"] == "rectangle":
                total += shape["width"] * shape["height"]
            elif shape["type"] == "triangle":
                total += 0.5 * shape["base"] * shape["height"]
        return total
```

This uses untyped dictionaries and an `if/elif` chain keyed on a `"type"` string — a
classic OCP violation, and fragile (typos in dict keys fail silently or crash at
runtime).

## Requirements

1. Adding a new shape must not require editing `AreaCalculator`.
2. Each shape should validate its own parameters (e.g., radius/width/height must be
   positive).

## Suggested solution

```python
from abc import ABC, abstractmethod

class Shape(ABC):
    @abstractmethod
    def area(self) -> float: ...

class Circle(Shape):
    def __init__(self, radius):
        if radius <= 0:
            raise ValueError("Radius must be positive")
        self.radius = radius
    def area(self):
        return 3.14159 * self.radius ** 2

class Rectangle(Shape):
    def __init__(self, width, height):
        if width <= 0 or height <= 0:
            raise ValueError("Width and height must be positive")
        self.width = width
        self.height = height
    def area(self):
        return self.width * self.height

class Triangle(Shape):
    def __init__(self, base, height):
        if base <= 0 or height <= 0:
            raise ValueError("Base and height must be positive")
        self.base = base
        self.height = height
    def area(self):
        return 0.5 * self.base * self.height


class AreaCalculator:
    def total_area(self, shapes: list[Shape]) -> float:
        return sum(shape.area() for shape in shapes)
```

## Usage

```python
shapes = [Circle(3), Rectangle(4, 5), Triangle(6, 2)]
print(AreaCalculator().total_area(shapes))
```

Adding `Pentagon` means writing one new `Shape` subclass — `AreaCalculator` never
changes, and every shape validates its own construction, which is also a natural SRP
win (validation logic lives with the data it validates).
