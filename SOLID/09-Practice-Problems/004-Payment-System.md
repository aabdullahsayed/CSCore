# Practice Problem: Payment System

## The problem

Refactor this checkout flow, which violates DIP (direct coupling to Stripe's SDK
shape), OCP (adding PayPal means editing `Checkout`), and SRP (validation, payment,
and receipt-printing are tangled together).

```python
class Checkout:
    def pay(self, method, amount, card_number=None, paypal_email=None):
        if amount <= 0:
            raise ValueError("Invalid amount")

        if method == "credit_card":
            print(f"Charging card {card_number} for ${amount} via Stripe API")
        elif method == "paypal":
            print(f"Charging PayPal account {paypal_email} for ${amount}")
        else:
            raise ValueError("Unsupported payment method")

        print(f"Receipt: paid ${amount} via {method}")
```

## Suggested solution

```python
from abc import ABC, abstractmethod

class PaymentMethod(ABC):
    @abstractmethod
    def pay(self, amount: float) -> None: ...

class CreditCardPayment(PaymentMethod):
    def __init__(self, card_number):
        self.card_number = card_number
    def pay(self, amount):
        print(f"Charging card {self.card_number} for ${amount} via Stripe API")

class PayPalPayment(PaymentMethod):
    def __init__(self, email):
        self.email = email
    def pay(self, amount):
        print(f"Charging PayPal account {self.email} for ${amount}")


class AmountValidator:
    def validate(self, amount):
        if amount <= 0:
            raise ValueError("Invalid amount")


class ReceiptPrinter:
    def print_receipt(self, amount, method: PaymentMethod):
        print(f"Receipt: paid ${amount} via {type(method).__name__}")


class Checkout:
    def __init__(self, validator: AmountValidator, printer: ReceiptPrinter):
        self.validator = validator
        self.printer = printer

    def pay(self, method: PaymentMethod, amount: float):
        self.validator.validate(amount)
        method.pay(amount)
        self.printer.print_receipt(amount, method)
```

## Usage

```python
checkout = Checkout(AmountValidator(), ReceiptPrinter())
checkout.pay(CreditCardPayment("4111-XXXX"), 49.99)
checkout.pay(PayPalPayment("bob@example.com"), 19.99)
```

Adding `CryptoPayment` (a new class implementing `PaymentMethod`) requires zero
changes to `Checkout`, `AmountValidator`, or `ReceiptPrinter`.
