# SRP: Exercises

Work through these on paper or in your editor. Suggested solutions follow each
prompt — try not to peek until you've written your own version.

## Exercise 1: Report Generator

Refactor this class to comply with SRP.

```python
class SalesReport:
    def __init__(self, sales_data):
        self.sales_data = sales_data

    def total_sales(self):
        return sum(self.sales_data)

    def generate_html(self):
        return f"<h1>Total: {self.total_sales()}</h1>"

    def email_report(self, recipient):
        html = self.generate_html()
        print(f"Emailing {recipient}: {html}")
```

<details>
<summary>Suggested split</summary>

- `SalesReport` — holds data, computes `total_sales()`.
- `HtmlReportFormatter` — turns a `SalesReport` into HTML.
- `ReportMailer` — sends a formatted report string to a recipient.
</details>

## Exercise 2: Identify the actors

For the class below, list every distinct actor (team/person) who might request a
change, and explain which method each actor owns.

```python
class Product:
    def apply_discount(self, percent): ...
    def to_json(self): ...
    def save_to_warehouse_db(self): ...
    def log_price_change(self, old_price, new_price): ...
```

<details>
<summary>Suggested answer</summary>

- Pricing/promotions team → `apply_discount`
- API/frontend team → `to_json`
- Warehouse/inventory systems team → `save_to_warehouse_db`
- Observability/audit team → `log_price_change`

Four actors → split into `Product`, `ProductSerializer`, `ProductRepository`, and a
price-change logger (or an event the audit system subscribes to).
</details>

## Exercise 3: Spot the violation

Is the following class SRP-compliant? Justify your answer.

```python
class Matrix:
    def add(self, other): ...
    def multiply(self, other): ...
    def transpose(self): ...
    def determinant(self): ...
```

<details>
<summary>Suggested answer</summary>

Yes — despite having four methods, they all serve one actor/purpose: "matrix
mathematics." SRP is about cohesion of purpose, not method count. This class should
**not** be split further.
</details>

## Exercise 4: Real-world refactor

Take a class from a personal or work project that you suspect is a "God Class." Write
down:
1. Every distinct actor who could request a change to it.
2. A proposed split, one class per actor.
3. One unit test for each newly extracted class that would have been awkward to write
   before the split.

There is no single correct answer for this one — the goal is applying the technique to
real code.
