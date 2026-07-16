# Interview Revision: SOLID in 5 Minutes

## The five principles, restated tightly

| Principle | Statement | Fixes | Typical smell |
|---|---|---|---|
| **SRP** | A class should have one reason to change / serve one actor. | God Classes | Class description needs "and" |
| **OCP** | Open for extension, closed for modification. | Fragile `if/elif` chains | New feature = editing existing function |
| **LSP** | Subtypes must be substitutable for their base type without breaking correctness. | Broken polymorphism | Override throws / no-ops unexpectedly |
| **ISP** | Clients shouldn't depend on methods they don't use. | Fat interfaces | Implementers stub out unused methods |
| **DIP** | Depend on abstractions, not concretions; high-level shouldn't depend on low-level. | Tight coupling to infrastructure | `new ConcreteClass()` inside business logic |

## The one-sentence version of each, memorized for rapid-fire questions

- **S**: "One class, one job, one actor."
- **O**: "Add new code, don't edit old code."
- **L**: "If it looks like a duck but can't quack, don't call it a duck."
- **I**: "Don't force classes to implement methods they'll never use."
- **D**: "Depend on interfaces, not concrete implementations — and let low-level
  details conform to the abstraction, not the other way around."

## How the principles relate to each other

- ISP violations frequently **cause** LSP violations (fat interfaces force fake
  implementations that then break substitutability).
- DIP is what makes OCP **achievable** in practice (you can only add new
  implementations without touching existing code if that code already depends on an
  abstraction).
- SRP and ISP are the same idea applied at different levels: SRP is about
  implementation classes, ISP is about interfaces/contracts.
- LSP is a *precondition* for OCP and DIP to be trustworthy — if subclasses don't
  honor their contracts, polymorphism (the mechanism both OCP and DIP rely on)
  becomes unsafe.

## Common interviewer angle: "Explain SOLID using a single running example"

A strong answer picks one example (e.g., a payment system) and shows all five in
minified form:

- **SRP**: `PaymentValidator`, `PaymentProcessor`, `ReceiptPrinter` — separate classes.
- **OCP**: new payment methods are new `PaymentMethod` subclasses, not new `elif`
  branches.
- **LSP**: every `PaymentMethod` fully honors `pay(amount) -> bool` — none throws
  unexpectedly.
- **ISP**: `PaymentMethod` has one lean method; a `Refundable` interface is separate
  so payment methods that can't refund aren't forced to implement it.
- **DIP**: `Checkout` depends on `PaymentMethod` (abstraction), not on
  `StripeSDK` (concrete detail) directly — a `StripeAdapter` bridges the gap.

This is exactly the running example built out across `09-Practice-Problems/004` and
`10-Case-Studies/006` — review those for the full code.

Continue to `002-Common-Questions.md`.
