# SRP: Interview Questions

## Q1: What is the Single Responsibility Principle?

**A:** A class or module should have only one reason to change — it should be
responsible to a single actor or stakeholder. This is about cohesion of purpose, not
literally limiting a class to one method.

## Q2: Does SRP mean a class can only have one method?

**A:** No. A class can have many methods as long as they all serve the same
responsibility. For example, a `Matrix` class can have `add`, `multiply`,
`transpose`, and `determinant` — all serve "matrix math," so it's still SRP-compliant.

## Q3: How do you identify an SRP violation in a code review?

**A:** Ask whether you can describe the class's purpose in one sentence without using
"and." Also check whether multiple unrelated teams would need to modify the same class
for unrelated reasons (e.g., a pricing change and a database migration both touching
the same `Order` class).

## Q4: What's the difference between SRP and the concept of "high cohesion"?

**A:** They're closely related — SRP is essentially a rule for achieving high
cohesion at the class level. Cohesion is the general software-quality metric; SRP is
the specific SOLID principle that operationalizes it for class design.

## Q5: Can you give an example where over-applying SRP is harmful?

**A:** Splitting a simple, stable `Point` class (with `x`, `y`, and a `distance_to()`
method) into three separate classes because "distance calculation is a different
responsibility from data storage" is usually overkill — it adds indirection without a
real, distinct actor driving change. SRP should be applied where change is likely and
where multiple actors realistically diverge, not reflexively everywhere.

## Q6: How does SRP relate to microservices?

**A:** At the service level, SRP suggests each microservice should own a single
bounded context / business capability (e.g., "billing," "inventory") rather than
becoming a monolith split only by network boundary. A service that handles billing,
shipping, and user auth is a "distributed God Class."

## Q7: A hiring manager gives you this class and asks "does this violate SRP?"

```python
class OrderProcessor:
    def process(self, order):
        self.validate(order)
        self.charge_payment(order)
        self.update_inventory(order)
        self.send_confirmation_email(order)
```

**Strong answer:** Yes — this method alone touches at least four separate concerns
(validation, payment, inventory, notifications), each owned by a different team in a
typical e-commerce org. The fix is to extract each concern into its own
single-purpose collaborator class and have `OrderProcessor` become a thin orchestrator
that calls each one in sequence — which is itself a legitimate, narrow
responsibility: "coordinate the order-processing workflow."

## Q8: How do you test whether your SRP split was correct?

**A:** After splitting, each new class should be unit-testable without mocking
unrelated concerns, and a change requested by one team should require touching only
one of the new classes, not several.
