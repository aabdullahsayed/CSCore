# Practice Problem: Calculator

## The problem

Refactor this calculator to be SOLID-compliant. It currently violates OCP (branching
on operator string) and mixes parsing, computing, and display logic together.

```python
class Calculator:
    def calculate(self, a, b, operator):
        if operator == "+":
            result = a + b
        elif operator == "-":
            result = a - b
        elif operator == "*":
            result = a * b
        elif operator == "/":
            result = a / b
        else:
            raise ValueError("Unknown operator")
        print(f"{a} {operator} {b} = {result}")
        return result
```

## Requirements for your solution

1. Adding a new operator (e.g., exponentiation `^`) must not require editing any
   existing class.
2. Printing/display logic must be separated from the arithmetic logic (SRP).
3. Division by zero should raise a clear error, not crash with a raw
   `ZeroDivisionError`.

## Suggested solution

```python
from abc import ABC, abstractmethod

class Operation(ABC):
    @abstractmethod
    def apply(self, a: float, b: float) -> float: ...

class Add(Operation):
    def apply(self, a, b): return a + b

class Subtract(Operation):
    def apply(self, a, b): return a - b

class Multiply(Operation):
    def apply(self, a, b): return a * b

class Divide(Operation):
    def apply(self, a, b):
        if b == 0:
            raise ValueError("Cannot divide by zero")
        return a / b


class Calculator:
    def __init__(self):
        self._operations = {"+": Add(), "-": Subtract(), "*": Multiply(), "/": Divide()}

    def register(self, symbol: str, operation: Operation):
        self._operations[symbol] = operation

    def calculate(self, a, b, operator) -> float:
        if operator not in self._operations:
            raise ValueError(f"Unknown operator: {operator}")
        return self._operations[operator].apply(a, b)


class CalculatorPrinter:
    def display(self, a, b, operator, result):
        print(f"{a} {operator} {b} = {result}")
```

## Usage

```python
calc = Calculator()
calc.register("^", type("Power", (Operation,), {"apply": lambda self, a, b: a ** b})())
result = calc.calculate(2, 3, "^")

CalculatorPrinter().display(2, 3, "^", result)
```

Adding `^` required zero edits to `Calculator` or any existing `Operation` class —
only a new registration call.
