# Case Study: Hotel Management System

## Requirements

- Multiple room types (standard, deluxe, suite) with different pricing and amenities.
- Multiple booking channels (direct website, third-party OTA, phone) each with
  different commission/fee rules.
- Must support seasonal/dynamic pricing without rewriting booking logic.

## Design

```python
from abc import ABC, abstractmethod

class RoomType(ABC):
    @abstractmethod
    def base_price(self) -> float: ...
    @abstractmethod
    def amenities(self) -> list[str]: ...

class StandardRoom(RoomType):
    def base_price(self): return 100.0
    def amenities(self): return ["WiFi", "TV"]

class DeluxeRoom(RoomType):
    def base_price(self): return 180.0
    def amenities(self): return ["WiFi", "TV", "Minibar", "City View"]

class Suite(RoomType):
    def base_price(self): return 350.0
    def amenities(self): return ["WiFi", "TV", "Minibar", "Ocean View", "Jacuzzi"]


class PricingModifier(ABC):
    @abstractmethod
    def apply(self, base_price: float) -> float: ...

class SeasonalSurcharge(PricingModifier):
    def __init__(self, multiplier):
        self.multiplier = multiplier
    def apply(self, base_price):
        return base_price * self.multiplier

class NoModifier(PricingModifier):
    def apply(self, base_price):
        return base_price


class BookingChannel(ABC):
    @abstractmethod
    def commission_rate(self) -> float: ...

class DirectWebsite(BookingChannel):
    def commission_rate(self): return 0.0

class OnlineTravelAgency(BookingChannel):
    def commission_rate(self): return 0.15

class PhoneBooking(BookingChannel):
    def commission_rate(self): return 0.05


class Booking:
    def __init__(self, room: RoomType, pricing_modifier: PricingModifier,
                 channel: BookingChannel):
        self.room = room
        self.pricing_modifier = pricing_modifier
        self.channel = channel

    def total_price_for_hotel(self) -> float:
        """What the hotel actually nets after channel commission."""
        adjusted = self.pricing_modifier.apply(self.room.base_price())
        commission = adjusted * self.channel.commission_rate()
        return adjusted - commission

    def total_price_for_guest(self) -> float:
        """What the guest pays — channel commission doesn't affect guest price here."""
        return self.pricing_modifier.apply(self.room.base_price())
```

## Usage

```python
booking = Booking(Suite(), SeasonalSurcharge(1.25), OnlineTravelAgency())
print("Guest pays:", booking.total_price_for_guest())
print("Hotel nets:", booking.total_price_for_hotel())
```

## SOLID principles applied

- **OCP:** adding a `PenthouseRoom`, a `BlackFridayDiscount` modifier, or a
  `MobileAppBooking` channel never touches `Booking`.
- **SRP:** pricing, room amenities, and channel commission are three independent
  concerns, each in its own class family.
- **DIP:** `Booking` depends on three separate small abstractions rather than any
  concrete room/channel/modifier class.

## Extension exercise

Add loyalty-program discounts that stack with seasonal surcharges (e.g., a member
gets 10% off *after* the seasonal multiplier is applied). Consider composing multiple
`PricingModifier` instances via the Decorator pattern
(`08-Design-Patterns/006-Decorator.md`).
