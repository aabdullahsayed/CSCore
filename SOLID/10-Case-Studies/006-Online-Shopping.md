# Case Study: Online Shopping Cart

## Requirements

- Cart supports multiple discount types (percentage, flat, buy-one-get-one) that can
  stack.
- Multiple shipping options, each with different cost and delivery-time rules.
- Multiple payment methods.
- Must produce an order confirmation without any single class knowing every detail.

## Design

```python
from abc import ABC, abstractmethod

class DiscountRule(ABC):
    @abstractmethod
    def apply(self, subtotal: float) -> float: ...

class PercentageDiscount(DiscountRule):
    def __init__(self, percent):
        self.percent = percent
    def apply(self, subtotal):
        return subtotal * (1 - self.percent / 100)

class FlatDiscount(DiscountRule):
    def __init__(self, amount):
        self.amount = amount
    def apply(self, subtotal):
        return max(0, subtotal - self.amount)


class ShippingOption(ABC):
    @abstractmethod
    def cost(self, weight: float) -> float: ...
    @abstractmethod
    def eta_days(self) -> int: ...

class StandardShipping(ShippingOption):
    def cost(self, weight): return weight * 1.0
    def eta_days(self): return 5

class ExpressShipping(ShippingOption):
    def cost(self, weight): return weight * 3.0
    def eta_days(self): return 2


class PaymentMethod(ABC):
    @abstractmethod
    def pay(self, amount: float) -> bool: ...

class CreditCardPayment(PaymentMethod):
    def pay(self, amount):
        print(f"Charging ${amount:.2f} to credit card")
        return True


class CartItem:
    def __init__(self, name, price, quantity, weight):
        self.name = name
        self.price = price
        self.quantity = quantity
        self.weight = weight


class Cart:
    def __init__(self):
        self.items: list[CartItem] = []
        self.discounts: list[DiscountRule] = []

    def add_item(self, item: CartItem):
        self.items.append(item)

    def add_discount(self, discount: DiscountRule):
        self.discounts.append(discount)

    def subtotal(self) -> float:
        return sum(item.price * item.quantity for item in self.items)

    def total_weight(self) -> float:
        return sum(item.weight * item.quantity for item in self.items)

    def total_after_discounts(self) -> float:
        total = self.subtotal()
        for discount in self.discounts:
            total = discount.apply(total)
        return total


class CheckoutService:
    def checkout(self, cart: Cart, shipping: ShippingOption, payment: PaymentMethod):
        total = cart.total_after_discounts() + shipping.cost(cart.total_weight())
        if payment.pay(total):
            return (
                f"Order confirmed! Total: ${total:.2f}, "
                f"arriving in {shipping.eta_days()} days"
            )
        return "Payment failed."
```

## Usage

```python
cart = Cart()
cart.add_item(CartItem("Book", 15.0, 2, 0.5))
cart.add_item(CartItem("Headphones", 60.0, 1, 0.3))
cart.add_discount(PercentageDiscount(10))
cart.add_discount(FlatDiscount(5))

checkout = CheckoutService()
print(checkout.checkout(cart, ExpressShipping(), CreditCardPayment()))
```

## SOLID principles applied

- **OCP:** stacking a new `BuyOneGetOneDiscount`, adding `OvernightShipping`, or
  `CryptoPayment` never touches `Cart` or `CheckoutService`.
- **SRP:** `Cart` only manages items/discounts/totals; `CheckoutService` only
  orchestrates the final transaction; each discount/shipping/payment class owns
  exactly one calculation.
- **DIP:** `CheckoutService` depends on the three abstractions, never on any concrete
  discount, shipping, or payment class.

## Extension exercise

Add a `LoyaltyPointsPayment` that can partially cover the total (points cover part of
the cost, remaining balance charged to a fallback `PaymentMethod`). Consider whether
`PaymentMethod.pay` needs a richer return type (e.g., amount actually covered) to
support this without breaking LSP for existing payment methods.
