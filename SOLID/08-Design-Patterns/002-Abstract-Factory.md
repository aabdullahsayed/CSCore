# Design Pattern: Abstract Factory

## Intent

Provide an interface for creating **families of related objects** without specifying
their concrete classes. Where Factory Method creates one product, Abstract Factory
creates a consistent *set* of related products.

## Motivating example: cross-platform UI components

```python
from abc import ABC, abstractmethod

class Button(ABC):
    @abstractmethod
    def render(self): ...

class Checkbox(ABC):
    @abstractmethod
    def render(self): ...


class WindowsButton(Button):
    def render(self): print("Rendering a Windows-style button")

class WindowsCheckbox(Checkbox):
    def render(self): print("Rendering a Windows-style checkbox")


class MacButton(Button):
    def render(self): print("Rendering a Mac-style button")

class MacCheckbox(Checkbox):
    def render(self): print("Rendering a Mac-style checkbox")


class UIFactory(ABC):
    @abstractmethod
    def create_button(self) -> Button: ...
    @abstractmethod
    def create_checkbox(self) -> Checkbox: ...

class WindowsUIFactory(UIFactory):
    def create_button(self): return WindowsButton()
    def create_checkbox(self): return WindowsCheckbox()

class MacUIFactory(UIFactory):
    def create_button(self): return MacButton()
    def create_checkbox(self): return MacCheckbox()
```

## Client code — never touches concrete classes

```python
class Application:
    def __init__(self, factory: UIFactory):
        self.button = factory.create_button()
        self.checkbox = factory.create_checkbox()

    def render_all(self):
        self.button.render()
        self.checkbox.render()

# Composition root decides the family:
factory = WindowsUIFactory()   # or MacUIFactory()
app = Application(factory)
app.render_all()
```

## Why this matters beyond Factory Method

The key extra guarantee Abstract Factory provides: **you can never accidentally mix
families** — e.g., a `WindowsButton` paired with a `MacCheckbox`. Because
`Application` only ever calls methods on a single `UIFactory` instance, every product
it creates is guaranteed to belong to the same consistent family.

## SOLID connections

- **OCP:** adding a new platform (`LinuxUIFactory`, `LinuxButton`, `LinuxCheckbox`) is
  purely additive — `Application` never changes.
- **DIP:** `Application` depends only on the abstract `UIFactory`, `Button`, and
  `Checkbox` — never on any concrete platform class.
- **LSP:** every concrete factory and product must fully honor its abstract contract
  for the family-consistency guarantee to hold.

## When to reach for Abstract Factory vs plain Factory Method

Use Abstract Factory specifically when objects naturally come in **related groups**
that must stay consistent with each other (UI theme components, database
driver+connection+transaction sets, cloud-provider resource clients). If you're only
ever creating one kind of object, plain Factory Method is simpler and sufficient.
