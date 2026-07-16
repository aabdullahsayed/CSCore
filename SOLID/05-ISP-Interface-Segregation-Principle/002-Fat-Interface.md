# ISP: The Fat Interface Problem

A classic real-world case: a `Printer` interface for an office equipment SDK.

```python
from abc import ABC, abstractmethod

class MultiFunctionDevice(ABC):
    @abstractmethod
    def print_document(self, doc): ...
    @abstractmethod
    def scan_document(self, doc): ...
    @abstractmethod
    def fax_document(self, doc): ...
    @abstractmethod
    def staple_document(self, doc): ...
```

This models a high-end office copier well. But now a vendor wants to add support for a
**basic inkjet printer** that only prints:

```python
class BasicInkjetPrinter(MultiFunctionDevice):
    def print_document(self, doc):
        print(f"Printing: {doc}")

    def scan_document(self, doc):
        raise NotImplementedError("This printer cannot scan")

    def fax_document(self, doc):
        raise NotImplementedError("This printer cannot fax")

    def staple_document(self, doc):
        raise NotImplementedError("This printer cannot staple")
```

## Symptoms of the violation

1. **Three out of four methods are lies.** `BasicInkjetPrinter` implements an
   interface it can only honor 25% of.
2. **Client code becomes unsafe.** Any code written generically against
   `MultiFunctionDevice` that calls `.scan_document()` will crash at runtime if handed
   a `BasicInkjetPrinter` — this is the LSP consequence of an ISP violation.
3. **Every new device type must stub out irrelevant methods**, cluttering the codebase
   with `NotImplementedError` boilerplate that grows linearly with the number of
   device types and interface methods.
4. **The interface can't evolve safely.** Adding a new method to
   `MultiFunctionDevice` (say, `email_document`) forces *every* implementer — even
   ones that will obviously never support it — to be touched.

## The general shape of this smell

Any interface where:
- implementers routinely throw `NotImplementedError` / return `None` / no-op for a
  subset of methods, or
- the interface has "capability flags" like `can_scan()` that client code checks
  before calling the corresponding method,

...is a fat interface begging to be split.

## Segregating by capability

```python
class Printer(ABC):
    @abstractmethod
    def print_document(self, doc): ...

class Scanner(ABC):
    @abstractmethod
    def scan_document(self, doc): ...

class FaxMachine(ABC):
    @abstractmethod
    def fax_document(self, doc): ...

class Stapler(ABC):
    @abstractmethod
    def staple_document(self, doc): ...

class BasicInkjetPrinter(Printer):
    def print_document(self, doc):
        print(f"Printing: {doc}")

class OfficeMultiFunctionDevice(Printer, Scanner, FaxMachine, Stapler):
    def print_document(self, doc): print(f"Printing: {doc}")
    def scan_document(self, doc): print(f"Scanning: {doc}")
    def fax_document(self, doc): print(f"Faxing: {doc}")
    def staple_document(self, doc): print(f"Stapling: {doc}")
```

Now client code that only needs to print depends only on `Printer` — and both device
types satisfy it truthfully, with no fake/throwing methods anywhere.

Continue to `003-Good-Design.md` for a full before/after with client code included.
