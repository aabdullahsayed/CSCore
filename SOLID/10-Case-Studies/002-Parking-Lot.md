# Case Study: Parking Lot System

## Requirements

- Supports multiple vehicle types (motorcycle, car, bus) with different space
  requirements.
- Supports multiple pricing strategies (hourly, flat daily rate, membership discount).
- Must track available spots per vehicle type and assign/free spots correctly.

## Design

```python
from abc import ABC, abstractmethod

class Vehicle(ABC):
    @abstractmethod
    def spots_required(self) -> int: ...
    @abstractmethod
    def vehicle_type(self) -> str: ...

class Motorcycle(Vehicle):
    def spots_required(self): return 1
    def vehicle_type(self): return "motorcycle"

class Car(Vehicle):
    def spots_required(self): return 1
    def vehicle_type(self): return "car"

class Bus(Vehicle):
    def spots_required(self): return 5
    def vehicle_type(self): return "bus"


class PricingStrategy(ABC):
    @abstractmethod
    def calculate_fee(self, hours: float) -> float: ...

class HourlyPricing(PricingStrategy):
    def calculate_fee(self, hours): return hours * 2.50

class FlatDailyPricing(PricingStrategy):
    def calculate_fee(self, hours): return 20.00

class MembershipPricing(PricingStrategy):
    def calculate_fee(self, hours): return hours * 1.50   # discounted rate


class ParkingLot:
    def __init__(self, total_spots: int, pricing: PricingStrategy):
        self.total_spots = total_spots
        self.available_spots = total_spots
        self.pricing = pricing

    def park(self, vehicle: Vehicle) -> bool:
        if vehicle.spots_required() > self.available_spots:
            print(f"Not enough space for {vehicle.vehicle_type()}")
            return False
        self.available_spots -= vehicle.spots_required()
        print(f"Parked {vehicle.vehicle_type()}, {self.available_spots} spots left")
        return True

    def leave(self, vehicle: Vehicle, hours: float) -> float:
        self.available_spots += vehicle.spots_required()
        fee = self.pricing.calculate_fee(hours)
        print(f"{vehicle.vehicle_type()} leaving, fee: ${fee:.2f}")
        return fee
```

## SOLID principles applied

- **OCP:** new vehicle types (e.g., `Truck`) or new pricing schemes (e.g.,
  `EventSurgePricing`) are added as new classes with zero edits to `ParkingLot`.
- **LSP:** any `Vehicle` can be safely passed to `park`/`leave` — none throws
  unexpectedly or violates the interface's promises.
- **DIP:** `ParkingLot` depends on the `PricingStrategy` abstraction, so pricing can
  be swapped per lot/location without touching the parking logic itself.

## Extension exercise

Add support for reserved/VIP spots that only certain `Vehicle` subtypes can use,
without breaking substitutability for regular spots. Consider whether this needs a new
interface (`ISP`-style split) rather than adding a flag to the existing `Vehicle`
interface.
