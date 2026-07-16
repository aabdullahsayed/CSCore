# Design Pattern: Template Method

## Intent

Define the skeleton of an algorithm in a base class method, deferring some steps to
subclasses. Template Method lets subclasses redefine specific steps of an algorithm
without changing the algorithm's overall structure — a controlled, narrow use of
inheritance that (done carefully) stays LSP-compliant.

## The pattern

```python
from abc import ABC, abstractmethod

class DataExporter(ABC):
    def export(self, data):
        """Template method — defines the fixed skeleton."""
        prepared = self.prepare(data)
        formatted = self.format(prepared)
        self.write(formatted)
        print("Export complete.")

    def prepare(self, data):
        """Default step — subclasses may override if needed."""
        return [row for row in data if row is not None]

    @abstractmethod
    def format(self, data):
        """Required step — every subclass must define this."""
        ...

    @abstractmethod
    def write(self, formatted_data):
        """Required step — every subclass must define this."""
        ...


class CsvExporter(DataExporter):
    def format(self, data):
        return "\n".join(",".join(map(str, row)) for row in data)

    def write(self, formatted_data):
        print(f"Writing CSV:\n{formatted_data}")


class JsonExporter(DataExporter):
    def format(self, data):
        import json
        return json.dumps(data)

    def write(self, formatted_data):
        print(f"Writing JSON:\n{formatted_data}")
```

## Usage

```python
CsvExporter().export([[1, "a"], [2, "b"]])
JsonExporter().export([[1, "a"], [2, "b"]])
```

Both exporters share the exact same overall algorithm (`prepare` → `format` → `write`
→ log completion), but customize the parts that genuinely vary.

## SOLID connections

- **OCP:** adding an `XmlExporter` requires implementing two abstract methods — the
  shared `export()` skeleton in `DataExporter` never changes.
- **LSP done right:** unlike the "bird that can't fly" anti-pattern, every subclass
  here is *required* by the abstract base to implement `format`/`write`, and the base
  class's `export()` behavior is fully honored by every subclass — there's no
  surprising override that breaks the contract, because the contract explicitly
  delegates those two steps to subclasses from the start.

## Template Method vs Strategy — when to pick which

- **Template Method** uses inheritance: subclasses override *specific steps* of an
  algorithm whose overall shape is fixed and shared.
- **Strategy** uses composition: the *entire* algorithm is swapped out as one
  interchangeable object, with no shared base implementation.

Prefer Strategy when you want to swap the whole algorithm at runtime, or avoid
inheritance's tighter coupling; prefer Template Method when several variants
genuinely share a fixed sequence of steps and only need to customize a few of them.
