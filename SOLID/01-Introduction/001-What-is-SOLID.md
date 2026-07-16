# What is SOLID?

SOLID is an acronym coined by Michael Feathers (based on principles collected and
championed by Robert C. Martin) for five principles of object-oriented design:

| Letter | Principle | One-line idea |
|---|---|---|
| **S** | Single Responsibility Principle (SRP) | A class should have only one reason to change. |
| **O** | Open/Closed Principle (OCP) | Software entities should be open for extension, closed for modification. |
| **L** | Liskov Substitution Principle (LSP) | Subtypes must be substitutable for their base types. |
| **I** | Interface Segregation Principle (ISP) | Clients shouldn't depend on methods they don't use. |
| **D** | Dependency Inversion Principle (DIP) | Depend on abstractions, not concretions. |

## Where it came from

The ideas were assembled through the 1990s–2000s, largely from Bertrand Meyer's work
on Open/Closed, Barbara Liskov's 1987 substitutability paper, and Robert C. Martin's
writing at Object Mentor. Martin bundled them into a single teachable set in the early
2000s; Michael Feathers later condensed the five principle names into the SOLID
acronym we use today.

## What SOLID is *not*

- It is **not** a methodology or process (that's Agile/Scrum territory).
- It is **not** a set of laws that must always be obeyed — applying SRP or OCP to a
  10-line script is usually overkill.
- It is **not** specific to any one language, though it's most commonly discussed in
  statically-typed OO languages (Java, C#) because interfaces make some principles
  more visible.

## Why five principles instead of one big rule?

Each principle attacks a different axis of fragility in code:

- **SRP** attacks classes that try to do too much.
- **OCP** attacks code that must be edited every time a new case appears.
- **LSP** attacks inheritance hierarchies that lie about their behavior.
- **ISP** attacks interfaces that are too fat for their consumers.
- **DIP** attacks tight coupling between high-level policy and low-level detail.

Together, they push you toward small, focused, composable units of code that are easy
to test in isolation and easy to recombine.

Continue to `002-Why-SOLID.md` for the motivation and cost/benefit tradeoffs.
