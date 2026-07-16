# OCP: Exercises

## Exercise 1: Tax Calculator

Refactor this function so a new country's tax rule can be added without editing this
function.

```python
def calculate_tax(country, amount):
    if country == "US":
        return amount * 0.07
    elif country == "UK":
        return amount * 0.20
    elif country == "DE":
        return amount * 0.19
    else:
        raise ValueError("Unsupported country")
```

<details>
<summary>Suggested solution</summary>

```python
from abc import ABC, abstractmethod

class TaxStrategy(ABC):
    @abstractmethod
    def calculate(self, amount: float) -> float: ...

class USTax(TaxStrategy):
    def calculate(self, amount): return amount * 0.07

class UKTax(TaxStrategy):
    def calculate(self, amount): return amount * 0.20

class DETax(TaxStrategy):
    def calculate(self, amount): return amount * 0.19

TAX_REGISTRY = {"US": USTax(), "UK": UKTax(), "DE": DETax()}

def calculate_tax(country, amount):
    return TAX_REGISTRY[country].calculate(amount)
```

Adding France means writing `FRTax` and adding one registry line — `calculate_tax`
itself never changes again.
</details>

## Exercise 2: File Exporter

This class violates OCP. Identify why, and refactor it.

```python
class ReportExporter:
    def export(self, report, format):
        if format == "csv":
            return ",".join(report.rows)
        elif format == "json":
            import json
            return json.dumps(report.rows)
        elif format == "xml":
            return "<report>" + "".join(f"<row>{r}</row>" for r in report.rows) + "</report>"
```

<details>
<summary>Hint</summary>

Extract an `Exporter` interface with `export(report) -> str`, then create
`CsvExporter`, `JsonExporter`, `XmlExporter`. `ReportExporter.export` should delegate
to the chosen exporter instance instead of branching on a string.
</details>

## Exercise 3: When NOT to apply OCP

Explain why the following is *not* a good candidate for the Strategy pattern, even
though it's an `if/elif` chain:

```python
def describe_day(is_weekend):
    if is_weekend:
        return "Relax!"
    else:
        return "Back to work."
```

<details>
<summary>Suggested answer</summary>

This branches on a boolean with exactly two, permanently fixed outcomes — there is no
realistic "third day type" coming in the future, and no team will ever need to add a
new case independently. Introducing an interface and two strategy classes here would
be pure ceremony with no benefit. OCP should be reserved for axes of variation that
are genuinely expected to grow.
</details>

## Exercise 4: Design from scratch

Design an OCP-compliant image-filter pipeline where new filters (grayscale, blur,
sepia, invert) can be added as plug-ins without modifying the pipeline class. Sketch
the interface and two concrete filter classes.
