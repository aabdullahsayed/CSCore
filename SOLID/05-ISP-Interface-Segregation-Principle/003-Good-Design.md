# ISP: Good Design — Full Before/After

## Before: one fat `Repository` interface

```python
from abc import ABC, abstractmethod

class Repository(ABC):
    @abstractmethod
    def get(self, id): ...
    @abstractmethod
    def get_all(self): ...
    @abstractmethod
    def add(self, item): ...
    @abstractmethod
    def update(self, item): ...
    @abstractmethod
    def delete(self, id): ...
    @abstractmethod
    def bulk_import(self, items): ...
```

A read-only reporting service that only ever needs `get_all()` is still forced to
depend on the entire interface:

```python
class ReportingService:
    def __init__(self, repo: Repository):
        self.repo = repo   # forced to accept something that CAN add/update/delete,
                            # even though ReportingService never calls those methods
    def generate_report(self):
        return self.repo.get_all()
```

This is dangerous: nothing stops a future maintainer from accidentally calling
`self.repo.delete(...)` inside `ReportingService`, since the dependency's type exposes
that capability even though it's conceptually a read-only consumer.

## After: segregated by client need

```python
class ReadableRepository(ABC):
    @abstractmethod
    def get(self, id): ...
    @abstractmethod
    def get_all(self): ...

class WritableRepository(ABC):
    @abstractmethod
    def add(self, item): ...
    @abstractmethod
    def update(self, item): ...
    @abstractmethod
    def delete(self, id): ...

class BulkImportable(ABC):
    @abstractmethod
    def bulk_import(self, items): ...

class SqlRepository(ReadableRepository, WritableRepository, BulkImportable):
    def get(self, id): ...
    def get_all(self): ...
    def add(self, item): ...
    def update(self, item): ...
    def delete(self, id): ...
    def bulk_import(self, items): ...
```

```python
class ReportingService:
    def __init__(self, repo: ReadableRepository):   # only depends on what it needs
        self.repo = repo
    def generate_report(self):
        return self.repo.get_all()
```

Now `ReportingService`'s dependency signature **documents its true capability
footprint**: anyone reading this class's constructor immediately knows it cannot
write or delete anything, because the type system won't let it call methods that
aren't part of `ReadableRepository`.

## Why this also improves testability

Mocking `ReadableRepository` for a unit test of `ReportingService` requires stubbing
only two methods (`get`, `get_all`), not all six — smaller, more honest test doubles.

```python
class FakeReadOnlyRepo(ReadableRepository):
    def get(self, id): return {"id": id}
    def get_all(self): return [{"id": 1}, {"id": 2}]

def test_generate_report():
    service = ReportingService(FakeReadOnlyRepo())
    assert len(service.generate_report()) == 2
```

## Rule of thumb

> Design interfaces around what **consumers** need, not around what a single
> **implementer** happens to be capable of providing all at once.

Continue to `004-Exercises.md` to practice segregating fat interfaces yourself.
