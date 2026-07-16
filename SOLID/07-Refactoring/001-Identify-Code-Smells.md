# Refactoring: Identifying Code Smells Systematically

Before refactoring toward SOLID, you need a repeatable process for finding what to
fix. This file gives a checklist-driven approach.

## The scan checklist

When reviewing an unfamiliar class or file, check for each of these in order:

1. **Length test** — Is this file/class over ~200-300 lines? Long files are the first
   signal of an SRP violation (though length alone isn't proof).
2. **"And" test** — Try to describe the class's purpose in one sentence. Did you need
   the word "and"? → likely SRP violation.
3. **Branch-on-type test** — Search for `if`/`elif`/`switch` chains keyed on a type,
   category, or enum field. → likely OCP violation.
4. **Throw-in-override test** — Search subclasses for `raise NotImplementedError`,
   empty overrides, or overrides that behave very differently from the base class
   docstring. → likely LSP violation.
5. **Fat-interface test** — Does any interface have implementers that only use a
   fraction of its methods, stubbing out the rest? → likely ISP violation.
6. **`new`/direct-instantiation test** — Search for high-level classes directly
   instantiating low-level infrastructure classes (`MySQLConnection()`,
   `requests.get(...)` embedded deep in business logic) instead of receiving them as
   injected abstractions. → likely DIP violation.

## Tooling that helps automate parts of this

- **Cyclomatic complexity linters** (e.g. `radon`, `flake8-cognitive-complexity` for
  Python) flag functions with too many branches — a proxy for OCP violations.
- **Dependency graph tools** can reveal high-level modules with edges pointing
  directly into low-level/infrastructure packages — a proxy for DIP violations.
- **Test-writing friction** is itself a smell detector: if writing a unit test for a
  class requires mocking five unrelated things, that class is probably violating
  SRP and/or DIP.

## A priority order for fixing what you find

When multiple smells are present (common in older codebases), fix in this order:

1. **DIP first** — inverting dependencies at the boundaries makes the code testable,
   which makes every subsequent refactor safer (you can verify you didn't break
   anything).
2. **SRP second** — splitting God Classes into focused pieces makes the remaining
   smells (OCP, LSP, ISP) easier to see and fix, because you're no longer staring at
   500 lines of unrelated logic at once.
3. **OCP, LSP, ISP** — apply as needed to the now-smaller, focused classes.

## A note on refactoring safety

Never refactor toward SOLID without a safety net. Before starting:

- Write characterization tests (tests that lock in *current* behavior, even if it's
  imperfect) if none exist.
- Refactor in small, independently-committable steps — extract one class, run tests,
  commit; repeat.
- Resist the urge to fix a *behavior* bug and a *structure* problem in the same
  commit — keep them separable so a regression can be bisected cleanly.

Continue to `002-Extract-Class.md` for the first concrete refactoring technique.
