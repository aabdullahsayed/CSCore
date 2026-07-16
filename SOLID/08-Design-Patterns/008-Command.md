# Design Pattern: Command

## Intent

Encapsulate a request (an action plus its parameters) as a standalone object, so you
can parameterize clients with different requests, queue or log requests, and support
undoable operations.

## The problem: tightly coupling an invoker to specific actions

```python
class Button:
    def click(self):
        # directly hardcodes exactly what happens on click
        light = Light()
        light.turn_on()
```

If you want the same `Button` class to trigger different actions (turn on a light,
open a garage door, play music) depending on configuration, hardcoding the action
inside `click()` doesn't scale — and violates OCP (every new action means editing
`Button`).

## The pattern

```python
from abc import ABC, abstractmethod

class Command(ABC):
    @abstractmethod
    def execute(self): ...
    @abstractmethod
    def undo(self): ...

class Light:
    def on(self): print("Light ON")
    def off(self): print("Light OFF")

class LightOnCommand(Command):
    def __init__(self, light: Light):
        self.light = light
    def execute(self):
        self.light.on()
    def undo(self):
        self.light.off()

class GarageDoor:
    def open(self): print("Garage door OPEN")
    def close(self): print("Garage door CLOSE")

class GarageOpenCommand(Command):
    def __init__(self, door: GarageDoor):
        self.door = door
    def execute(self):
        self.door.open()
    def undo(self):
        self.door.close()


class Button:
    def __init__(self, command: Command):
        self.command = command
    def click(self):
        self.command.execute()
```

## Usage, including undo history

```python
history = []

def press(button: Button):
    button.command.execute()
    history.append(button.command)

def undo_last():
    if history:
        history.pop().undo()

light_button = Button(LightOnCommand(Light()))
press(light_button)   # Light ON
undo_last()            # Light OFF
```

## SOLID connections

- **OCP:** `Button` supports any new action (`GarageOpenCommand`,
  `PlayMusicCommand`, ...) without ever being modified — it only knows the `Command`
  interface.
- **SRP:** each command class has exactly one job: perform (and reverse) one specific
  action.
- **DIP:** `Button` depends on the abstract `Command`, never on `Light` or
  `GarageDoor` directly.

## Real-world use cases

Undo/redo stacks in editors, task queues/job schedulers (a `Command` object can be
serialized and executed later or on a different machine), macro recording (store a
list of `Command` objects, replay them), and GUI menu items/toolbar buttons that all
trigger different app actions through a uniform interface.
