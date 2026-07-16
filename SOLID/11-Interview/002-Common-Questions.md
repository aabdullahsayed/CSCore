# Interview Revision: Common Questions

## Conceptual questions

1. What is SOLID and who introduced it?
2. State each of the five principles precisely, not just the acronym expansion.
3. What's the difference between SRP and high cohesion generally?
4. Why is "is-a" from real life not sufficient justification for inheritance? (Answer
   with the Rectangle/Square or Bird/Penguin example.)
5. What's the difference between Dependency Inversion and Dependency Injection?
6. How does ISP relate to LSP? Give an example where violating one causes the other.
7. Is SOLID applicable to functional programming, or only OOP?
   - **Short answer:** the *spirit* transfers (small focused functions ≈ SRP, passing
     functions as parameters instead of hardcoding branches ≈ OCP/DIP, avoiding
     partial functions that throw on subsets of input ≈ LSP), even though the
     class/interface vocabulary is OOP-specific.
8. When is it a mistake to apply a SOLID principle? Give a concrete example for each
   principle.

## Code-review style questions

Interviewers often show a code snippet and ask "what SOLID principle does this
violate, and how would you fix it?" Practice recognizing the fingerprint quickly:

- Long class with unrelated method groups → **SRP**
- `if type == ... elif type == ...` chain that keeps growing → **OCP**
- Subclass method that throws `NotImplementedError` or silently no-ops → **LSP**
  (often traceable back to an **ISP** violation in the parent interface)
- Interface with many implementers, most stubbing out several methods → **ISP**
- Business logic class directly instantiating a database/HTTP/file class → **DIP**

## System design questions

- "Design a plugin architecture for X" → expect OCP + DIP (an interface for plugins,
  a registry, no core-code edits for new plugins).
- "Design a notification/event system" → expect Observer (OCP + DIP,
  `08-Design-Patterns/005-Observer.md`).
- "How would you make this payment system support a new provider without downtime or
  redeploying core logic?" → expect DIP + Adapter
  (`08-Design-Patterns/007-Adapter.md`).

## Behavioral / experience questions

- "Tell me about a time you refactored a God Class." Structure your answer using the
  process from `07-Refactoring/001` through `007-Refactoring/005`: identify actors →
  extract classes → verify with tests.
- "Tell me about a time SOLID was over-applied and caused unnecessary complexity."
  Interviewers value engineers who know the principles are heuristics, not laws — see
  `01-Introduction/002-Why-SOLID.md`'s "honest tradeoff" section for language to use
  here.

Continue to `003-Scenario-Based-Problems.md` for full worked scenarios.
