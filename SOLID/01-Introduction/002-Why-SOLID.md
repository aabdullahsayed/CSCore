# Why SOLID?

## The problem SOLID solves

Software rots. Not because the disk degrades, but because as requirements change,
small unprincipled edits accumulate: an `if` here, a new parameter there, a quick
subclass to reuse 90% of a method. Individually each change looks harmless. Over
months, the codebase becomes:

- **Rigid** — one change forces cascading changes elsewhere.
- **Fragile** — changes break things in unrelated parts of the system.
- **Immobile** — code can't be reused because it's tangled with things it doesn't need.
- **Viscous** — doing things the "right" way is harder than hacking around the design,
  so people take the hack.

SOLID principles are heuristics that keep these four symptoms in check *before* they
become expensive.

## Concrete costs of ignoring SOLID

Consider a class `OrderProcessor` that calculates totals, saves to a database, sends a
confirmation email, and logs to a file — all in one method. Every one of these
responsibilities is a separate reason for the class to change:

- Tax law changes → edit `OrderProcessor`.
- Switch from MySQL to Postgres → edit `OrderProcessor`.
- Marketing wants a nicer email template → edit `OrderProcessor`.
- Ops wants structured JSON logs instead of plain text → edit `OrderProcessor`.

Four unrelated teams now collide on the same file, the same pull requests, the same
merge conflicts, and the same regressions. This is exactly what SRP prevents.

## What you get in return

- **Testability** — small classes with one job can be unit tested without mocking half
  the system.
- **Extensibility** — OCP-compliant code lets you add a new payment method or a new
  report type by *adding* a file, not editing ten existing ones.
- **Confidence in polymorphism** — LSP means you can trust that any subclass passed
  into a function will behave correctly, so code reviews don't need to re-verify every
  call site.
- **Decoupled teams** — DIP lets a UI team code against an interface while a backend
  team implements it, in parallel, without blocking each other.

## The honest tradeoff

SOLID is not free. Over-applying it produces:

- Excessive indirection (an interface with exactly one implementation, forever).
- "Enterprise FizzBuzz"-style over-engineering for problems that will never scale.
- More files and more ceremony for genuinely simple, stable code.

The skill isn't memorizing the five letters — it's recognizing **when a piece of code
is likely to change**, and applying the relevant principle only there. Stable,
unlikely-to-change code can stay simple.

Continue to `003-Code-Smells.md` to learn the warning signs that a principle is being
violated.
