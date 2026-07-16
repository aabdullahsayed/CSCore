# SOLID Principles — A Complete Study Guide

A self-contained course on the SOLID principles of object-oriented design, written for
learners preparing for interviews, refactoring real codebases, or studying software
architecture from the ground up.

Code examples throughout use **Python** for readability, but every principle is
language-agnostic and applies equally to Java, C#, TypeScript, Go (with interfaces), etc.

## How this repo is organized

| Folder | Contents |
|---|---|
| `01-Introduction/` | What SOLID is, why it matters, code smells, coupling/cohesion, composition vs inheritance |
| `02-SRP.../` | Single Responsibility Principle |
| `03-OCP.../` | Open/Closed Principle |
| `04-LSP.../` | Liskov Substitution Principle |
| `05-ISP.../` | Interface Segregation Principle |
| `06-DIP.../` | Dependency Inversion Principle |
| `07-Refactoring/` | General refactoring techniques that support SOLID |
| `08-Design-Patterns/` | Classic GoF patterns that emerge naturally from SOLID |
| `09-Practice-Problems/` | Hands-on problems to refactor yourself |
| `10-Case-Studies/` | End-to-end designs of real systems using SOLID |
| `11-Interview/` | Revision notes, common questions, and a cheat sheet |

## Suggested study path

1. Read `01-Introduction` fully before anything else — it sets up vocabulary
   (coupling, cohesion, code smells) used throughout the rest of the repo.
2. Work through folders `02` to `06` in order. Each principle folder follows the same
   pattern: **Definition → Bad Example → Good Example → Extra topic → Exercises →
   Interview Questions**.
3. Use `07-Refactoring` and `08-Design-Patterns` to connect the dots between
   principles and the patterns that implement them.
4. Test yourself with `09-Practice-Problems`, then check your design instincts against
   `10-Case-Studies`.
5. Use `11-Interview` the night before an interview as a fast refresher.

## What is SOLID, in one paragraph

SOLID is an acronym for five design principles — Single Responsibility, Open/Closed,
Liskov Substitution, Interface Segregation, and Dependency Inversion — that together
push a codebase toward being easier to understand, extend, test, and maintain. They
were popularized by Robert C. Martin ("Uncle Bob") and are not laws but heuristics:
useful defaults you deviate from with a reason, not rules to apply mechanically
everywhere.

Happy studying!
