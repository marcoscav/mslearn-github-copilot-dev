# Copilot Instructions for Shopping App Prototype

## PRD

# Product Requirements Document (PRD) for a Vibe Coding Prototype Shopping App

## 1. Project Summary

### Product
A static, client-side shopping prototype web application for browsing a small catalog of fruit products, viewing product details, adding items to a cart, and completing a simple checkout flow.

### Purpose
The app is intended to demonstrate the basic interaction model of an online shopping experience using a lightweight prototype built with HTML, CSS, and JavaScript.

### Target Audience
Online shoppers who want to explore a simple product catalog and experience a basic e-commerce workflow in a prototype environment.

### Goals
- Allow users to browse a set of sample fruit products.
- Let users view more detailed product information.
- Support adding products to a cart and updating cart quantities.
- Provide a simple checkout summary and order processing interaction.
- Demonstrate navigation across multiple pages in a client-side app.

## 2. Problem Overview

The prototype should show how a simple shopping interface can be built without a backend. The app should focus on demonstrating key UI and interaction patterns, including:
- product browsing
- product details
- cart management
- checkout summary

### Constraints
- The app must be static and client-side only.
- No backend functionality is required.
- No user authentication, payment processing, or database integration should be included.
- The app should use a small sample dataset for demonstration purposes.

## 3. Scope

### In Scope
- A Products page with product cards or list items
- A ProductDetails page for detailed item information
- A ShoppingCart page for quantity updates and removals
- A Checkout page for order summary and processing
- Left-side navigation between the pages
- Responsive layout behavior for desktop and narrow mobile widths
- Basic, visually appealing styling

### Out of Scope
- User accounts
- Secure payment processing
- Real-order persistence
- Backend APIs
- Inventory management
- Shipping or tax calculation
- Database integration

## 4. User Experience and Interface Requirements

### Layout
- The app should include a left-side navigation menu for moving between pages.
- On narrow screens, the navigation should collapse into abbreviated labels, such as one- or two-letter abbreviations, when the display width drops below 600 pixels.
- The interface should scale automatically to display adequately on both large screens and phone-sized screens.

### Styling
- The styling should be basic but visually appealing.
- The UI should use a clean, simple layout that is easy to understand.
- The design should prioritize readability and usability over polish or advanced responsiveness.

## 5. Page Requirements

### Products Page
The Products page should:
- Display a list of products.
- Show basic product information, including:
  - product name
  - price per unit
  - image or emoji representation
- Provide a way to select a quantity for each product.
- Allow the user to add the selected quantity to the shopping cart.

### ProductDetails Page
The ProductDetails page should:
- Display detailed information for the selected product.
- Show:
  - product name
  - description
  - price per unit
  - image or emoji representation
- Provide a way to navigate back to the Products page.

### ShoppingCart Page
The ShoppingCart page should:
- Display the list of products added to the cart.
- Include:
  - product name
  - quantity
  - total price for each product
- Allow the user to update the quantity of each item in the cart.
- Allow the user to remove products from the cart.

### Checkout Page
The Checkout page should:
- Display a summary of items being purchased.
- Include:
  - product name
  - quantity
  - price
- Clearly display the total price.
- Include an option labeled “Process Order”.

## 6. Navigation Requirements

The app must support simple navigation between the following pages:
- Products
- ProductDetails
- ShoppingCart
- Checkout

### Navigation Behavior
- A left-side menu should allow navigation between pages.
- The navigation should remain functional and visible across the app.
- When the screen width drops below 600 pixels, the navigation bar should collapse to abbreviated labels, such as one- or two-letter short forms, while still maintaining page navigation.

## 7. Sample Data

The prototype should use a small sample dataset containing 10 fruit products.

Each product should include:
- product name
- description
- price per unit
- quantity or unit type (for example: each, ounces, pounds)
- an emoji or simple image representation

### Example Product Fields
- Product Name
- Description
- Price
- Unit
- Image/Emoji

### Example Dataset Intent
Use a small, fixed catalog such as:
- Apple
- Banana
- Orange
- Strawberry
- Pineapple
- Watermelon
- Grapes
- Mango
- Peach
- Pear

## 8. Functional Requirements

### Core Use Cases
1. Browse available products.
2. Open a product’s details page.
3. Add one or more products to the cart.
4. Update quantities in the cart.
5. Remove products from the shopping cart.
6. Review checkout summary.
7. Process the order from the checkout page.

### Functional Expectations
- Product selection should update the ProductDetails page.
- Cart updates should be reflected immediately on the ShoppingCart page.
- Checkout should show a summary of the user’s selected items and total.
- All interactions should work entirely on the client side.

## 9. Technical Requirements

### Implementation Stack
- HTML
- CSS
- JavaScript

### Architecture
- Client-side web application
- Static prototype
- No backend services
- No external database

### Technical Constraints
- Use a simple dataset stored locally in the app.
- Use JavaScript state or in-memory data to simulate cart behavior.
- Use standard web technologies only unless a small, simple library is explicitly required.

## 10. Non-Functional Requirements

- The app should be easy to use and understand.
- The UI should be readable on desktop and phone-sized screens.
- The navigation menu should remain workable even on narrow display widths.
- Basic styling should help the app feel polished enough for a prototype.

## 11. Success Criteria

The prototype should be considered successful if:
- Users can navigate between the required pages.
- Users can browse products and view product details.
- Users can add items to the cart and adjust quantities.
- Users can remove items from the cart.
- Users can view a checkout summary and complete the “Process Order” action.
- The interface works on both large and small screens with basic responsive behavior.

## 12. Suggested PRD Notes for GitHub Copilot Agent

The PRD should explicitly tell the agent to:
- Build a static HTML/CSS/JavaScript prototype.
- Include the four required pages.
- Use a sample dataset of 10 fruit products.
- Implement simple cart behavior.
- Support basic navigation and mobile-friendly collapsing navigation.
- Keep the app lightweight and prototype-focused.

## Wireframe Guidance

### Products Page Wireframe

┌──────────────────────────────────────────────────────────────┐
│ Sidebar Navigation                                           │
│ [Products]  [Details]  [Cart]  [Checkout]                    │
│                                                              │
│ When width < 600px:                                         │
│ [P] [D] [C] [Ch]                                             │
└──────────────────────────────────────────────────────────────┘
│ Product Catalog                                               │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐              │
│ │ 🍎 Apple    │ │ 🍌 Banana   │ │ 🍊 Orange   │              │
│ │ $1.25 each  │ │ $0.75 each  │ │ $1.10 each  │              │
│ │ Qty: [1]    │ │ Qty: [1]    │ │ Qty: [1]    │              │
│ │ [Add to Cart]│ │ [Add to Cart]│ │ [Add to Cart]│             │
│ └─────────────┘ └─────────────┘ └─────────────┘              │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐              │
│ │ 🍓 Strawberry│ │ 🍍 Pineapple│ │ 🍉 Watermelon│            │
│ │ $2.50 each  │ │ $3.00 each  │ │ $4.00 each  │            │
│ │ Qty: [1]    │ │ Qty: [1]    │ │ Qty: [1]    │              │
│ │ [Add to Cart]│ │ [Add to Cart]│ │ [Add to Cart]│            │
│ └─────────────┘ └─────────────┘ └─────────────┘              │
└──────────────────────────────────────────────────────────────┘

### ProductDetails Page Wireframe

┌──────────────────────────────────────────────────────────────┐
│ Sidebar Navigation                                           │
│ [Products]  [Details]  [Cart]  [Checkout]                    │
│                                                              │
│ [← Back to Products]                                         │
└──────────────────────────────────────────────────────────────┘

│ Product Details                                               │
│ ┌──────────────────────────────────────────────────────────┐ │
│ │ 🍎 Apple                                                  │ │
│ │ Price: $1.25 per unit                                     │ │
│ │ Description: Crisp and sweet, perfect for snacking.      │ │
│ │                                                            │ │
│ │ [Add to Cart]                                             │ │
│ └──────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘

### ShoppingCart Page Wireframe

┌──────────────────────────────────────────────────────────────┐
│ Sidebar Navigation                                           │
│ [Products]  [Details]  [Cart]  [Checkout]                    │
└──────────────────────────────────────────────────────────────┘

│ Shopping Cart                                                 │
│ ┌──────────────────────────────────────────────────────────┐ │
│ │ 🍎 Apple    Qty: [2]   Total: $2.50                      │ │
│ │ [Update Qty] [Remove]                                    │ │
│ ├──────────────────────────────────────────────────────────┤ │
│ │ 🍌 Banana   Qty: [1]   Total: $0.75                      │ │
│ │ [Update Qty] [Remove]                                    │ │
│ ├──────────────────────────────────────────────────────────┤ │
│ │ 🍊 Orange   Qty: [3]   Total: $3.30                      │ │
│ │ [Update Qty] [Remove]                                    │ │
│ └──────────────────────────────────────────────────────────┘ │
│                                                              │
│ Subtotal: $6.55                                              │
│ [Proceed to Checkout]                                        │
└──────────────────────────────────────────────────────────────┘

### Checkout Page Wireframe

┌──────────────────────────────────────────────────────────────┐
│ Sidebar Navigation                                           │
│ [Products]  [Details]  [Cart]  [Checkout]                    │
└──────────────────────────────────────────────────────────────┘

│ Checkout                                                      │
│ ┌──────────────────────────────────────────────────────────┐ │
│ │ Order Summary                                            │ │
│ │ 🍎 Apple   Qty: 2   Price: $2.50                          │ │
│ │ 🍌 Banana  Qty: 1   Price: $0.75                          │ │
│ │ 🍊 Orange  Qty: 3   Price: $3.30                          │ │
│ │                                                          │ │
│ │ Total: $6.55                                             │ │
│ │ [Process Order]                                          │ │
│ └──────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘

### Navigation Wireframe

Expanded Navigation:

┌─────────────────────────┐
│ Products                │
│ Product Details         │
│ Shopping Cart           │
│ Checkout                │
└─────────────────────────┘

Collapsed Navigation (width < 600px):

┌─────────┐
│ P │ D │ │
│ C │ Ch │
└─────────┘

Alternative compact collapsed version:

┌─────┐
│ P   │
│ D   │
│ C   │
│ Ch  │
└─────┘
