# SOLID Cheat Sheet

Print this page (or keep it open) the night before an interview.

## The five, ultra-condensed

- **S — Single Responsibility:** One class, one actor, one reason to change.
- **O — Open/Closed:** Add new code for new behavior; don't edit existing, tested code.
- **L — Liskov Substitution:** Subclasses must be fully swappable for their base type —
  no surprise throws, no weakened guarantees.
- **I — Interface Segregation:** Split fat interfaces so no class is forced to
  implement methods it doesn't need.
- **D — Dependency Inversion:** High-level code depends on abstractions; low-level
  details implement those abstractions — not the reverse.

## Smell → Principle → Fix table

| Smell | Principle violated | Refactoring technique |
|---|---|---|
| God Class doing 3+ unrelated things | SRP | Extract Class |
| Growing `if/elif`/`switch` on a type field | OCP | Replace Conditional with Polymorphism (→ Strategy) |
| Subclass overrides a method to throw/no-op unexpectedly | LSP | Replace Inheritance with Composition, or redesign hierarchy |
| Interface with implementers stubbing out unused methods | ISP | Split interface by client need |
| Business logic directly instantiates infra classes (`new SqlDb()`) | DIP | Extract Interface + Constructor Injection |

## Pattern → Principle cross-reference

| Pattern | Primarily supports |
|---|---|
| Strategy | OCP |
| Factory Method / Abstract Factory | OCP, DIP |
| Builder | SRP |
| Observer | OCP, DIP |
| Decorator | OCP, LSP |
| Adapter | DIP |
| Command | OCP, SRP, DIP |
| State | OCP (same structure as Strategy, different intent) |
| Template Method | OCP, disciplined LSP-safe inheritance |

## Three questions to ask before applying any SOLID principle

1. **Is this axis of variation actually likely to grow**, or is it a fixed, small set
   that will never realistically need a third/fourth case?
2. **Is there a real second consumer or second implementation** on the horizon (or a
   genuine testing need for a fake), or would the abstraction have exactly one
   implementation forever?
3. **Does the fix reduce or increase the cognitive load** of reading this code for the
   next person? (SOLID should make code *easier* to reason about — if a "fix" adds
   three files and two interfaces to solve a problem that will never recur, it's
   probably over-engineering.)

## One-liner interview answer if asked "why does SOLID matter?"

> "SOLID principles reduce the blast radius of change — they make it so that adding a
> feature means writing new code instead of editing old code, and so that a change
> requested by one team doesn't accidentally break code owned by another."
