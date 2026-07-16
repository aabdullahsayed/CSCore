# Case Study: Elevator System

## Requirements

- An elevator moves between floors, responding to requests.
- Different scheduling algorithms should be swappable (FCFS, nearest-floor-first,
  SCAN/elevator algorithm).
- The elevator's state (idle, moving up, moving down, doors open) drives its behavior.

## Design — combining Strategy (scheduling) and State (elevator lifecycle)

```python
from abc import ABC, abstractmethod

class SchedulingStrategy(ABC):
    @abstractmethod
    def next_floor(self, current_floor: int, requests: list[int]) -> int: ...

class FCFSScheduling(SchedulingStrategy):
    def next_floor(self, current_floor, requests):
        return requests[0]

class NearestFloorScheduling(SchedulingStrategy):
    def next_floor(self, current_floor, requests):
        return min(requests, key=lambda f: abs(f - current_floor))


class ElevatorState(ABC):
    @abstractmethod
    def open_doors(self, elevator): ...
    @abstractmethod
    def move(self, elevator, target_floor): ...

class IdleState(ElevatorState):
    def open_doors(self, elevator):
        print("Opening doors from idle.")
        elevator.state = DoorsOpenState()
    def move(self, elevator, target_floor):
        print(f"Starting to move toward floor {target_floor}")
        elevator.state = MovingState()

class MovingState(ElevatorState):
    def open_doors(self, elevator):
        print("Cannot open doors while moving.")
    def move(self, elevator, target_floor):
        elevator.current_floor = target_floor
        print(f"Arrived at floor {target_floor}")
        elevator.state = DoorsOpenState()

class DoorsOpenState(ElevatorState):
    def open_doors(self, elevator):
        print("Doors already open.")
    def move(self, elevator, target_floor):
        print("Closing doors before moving.")
        elevator.state = IdleState()
        elevator.state.move(elevator, target_floor)


class Elevator:
    def __init__(self, scheduling: SchedulingStrategy):
        self.current_floor = 0
        self.state: ElevatorState = IdleState()
        self.scheduling = scheduling
        self.requests: list[int] = []

    def request_floor(self, floor: int):
        self.requests.append(floor)

    def step(self):
        if not self.requests:
            return
        target = self.scheduling.next_floor(self.current_floor, self.requests)
        self.requests.remove(target)
        self.state.move(self, target)
```

## SOLID principles applied

- **OCP:** new scheduling algorithms (SCAN) or new states (`MaintenanceState`,
  `EmergencyState`) are added as new classes, without editing `Elevator`.
- **SRP:** scheduling logic, lifecycle/state logic, and the elevator's own
  floor-tracking are cleanly separated into different class families.
- **DIP:** `Elevator` depends on `SchedulingStrategy` and `ElevatorState`
  abstractions, not concrete algorithms.

## Extension exercise

Add an `EmergencyState` that overrides all movement to force the elevator to the
ground floor and disables `request_floor`. Confirm this doesn't require any
`isinstance()` checks anywhere in `Elevator`.
