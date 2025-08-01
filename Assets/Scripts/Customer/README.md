# Customer System Documentation

This document describes the comprehensive customer system implemented for the GMTK2025 game.

## Overview

The customer system consists of several interconnected components that work together to create a dynamic customer experience:

- **Customer.cs** - Main customer behavior and interaction logic
- **CustomerManager.cs** - Manages customer spawning and overall system coordination
- **CustomerProgressBar.cs** - UI component for displaying customer patience
- **CustomerPrefab.cs** - Helper script for setting up customer prefabs
- **CustomerData.cs** - ScriptableObject for defining customer types and personalities

## Core Components

### 1. Customer.cs

The main customer script that handles:
- Movement along waypoints to seats
- Patience system with visual progress bar
- Order validation and response
- Dialogue integration
- State management (Walking, Seated, HasOrdered, Impatient, Satisfied, Enraged)

**Key Features:**
- Automatic movement from spawn point to assigned seat
- Patience timer that affects customer behavior
- Integration with the product system (using int IDs instead of strings)
- Event-driven system for customer satisfaction/enragement
- Progress bar UI integration

**Important Changes Made:**
- Changed from string-based product IDs to int-based IDs to match ProductData
- Integrated with FridgeItemSelector for product selection
- Added static product tracking system
- Improved dialogue end handling

### 2. CustomerManager.cs

Manages the overall customer system:
- Automatic customer spawning
- Seat management
- Customer type configuration
- Statistics tracking

**Key Features:**
- Configurable spawn intervals and maximum customers
- Dynamic seat assignment
- Customer type-based configuration
- Event system for customer lifecycle

### 3. CustomerProgressBar.cs

Handles the visual representation of customer patience:
- Animated progress bar
- Color-coded patience levels (green → yellow → red)
- Status text display
- Automatic show/hide based on customer state

### 4. CustomerData.cs

ScriptableObject for defining customer personalities:
- Personality traits (patience, politeness, generosity)
- Order preferences and dislikes
- Dialogue options
- Reward calculations

### 5. CustomerPrefab.cs

Helper script for setting up customer prefabs:
- Automatic component validation
- Progress bar UI creation
- Prefab setup automation

## Setup Instructions

### 1. Create Customer Prefab

1. Create a new GameObject in your scene
2. Add the `CustomerPrefab` component
3. Right-click the component and select "Auto-Setup Customer Prefab"
4. This will automatically add all required components

### 2. Set Up CustomerManager

1. Create a new GameObject and add the `CustomerManager` component
2. Assign your customer prefab to the `customerPrefab` field
3. Set up spawn points and seat positions
4. Assign your `ProductDatabase` and `CustomerData` assets
5. Configure spawn intervals and behavior settings

### 3. Create Customer Types

1. Right-click in the Project window → Create → Customer → Customer Data
2. Configure the customer's personality traits
3. Set preferred and disliked dish IDs
4. Assign dialogues and possible recipes
5. Use the "Validate Customer Data" context menu option to check for issues

### 4. Set Up Product Integration

The customer system now integrates with the existing product system:
- Products are selected from the fridge using `FridgeItemSelector`
- Customer validates products using their `acceptableDishIds` and `perfectDishId`
- Products are automatically "consumed" when given to customers

## Usage Examples

### Basic Customer Interaction

```csharp
// Customer automatically becomes interactable when seated
// Player can interact to start dialogue and give orders
customer.Interact(); // Called automatically by InteractionDetector
```

### Manual Customer Spawning

```csharp
CustomerManager manager = FindObjectOfType<CustomerManager>();
manager.SpawnCustomer();
```

### Customer Statistics

```csharp
CustomerManager manager = FindObjectOfType<CustomerManager>();
int totalServed = manager.GetTotalCustomersServed();
int satisfied = manager.GetSatisfiedCustomers();
int enraged = manager.GetEnragedCustomers();
```

### Product Integration

```csharp
// Products are automatically tracked when selected from fridge
// Customer system listens to FridgeItemSelector.OnItemSelected event
// Products are validated against customer preferences
```

## Configuration

### Customer Behavior Settings

- **Patience Time**: How long customer waits before getting impatient
- **Approach Patience Time**: How long customer waits before leaving if not approached
- **Annoyed Patience Time**: Reduced patience time after asking for hints

### Spawning Settings

- **Spawn Interval**: Time between customer spawns
- **Max Customers**: Maximum number of customers in the cafe
- **Min/Max Patience Times**: Range for random patience values

### Visual Settings

- **Progress Bar Colors**: Green (normal), Yellow (warning), Red (danger)
- **Animation Speed**: How fast the progress bar animates
- **Warning/Danger Thresholds**: When colors change

## Events

The customer system provides several events for integration:

- `onCustomerSpawned` - Triggered when a new customer spawns
- `onCustomerLeft` - Triggered when any customer leaves
- `onCustomerSatisfied` - Triggered when a customer is satisfied
- `onCustomerEnraged` - Triggered when a customer is enraged
- `onFamilyRecipeShared` - Triggered when a perfect dish is given
- `onAllCustomersSatisfied` - Triggered when all customers are satisfied

## Troubleshooting

### Common Issues

1. **Customer not moving**: Check waypoints are assigned
2. **No interaction**: Ensure customer is seated and `IsInteractable` is true
3. **Progress bar not showing**: Check `CustomerProgressBar` component setup
4. **Products not validating**: Verify product IDs match between `ProductData` and customer preferences

### Debug Commands

- `DebugSpawnCustomer()` - Manually spawn a customer
- `DebugClearAllCustomers()` - Remove all customers
- `ValidateCustomerPrefab()` - Check prefab setup
- `ValidateCustomerData()` - Check customer data configuration

## Future Enhancements

Potential improvements for the customer system:

1. **Customer Varieties**: Different customer sprites and animations
2. **Time-based Behavior**: Different customer types at different times
3. **Reputation System**: Customer satisfaction affects future customers
4. **Special Events**: Customer events and challenges
5. **Advanced AI**: More complex customer decision making
6. **Customer Relationships**: Returning customers with memory
7. **Seasonal Variations**: Different customer types based on seasons/events 