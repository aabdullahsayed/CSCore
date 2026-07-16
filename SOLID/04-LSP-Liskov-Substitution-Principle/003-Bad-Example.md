# LSP: Bad Example — Birds That Can't Fly

```python
class Bird:
    def fly(self):
        print("Flying through the air!")

    def eat(self):
        print("Eating food.")


class Sparrow(Bird):
    pass  # inherits fly() correctly


class Penguin(Bird):
    def fly(self):
        raise Exception("Penguins can't fly!")
```

## Where this breaks calling code

```python
def make_bird_fly(bird: Bird):
    bird.fly()

birds = [Sparrow(), Penguin()]
for b in birds:
    make_bird_fly(b)   # crashes on Penguin!
```

Any function written against `Bird` — reasonably assuming every `Bird` can `fly()`,
because that's the contract the base class advertises — will crash the instant a
`Penguin` is passed in. The caller did nothing wrong; the `Penguin` subclass silently
broke the promise made by its parent type.

## Why this is worse than a compile error

In statically-typed languages this compiles fine — `Penguin` *is* a `Bird`
syntactically. The violation only shows up at runtime, often in production, often far
away from where `Penguin` was written. This is exactly the risk LSP protects against:
a type system can't catch behavioral contract violations, only structural ones.

## Another variant: throwing on a documented no-op

```python
class Logger:
    def log(self, message: str):
        print(message)

class NoOpLogger(Logger):
    def log(self, message: str):
        pass  # fine — this is actually LSP-compliant, it just does nothing safely

class BrokenLogger(Logger):
    def log(self, message: str):
        raise NotImplementedError("Logging disabled in this environment")
        # NOT LSP-compliant: callers expect log() to always succeed silently
```

`NoOpLogger` is a valid substitute (does nothing, but doesn't break the caller).
`BrokenLogger` is not — any code that calls `.log()` without expecting an exception
will crash.

## The general pattern to watch for

Any subclass that:
- overrides a method to `raise NotImplementedError` or a custom exception,
- overrides a method to silently do less than what callers expect,
- adds new, narrower preconditions (e.g., requiring non-null where the base class
  accepted null),

...is a strong candidate for an LSP violation.

Continue to `004-Good-Example.md` for the fix.
