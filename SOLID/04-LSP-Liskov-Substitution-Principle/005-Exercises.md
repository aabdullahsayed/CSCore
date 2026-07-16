# LSP: Exercises

## Exercise 1: Spot the violation

```python
class Stack:
    def push(self, item): ...
    def pop(self): ...
    def peek(self): ...

class FixedSizeStack(Stack):
    def __init__(self, max_size):
        self.max_size = max_size
        self.items = []

    def push(self, item):
        if len(self.items) >= self.max_size:
            raise OverflowError("Stack is full")
        self.items.append(item)
```

Does `FixedSizeStack` violate LSP? Justify your answer.

<details>
<summary>Suggested answer</summary>

Arguably yes, in the strict sense: if `Stack.push` is documented/assumed to always
succeed, `FixedSizeStack` strengthens the precondition (only accepts pushes while
under capacity) and can throw where the base type wouldn't. Whether this is
*acceptable* depends on whether "may reject a push" was part of the base `Stack`
contract from the start. If `Stack` were documented as "an abstract stack of unbounded
size," then yes, this is a violation — a caller of a general `Stack` reasonably
assumes pushes never fail for capacity reasons, and `FixedSizeStack` breaks that
assumption. The cleaner design documents capacity limits as part of the base contract
(e.g., a `BoundedStack` abstract type) rather than surprising callers.
</details>

## Exercise 2: Fix this hierarchy

```python
class ReadOnlyFile:
    def read(self): ...

class File(ReadOnlyFile):
    def write(self, data): ...

class LockedFile(File):
    def write(self, data):
        raise PermissionError("File is locked")
```

Redesign this so `LockedFile` doesn't violate LSP.

<details>
<summary>Suggested solution</summary>

Split into `Readable` and `Writable` interfaces. `LockedFile` implements only
`Readable`, not `Writable` — so it's structurally impossible to call `.write()` on it
where a generic `Writable` is expected, instead of throwing a runtime exception when
someone does.

```python
class Readable(ABC):
    @abstractmethod
    def read(self): ...

class Writable(ABC):
    @abstractmethod
    def write(self, data): ...

class File(Readable, Writable):
    def read(self): ...
    def write(self, data): ...

class LockedFile(Readable):
    def read(self): ...
    # no write() at all — can't be misused as Writable
```
</details>

## Exercise 3: Precondition strengthening

```python
class Calculator:
    def divide(self, a, b):
        return a / b if b != 0 else None

class StrictCalculator(Calculator):
    def divide(self, a, b):
        if b == 0:
            raise ValueError("Cannot divide by zero")
        return a / b
```

Does `StrictCalculator` violate LSP? What would code written against `Calculator`
expect, and what actually happens if a `StrictCalculator` is substituted?

<details>
<summary>Suggested answer</summary>

Yes. Code written against `Calculator` reasonably expects `divide(a, 0)` to return
`None`, not raise. Substituting `StrictCalculator` where a `Calculator` is expected
introduces an unhandled exception the caller never anticipated — a classic
"weakens the guarantee, strengthens the precondition informally" violation, since the
base type promised graceful handling of the zero case.
</details>

## Exercise 4: Design challenge

Design a small class hierarchy for `PaymentMethod` with `CreditCard`, `PayPal`, and
`GiftCard`, where `GiftCard` has a maximum balance and can't process amounts larger
than its remaining balance. Show how you'd avoid an LSP violation when `GiftCard`
can't honor an arbitrarily large payment the way `CreditCard` might.
