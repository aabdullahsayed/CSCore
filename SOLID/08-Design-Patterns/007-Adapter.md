# Design Pattern: Adapter

## Intent

Convert the interface of a class into another interface clients expect. Adapter lets
classes with incompatible interfaces work together — often used to satisfy DIP when
integrating a third-party library whose API doesn't match the abstraction your
high-level code depends on.

## The problem: a vendor SDK doesn't match your abstraction

```python
# Your domain's abstraction (owned by high-level business logic):
class PaymentGateway(ABC):
    @abstractmethod
    def charge(self, amount_cents: int, customer_id: str) -> bool: ...

# A third-party SDK you don't control, with an incompatible shape:
class StripeSDK:
    def create_charge(self, amount, currency, customer):
        print(f"Stripe charging {amount} {currency} to {customer}")
        return {"status": "succeeded"}
```

You can't modify `StripeSDK` (it's a vendor library), and you don't want
`OrderService` coupled to Stripe's specific method names and response shape — that
would violate DIP.

## The Adapter

```python
class StripeAdapter(PaymentGateway):
    def __init__(self, stripe_sdk: StripeSDK):
        self._sdk = stripe_sdk

    def charge(self, amount_cents: int, customer_id: str) -> bool:
        response = self._sdk.create_charge(
            amount=amount_cents / 100, currency="usd", customer=customer_id
        )
        return response["status"] == "succeeded"
```

## Usage

```python
class OrderService:
    def __init__(self, gateway: PaymentGateway):
        self.gateway = gateway

    def checkout(self, amount_cents, customer_id):
        return self.gateway.charge(amount_cents, customer_id)


gateway = StripeAdapter(StripeSDK())
service = OrderService(gateway)
service.checkout(2500, "cust_123")
```

`OrderService` never sees `StripeSDK`'s method names, parameter order, or response
shape — the adapter absorbs all of that translation.

## SOLID connections

- **DIP:** the adapter lets high-level code depend on `PaymentGateway` (an
  abstraction it owns) rather than being reshaped around a vendor's specific API.
- **OCP:** switching from Stripe to Braintree means writing a `BraintreeAdapter` — no
  changes to `OrderService`.
- **LSP:** the adapter must fully and correctly honor the `PaymentGateway` contract —
  if it can't (e.g., the vendor doesn't support a feature the interface promises),
  that's an LSP violation waiting to surface, and the interface may need
  re-examining.

## Adapter vs Decorator — don't confuse them

Both wrap another object, but:
- **Adapter** changes the *interface* (translates one shape of API into another it's
  compatible with).
- **Decorator** keeps the *same* interface and adds behavior around it.

If you're translating between two different interfaces, it's Adapter. If you're
layering extra behavior onto the same interface, it's Decorator.
