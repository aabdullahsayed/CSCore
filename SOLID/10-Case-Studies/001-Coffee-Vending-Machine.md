# Case Study: Coffee Vending Machine

## Requirements

- Supports multiple drink types (coffee, tea, hot chocolate), each with its own
  recipe/ingredients.
- Supports multiple payment methods (coin, card, mobile pay).
- Must be easy to add a new drink or payment method without touching the core
  dispensing logic.

## Design

```python
from abc import ABC, abstractmethod

class Drink(ABC):
    @abstractmethod
    def name(self) -> str: ...
    @abstractmethod
    def price(self) -> float: ...
    @abstractmethod
    def brew(self) -> None: ...

class Coffee(Drink):
    def name(self): return "Coffee"
    def price(self): return 2.50
    def brew(self): print("Brewing coffee: hot water + ground beans")

class Tea(Drink):
    def name(self): return "Tea"
    def price(self): return 2.00
    def brew(self): print("Brewing tea: hot water + tea leaves")

class HotChocolate(Drink):
    def name(self): return "Hot Chocolate"
    def price(self): return 3.00
    def brew(self): print("Brewing hot chocolate: hot milk + cocoa powder")


class PaymentMethod(ABC):
    @abstractmethod
    def pay(self, amount: float) -> bool: ...

class CoinPayment(PaymentMethod):
    def pay(self, amount):
        print(f"Inserting coins for ${amount}")
        return True

class CardPayment(PaymentMethod):
    def pay(self, amount):
        print(f"Charging card ${amount}")
        return True

class MobilePayment(PaymentMethod):
    def pay(self, amount):
        print(f"Processing mobile payment ${amount}")
        return True


class VendingMachine:
    def dispense(self, drink: Drink, payment: PaymentMethod):
        if payment.pay(drink.price()):
            drink.brew()
            print(f"Here's your {drink.name()}!")
        else:
            print("Payment failed.")
```

## SOLID principles applied

- **SRP:** `VendingMachine` only orchestrates; `Drink` classes own recipes;
  `PaymentMethod` classes own payment logic.
- **OCP:** adding `Espresso` or `ApplePayPayment` requires zero edits to
  `VendingMachine`.
- **LSP:** every `Drink` and `PaymentMethod` fully honors its abstract contract — no
  drink throws on `brew()`, no payment method throws on `pay()`.
- **DIP:** `VendingMachine` depends only on the two abstractions, never on concrete
  drink or payment classes.

## Extension exercise

Add an `InventoryTracker` that decrements ingredient stock on every `brew()` call and
prevents dispensing when out of stock, without modifying `VendingMachine`'s public
interface. (Hint: this is a natural fit for the Observer pattern —
`08-Design-Patterns/005-Observer.md`.)
