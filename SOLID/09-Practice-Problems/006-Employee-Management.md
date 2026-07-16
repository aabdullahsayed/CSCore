# Practice Problem: Employee Management

## The problem

```python
class Employee:
    def __init__(self, name, emp_type, base_salary, sales=0):
        self.name = name
        self.emp_type = emp_type
        self.base_salary = base_salary
        self.sales = sales

    def calculate_pay(self):
        if self.emp_type == "salaried":
            return self.base_salary
        elif self.emp_type == "hourly":
            return self.base_salary * 160  # hours in a month
        elif self.emp_type == "commissioned":
            return self.base_salary + (self.sales * 0.05)
        else:
            raise ValueError("Unknown employee type")
```

This is an OCP violation (branching on `emp_type`) that also mixes very different
concepts (a salaried base pay, an hourly rate, a commission calculation) into one
formula, making the class fragile as payroll rules evolve.

## Requirements

1. New employee types (e.g., "intern," "contractor" with different pay rules) must not
   require editing existing code.
2. Each pay-calculation rule should be independently unit-testable.

## Suggested solution

```python
from abc import ABC, abstractmethod

class PayStrategy(ABC):
    @abstractmethod
    def calculate(self, base: float, sales: float = 0) -> float: ...

class SalariedPay(PayStrategy):
    def calculate(self, base, sales=0):
        return base

class HourlyPay(PayStrategy):
    def calculate(self, base, sales=0):
        return base * 160

class CommissionedPay(PayStrategy):
    def calculate(self, base, sales=0):
        return base + (sales * 0.05)


class Employee:
    def __init__(self, name, base_salary, pay_strategy: PayStrategy, sales=0):
        self.name = name
        self.base_salary = base_salary
        self.pay_strategy = pay_strategy
        self.sales = sales

    def calculate_pay(self):
        return self.pay_strategy.calculate(self.base_salary, self.sales)
```

## Usage

```python
alice = Employee("Alice", 5000, SalariedPay())
bob = Employee("Bob", 25, HourlyPay())
carol = Employee("Carol", 2000, CommissionedPay(), sales=10000)

for emp in [alice, bob, carol]:
    print(emp.name, emp.calculate_pay())
```

Adding `InternPay` (perhaps a fixed stipend plus a cap) is a new class — `Employee`
never changes, and each pay rule can be unit-tested with plain numbers, no `Employee`
instance required.
