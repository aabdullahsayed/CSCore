# OCP: Bad Example

A discount calculator for an e-commerce checkout, implemented with a growing
`if/elif` chain.

```python
class DiscountCalculator:
    def calculate(self, customer_type, amount):
        if customer_type == "regular":
            return amount * 0.95
        elif customer_type == "premium":
            return amount * 0.90
        elif customer_type == "vip":
            return amount * 0.80
        else:
            return amount
```

## Why this violates OCP

Every time the business introduces a new customer tier — "student," "employee,"
"first-time-buyer" — a developer must **open this exact function and add another
`elif`**. This function is never closed; it's a permanent modification target for
anyone touching the discount system.

## The compounding cost

1. **Merge conflicts:** if two developers each add a new tier in parallel branches,
   they collide on the same lines of the same function.
2. **Regression risk:** editing this function to add "student" discount risks a typo
   that breaks the existing "vip" branch — and nothing in the type system prevents it.
3. **Testing burden:** every edit to this function means the *entire* function needs
   re-review, even though only one new branch was added — because a shared function
   body means shared blast radius.
4. **Violates SRP too:** this function increasingly becomes responsible for
   *knowing about every discount rule that exists in the business*, which is really
   the job of individual pricing/promotions owners, not one central function.

## The tell

Search your codebase for `if type ==` or `switch (type)` patterns scattered across
multiple functions that all branch on the *same* enumeration of types (customer type,
shape type, payment type, file type). That's the fingerprint of an OCP violation
waiting for a polymorphic fix.

Continue to `003-Good-Example.md`.
