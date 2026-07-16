# Refactoring Technique: Replace Inheritance with Composition

This technique fixes LSP violations that stem from misusing inheritance purely for
code reuse (see `01-Introduction/005-Composition-vs-Inheritance.md` for the
conceptual background). This file focuses on the mechanical refactoring steps.

## When to apply it

Apply this when:
- A subclass overrides a method to throw, no-op, or behave in a surprising way
  relative to the base class's contract.
- You find yourself writing `isinstance()` checks to special-case certain subclasses
  before calling a method.
- The "is-a" relationship was chosen for reuse convenience, not because the subtype
  truly satisfies every behavioral promise of the supertype.

## Step-by-step process

**Before:**

```python
class Vehicle:
    def __init__(self):
        self.fuel_level = 100

    def refuel(self):
        self.fuel_level = 100
        print("Refueled.")

    def drive(self):
        print("Driving using fuel.")


class ElectricCar(Vehicle):
    def refuel(self):
        raise Exception("Electric cars don't use fuel, they charge!")   # LSP violation

    def drive(self):
        print("Driving using battery.")
```

**Step 1 — identify the varying behavior** ("how the vehicle gets its energy") that's
being forced awkwardly into an override.

**Step 2 — extract that behavior into its own interface:**

```python
from abc import ABC, abstractmethod

class EnergySource(ABC):
    @abstractmethod
    def replenish(self): ...
    @abstractmethod
    def power_description(self): ...

class FuelTank(EnergySource):
    def replenish(self):
        print("Refueled.")
    def power_description(self):
        return "fuel"

class Battery(EnergySource):
    def replenish(self):
        print("Charged.")
    def power_description(self):
        return "battery"
```

**Step 3 — compose instead of inherit:**

```python
class Vehicle:
    def __init__(self, energy_source: EnergySource):
        self.energy_source = energy_source

    def refuel(self):
        self.energy_source.replenish()

    def drive(self):
        print(f"Driving using {self.energy_source.power_description()}.")
```

**Step 4 — construct variants via composition, not subclassing:**

```python
gas_car = Vehicle(FuelTank())
electric_car = Vehicle(Battery())

gas_car.refuel()       # "Refueled."
electric_car.refuel()  # "Charged." — no exception, no special-casing needed
```

## Why this is strictly better here

- No method exists that any variant can't honor — the LSP violation is structurally
  impossible now, not just avoided by convention.
- Adding a `HybridEnergySource` (combining a small battery and a small tank) is a new
  class, with zero changes to `Vehicle` — a nice OCP bonus on top of the LSP fix.
- Testing `Vehicle`'s `drive()` logic can use a trivial fake `EnergySource`, without
  needing a real `FuelTank` or `Battery` implementation.

## When inheritance was fine to begin with

If every subclass can *fully* honor 100% of the base class's behavioral contract with
no overrides that throw, no-op, or diverge from documented behavior — inheritance was
never the problem, and this refactor is unnecessary. The smell to watch for is
specifically an override that surprises or breaks the base contract.
