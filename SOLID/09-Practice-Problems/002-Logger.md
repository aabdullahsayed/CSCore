# Practice Problem: Logger

## The problem

This `Logger` violates DIP (business code depends directly on a concrete file-writing
class) and OCP (adding a new log destination means editing the class).

```python
class Logger:
    def log(self, message, destination="file"):
        if destination == "file":
            with open("app.log", "a") as f:
                f.write(message + "\n")
        elif destination == "console":
            print(message)
        elif destination == "network":
            print(f"Sending to remote log server: {message}")
```

## Requirements

1. `Logger` (or whatever orchestrates logging) must not know about file I/O, console
   printing, or network calls directly — those must live behind an abstraction.
2. Adding a new destination (e.g., a database logger) must not require editing
   existing code.
3. It should be possible to log to *multiple* destinations at once.

## Suggested solution

```python
from abc import ABC, abstractmethod

class LogHandler(ABC):
    @abstractmethod
    def handle(self, message: str) -> None: ...

class FileLogHandler(LogHandler):
    def __init__(self, path="app.log"):
        self.path = path
    def handle(self, message):
        with open(self.path, "a") as f:
            f.write(message + "\n")

class ConsoleLogHandler(LogHandler):
    def handle(self, message):
        print(message)

class NetworkLogHandler(LogHandler):
    def handle(self, message):
        print(f"Sending to remote log server: {message}")


class Logger:
    def __init__(self, handlers: list[LogHandler]):
        self.handlers = handlers

    def log(self, message: str):
        for handler in self.handlers:
            handler.handle(message)
```

## Usage

```python
logger = Logger([FileLogHandler(), ConsoleLogHandler()])
logger.log("Application started")   # writes to file AND prints to console
```

Adding a `DatabaseLogHandler` is one new class passed into the constructor list —
`Logger` itself never changes. This mirrors the Observer pattern
(`08-Design-Patterns/005-Observer.md`) applied to logging.
