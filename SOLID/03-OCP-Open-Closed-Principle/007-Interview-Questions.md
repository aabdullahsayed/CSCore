# OCP: Interview Questions

## Q1: State the Open/Closed Principle.

**A:** Software entities should be open for extension but closed for modification —
you should be able to add new behavior without editing existing, tested code, usually
via polymorphism or composition.

## Q2: How is OCP achieved in practice?

**A:** Typically via an abstraction (interface or abstract base class) that existing
code depends on, plus new concrete implementations added as new classes/files for each
new variant — most commonly formalized as the Strategy pattern.

## Q3: Give a real code smell that signals an OCP violation.

**A:** A function or method with a long `if/elif` or `switch` chain that branches on a
"type" field, where new cases are added over time by editing that same function.

## Q4: Is OCP only about class inheritance?

**A:** No — OCP is most often achieved via composition + interfaces (Strategy
pattern) rather than inheritance. Pure inheritance-based extension can itself violate
LSP if not done carefully; composition is generally the safer default.

## Q5: Does closed-for-modification mean you should never edit a file again?

**A:** No. Bug fixes and changes to the requirements of *existing* behavior still
require modification. OCP targets the *addition of new variants/behavior*, not all
change everywhere — an OCP-compliant discount system still gets edited if the VIP
discount percentage itself needs correcting; it just doesn't get edited to add a new
tier.

## Q6: When is it a mistake to apply OCP?

**A:** When the set of variants is small, fixed, and unlikely to grow (e.g., a
boolean flag with two permanent outcomes). Introducing interfaces and strategy classes
for something that will never have a third case adds indirection without payoff — a
classic over-engineering mistake.

## Q7: How does OCP relate to the Strategy design pattern?

**A:** Strategy is the standard implementation vehicle for OCP: define an interface
for "a way of doing X," implement each variant as a class, and have client code depend
only on the interface. New variants are added by writing new classes, never by editing
existing ones.

## Q8: A reviewer says "this violates OCP" on your PR that added an `elif` branch to an
existing function. How do you respond / what would you change?

**A:** Acknowledge the concern, then evaluate: is this axis of variation likely to
keep growing (new payment methods, new file formats, new customer tiers)? If yes,
extract an interface and convert the branch into a new class implementing it,
registered in a small factory/lookup — leaving the original function to just delegate.
If the branches are genuinely fixed and unlikely to grow, explain why the `if/elif` is
an acceptable, simpler tradeoff in this specific case, since OCP is a heuristic, not a
hard rule.
