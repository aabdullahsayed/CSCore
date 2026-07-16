# LSP: Good Example — Fixing the Bird Hierarchy

The fix is to stop modeling "flying" as something every `Bird` must do, and instead
separate "being a bird" from "being able to fly."

```python
from abc import ABC, abstractmethod

class Bird(ABC):
    @abstractmethod
    def eat(self):
        ...

class FlyingBird(Bird):
    @abstractmethod
    def fly(self):
        ...

class SwimmingBird(Bird):
    @abstractmethod
    def swim(self):
        ...


class Sparrow(FlyingBird):
    def eat(self):
        print("Sparrow eating seeds.")
    def fly(self):
        print("Sparrow flying.")


class Penguin(SwimmingBird):
    def eat(self):
        print("Penguin eating fish.")
    def swim(self):
        print("Penguin swimming.")
```

## Calling code is now safe by construction

```python
def make_bird_fly(bird: FlyingBird):
    bird.fly()

make_bird_fly(Sparrow())   # fine
make_bird_fly(Penguin())   # TYPE ERROR at the call site / static-analysis time,
                           # not a runtime crash deep in production
```

Because `Penguin` never claims to *be* a `FlyingBird`, it's now **impossible** to
accidentally pass a penguin into `make_bird_fly` — the type system (or at minimum, a
linter/reviewer) catches the mistake at the boundary, instead of it surfacing as a
runtime exception.

## Equivalent fix via composition (see `01-Introduction/005`)

```python
class FlyingAbility:
    def move(self):
        print("Flying!")

class SwimmingAbility:
    def move(self):
        print("Swimming!")

class Bird:
    def __init__(self, movement):
        self.movement = movement
    def move(self):
        self.movement.move()

sparrow = Bird(FlyingAbility())
penguin = Bird(SwimmingAbility())
```

Both fixes share the same core insight: **don't put a method on a base type unless
every single subtype can honor it fully and unconditionally.** If even one subtype
needs to opt out, the method belongs on a more specific interface, not the shared
base.

## A useful test before adding a method to a base class

Ask: *"Can I confidently write this method's implementation once, in the base class or
as an abstract contract, such that literally every future subclass will be able to
honor it exactly as documented?"* If you can already think of an exception, don't put
it on the shared base.

Continue to `005-Exercises.md` to practice spotting and fixing LSP violations.
