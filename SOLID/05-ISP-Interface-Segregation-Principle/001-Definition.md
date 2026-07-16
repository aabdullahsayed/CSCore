# ISP: Definition

> "Clients should not be forced to depend on methods they do not use."
> — Robert C. Martin

## The core idea

Interface Segregation says: prefer many small, specific interfaces over one large,
general-purpose interface. If a class only needs 2 of an interface's 10 methods, that
interface is too fat, and should be split.

## Why "fat interfaces" are dangerous

```python
from abc import ABC, abstractmethod

class Worker(ABC):
    @abstractmethod
    def work(self): ...
    @abstractmethod
    def eat(self): ...
    @abstractmethod
    def sleep(self): ...
```

This looks reasonable until a `Robot` needs to implement `Worker`:

```python
class Robot(Worker):
    def work(self):
        print("Robot working.")
    def eat(self):
        raise NotImplementedError("Robots don't eat")   # forced, meaningless method
    def sleep(self):
        raise NotImplementedError("Robots don't sleep")  # forced, meaningless method
```

`Robot` is forced to implement — or explicitly reject — methods that make no sense for
it. This is both an ISP violation (fat interface) and, as a direct consequence, an LSP
violation (any code calling `worker.eat()` will crash on a `Robot`).

## The fix: split by client need

```python
class Workable(ABC):
    @abstractmethod
    def work(self): ...

class Eatable(ABC):
    @abstractmethod
    def eat(self): ...

class Sleepable(ABC):
    @abstractmethod
    def sleep(self): ...

class Human(Workable, Eatable, Sleepable):
    def work(self): print("Human working.")
    def eat(self): print("Human eating.")
    def sleep(self): print("Human sleeping.")

class Robot(Workable):
    def work(self): print("Robot working.")
```

`Robot` now only implements `Workable` — it's never forced to lie about eating or
sleeping. Code that only needs work-related behavior depends only on `Workable`, so it
can accept a `Robot` too.

## ISP from the *client's* perspective

The principle is named "interface segregation," but it's really about **segregating
by client**: look at who *consumes* an interface and what subset of methods each
consumer actually calls. If different consumers use disjoint subsets of a big
interface's methods, that's the signal to split it along those lines.

## What ISP is *not*

- It's not "always use tiny one-method interfaces." Cohesive interfaces (like
  `Comparable` with just `compare_to`) are fine as-is if every client genuinely needs
  the whole thing.
- It's not unrelated to SRP — ISP is essentially SRP applied to interfaces/contracts
  instead of implementation classes.

Continue to `002-Fat-Interface.md` for a deeper example.
