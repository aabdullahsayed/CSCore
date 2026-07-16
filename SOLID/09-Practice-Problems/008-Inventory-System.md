# Practice Problem: Inventory System

## The problem

Design an inventory system that needs to: (1) support different restocking rules per
product category (perishable items reorder differently than electronics), and (2)
support multiple suppliers with different ordering APIs, without the core
`InventoryManager` knowing any category-specific or supplier-specific details.

## Requirements

1. Each product category encapsulates its own restock-threshold logic (SRP + OCP).
2. Each supplier integration is isolated behind a common interface (DIP), so swapping
   or adding suppliers doesn't touch `InventoryManager`.
3. `InventoryManager` should be a thin orchestrator, not a God Class.

## Suggested solution

```python
from abc import ABC, abstractmethod

class RestockPolicy(ABC):
    @abstractmethod
    def should_reorder(self, current_stock: int) -> bool: ...
    @abstractmethod
    def reorder_quantity(self) -> int: ...

class PerishableRestockPolicy(RestockPolicy):
    """Reorder frequently, in small batches, to minimize spoilage."""
    def should_reorder(self, current_stock):
        return current_stock < 10
    def reorder_quantity(self):
        return 20

class ElectronicsRestockPolicy(RestockPolicy):
    """Reorder rarely, in large batches."""
    def should_reorder(self, current_stock):
        return current_stock < 5
    def reorder_quantity(self):
        return 50


class SupplierGateway(ABC):
    @abstractmethod
    def place_order(self, product_name: str, quantity: int) -> None: ...

class AcmeSupplier(SupplierGateway):
    def place_order(self, product_name, quantity):
        print(f"[Acme API] Ordering {quantity} of {product_name}")

class GlobalSupplyCoSupplier(SupplierGateway):
    def place_order(self, product_name, quantity):
        print(f"[GlobalSupplyCo SOAP call] Order: {product_name} x{quantity}")


class Product:
    def __init__(self, name, current_stock, restock_policy: RestockPolicy,
                 supplier: SupplierGateway):
        self.name = name
        self.current_stock = current_stock
        self.restock_policy = restock_policy
        self.supplier = supplier


class InventoryManager:
    def check_and_reorder(self, product: Product):
        if product.restock_policy.should_reorder(product.current_stock):
            qty = product.restock_policy.reorder_quantity()
            product.supplier.place_order(product.name, qty)
```

## Usage

```python
milk = Product("Milk", current_stock=4, restock_policy=PerishableRestockPolicy(),
                supplier=AcmeSupplier())
laptop = Product("Laptop", current_stock=8, restock_policy=ElectronicsRestockPolicy(),
                  supplier=GlobalSupplyCoSupplier())

manager = InventoryManager()
manager.check_and_reorder(milk)     # triggers reorder (4 < 10)
manager.check_and_reorder(laptop)   # no reorder (8 >= 5)
```

Adding a `FrozenGoodsRestockPolicy` or a new `FastFreightSupplier` requires zero
changes to `InventoryManager` — the whole point of applying OCP and DIP together.
