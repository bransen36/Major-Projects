#include <iostream>
#include <fstream>
#include <vector>
#include <iomanip>

using namespace std;

// Product class to store inventory items
class Product {
public:
    int id;
    string name;
    double price;
    int quantity;

    Product(int pid, string pname, double pprice, int pquantity)
        : id(pid), name(pname), price(pprice), quantity(pquantity) {}

    // Display product details
    void display() const {
        cout << left << setw(10) << id << setw(20) << name 
             << setw(10) << price << setw(10) << quantity << endl;
    }
};

// Inventory class to manage products
class Inventory {
private:
    vector<Product> products;
    const string filename = "inventory.txt";

public:
    Inventory() {
        loadFromFile();  // Load existing data on startup
    }

    // Add a new product
    void addProduct(int id, string name, double price, int quantity) {
        products.emplace_back(id, name, price, quantity);
        saveToFile();
    }

    // Remove product by ID
    void removeProduct(int id) {
        for (auto it = products.begin(); it != products.end(); ++it) {
            if (it->id == id) {
                products.erase(it);
                saveToFile();
                cout << "Product removed successfully!\n";
                return;
            }
        }
        cout << "Product ID not found!\n";
    }

    // Update product details
    void updateProduct(int id, double newPrice, int newQuantity) {
        for (auto &prod : products) {
            if (prod.id == id) {
                prod.price = newPrice;
                prod.quantity = newQuantity;
                saveToFile();
                cout << "Product updated successfully!\n";
                return;
            }
        }
        cout << "Product ID not found!\n";
    }

    // Display all products
    void displayInventory() const {
        if (products.empty()) {
            cout << "No products in inventory.\n";
            return;
        }
        cout << left << setw(10) << "ID" << setw(20) << "Name" 
             << setw(10) << "Price" << setw(10) << "Quantity" << endl;
        cout << string(50, '-') << endl;
        for (const auto &prod : products) {
            prod.display();
        }
    }

    // Save inventory to a file
    void saveToFile() {
        ofstream file(filename);
        if (!file) {
            cout << "Error saving data!\n";
            return;
        }
        for (const auto &prod : products) {
            file << prod.id << "," << prod.name << "," 
                 << prod.price << "," << prod.quantity << endl;
        }
        file.close();
    }

    // Load inventory from a file
    void loadFromFile() {
        ifstream file(filename);
        if (!file) return;

        products.clear();
        int id, quantity;
        double price;
        string name;
        while (file >> id) {
            file.ignore(); // Ignore comma
            getline(file, name, ',');
            file >> price;
            file.ignore();
            file >> quantity;
            products.emplace_back(id, name, price, quantity);
        }
        file.close();
    }
};

// Main menu
int main() {
    Inventory inventory;
    int choice, id, quantity;
    double price;
    string name;

    while (true) {
        cout << "\nInventory Management System\n";
        cout << "1. Add Product\n";
        cout << "2. Remove Product\n";
        cout << "3. Update Product\n";
        cout << "4. Display Inventory\n";
        cout << "5. Exit\n";
        cout << "Enter choice: ";
        cin >> choice;

        switch (choice) {
            case 1:
                cout << "Enter ID: ";
                cin >> id;
                cout << "Enter Name: ";
                cin.ignore();
                getline(cin, name);
                cout << "Enter Price: ";
                cin >> price;
                cout << "Enter Quantity: ";
                cin >> quantity;
                inventory.addProduct(id, name, price, quantity);
                break;
            case 2:
                cout << "Enter Product ID to remove: ";
                cin >> id;
                inventory.removeProduct(id);
                break;
            case 3:
                cout << "Enter Product ID to update: ";
                cin >> id;
                cout << "Enter New Price: ";
                cin >> price;
                cout << "Enter New Quantity: ";
                cin >> quantity;
                inventory.updateProduct(id, price, quantity);
                break;
            case 4:
                inventory.displayInventory();
                break;
            case 5:
                cout << "Exiting...\n";
                return 0;
            default:
                cout << "Invalid choice. Try again!\n";
        }
    }
}
