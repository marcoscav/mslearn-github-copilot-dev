using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommercePricing
{
    public enum MembershipLevel { Guest, Silver, Gold, Premium }
    public enum SeasonalEvent { None, BlackFriday, CyberMonday, HolidayWeek, NewYear, BackToSchool }
    public enum RegionType { Domestic, International, PremiumZone }
    public enum PaymentMethod { CreditCard, DebitCard, PayPal, BankTransfer, Cryptocurrency }

    public class User
    {
        public MembershipLevel Membership { get; set; }
        public bool IsFirstTimeBuyer { get; set; }
        public int YearsAsMember { get; set; }
        public decimal LifetimeSpent { get; set; }
        public bool HasActiveSubscription { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsCorporateAccount { get; set; }
    }

    public class Coupon
    {
        public string? Code { get; set; }
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }
        public string? Type { get; set; } // "percent" or "shipping"
        public decimal Value { get; set; } // e.g., 10 for 10% off
    }

    public class Item
    {
        public string? Name { get; set; }
        public string? Category { get; set; } // e.g., "Electronics", "Clothing"
        public decimal Price { get; set; }
    }

    public class Order
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public bool IsDomestic { get; set; }
        public RegionType ShippingRegion { get; set; }
        public Coupon? Coupon { get; set; }
        public SeasonalEvent ActiveEvent { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool HasExpressShipping { get; set; }
        public bool IsPreOrder { get; set; }
        public DateTime OrderTime { get; set; }
        public bool IsBulkOrder { get; set; }
        public bool HasGiftWrap { get; set; }

        public decimal GetSubtotal() => Items.Sum(i => i.Price);
        public decimal GetSubtotalForCategory(string category) =>
            Items.Where(i => i.Category == category).Sum(i => i.Price);
        public bool ContainsCategory(string category) =>
            Items.Any(i => i.Category == category);
        public int GetCategoryItemCount(string category) =>
            Items.Count(i => i.Category == category);
        public bool IsHighValueOrder() => GetSubtotal() > 1000m;
        public bool HasMixedCategories() => Items.Select(i => i.Category).Distinct().Count() >= 3;
    }

    public class PricingEngine
    {
        // Security: Constants for validation bounds
        private const decimal MAX_DISCOUNT_PERCENT = 95m; // Maximum 95% discount
        private const decimal MIN_FINAL_PRICE = 0.01m; // Minimum $0.01 final price
        private const decimal MAX_ORDER_VALUE = 1_000_000m; // Maximum $1M order value

        public static void CalculateFinalPrice(User user, Order order)
        {
            // Security: Input validation to prevent null reference attacks
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }

            if (!IsValidOrder(order))
            {
                Console.WriteLine("Error: Invalid order data detected. Pricing calculation aborted.");
                return;
            }

            decimal baseTotal = order.GetSubtotal();

            // Security: Validate base total is within reasonable bounds
            if (baseTotal <= 0 || baseTotal > MAX_ORDER_VALUE)
            {
                Console.WriteLine($"Error: Order total ${baseTotal:F2} is outside valid range ($0.01 - ${MAX_ORDER_VALUE:N0})");
                return;
            }

            decimal discountPercent = 0m;
            decimal shippingCost = CalculateBaseShipping(order);
            var appliedDiscounts = new List<string>();

            discountPercent = ApplyMembershipDiscounts(user, order, baseTotal, discountPercent, appliedDiscounts);
            discountPercent = ApplyCouponDiscounts(user, order, discountPercent, ref shippingCost, appliedDiscounts);
            discountPercent = ApplyBulkDiscounts(order, discountPercent, appliedDiscounts);

            // Security: Final discount validation
            discountPercent = Math.Min(discountPercent, MAX_DISCOUNT_PERCENT);

            // Apply final calculations with category-specific rules
            var finalCalculation = ApplyCategorySpecificDiscounts(baseTotal, discountPercent, order);
            decimal finalPrice = Math.Max(MIN_FINAL_PRICE, finalCalculation.finalPrice + shippingCost);

            // Display results
            Console.WriteLine($"Base Total: ${baseTotal:F2}");
            Console.WriteLine($"Applied Discounts: {string.Join(", ", appliedDiscounts)}");
            Console.WriteLine($"Total Discount: {discountPercent:F1}% (Electronics capped at 15%)");
            Console.WriteLine($"Shipping Cost: ${shippingCost:F2}");
            Console.WriteLine($"Final Price: ${finalPrice:F2}");
        }

        private static decimal ApplyMembershipDiscounts(User user, Order order, decimal baseTotal, decimal discountPercent, List<string> appliedDiscounts)
        {
            if (user.Membership == MembershipLevel.Premium)
            {
                return ApplyPremiumMemberDiscounts(user, order, baseTotal, discountPercent, appliedDiscounts);
            }

            if (user.Membership == MembershipLevel.Gold)
            {
                return ApplyGoldMemberDiscounts(user, order, discountPercent, appliedDiscounts);
            }

            if (user.Membership == MembershipLevel.Silver)
            {
                return ApplySilverMemberDiscounts(user, order, discountPercent, appliedDiscounts);
            }

            if (user.IsFirstTimeBuyer)
            {
                return ApplyFirstTimeBuyerDiscounts(user, order, discountPercent, appliedDiscounts);
            }

            return discountPercent;
        }

        private static decimal ApplyPremiumMemberDiscounts(User user, Order order, decimal baseTotal, decimal discountPercent, List<string> appliedDiscounts)
        {
            discountPercent = SafeAddDiscount(discountPercent, 15, "Premium membership (15%)", appliedDiscounts);

            if (baseTotal > 10000)
            {
                discountPercent = SafeAddDiscount(discountPercent, 10, "Ultra high-value bonus (10%)", appliedDiscounts);

                if (order.ActiveEvent == SeasonalEvent.BlackFriday || order.ActiveEvent == SeasonalEvent.CyberMonday)
                {
                    discountPercent = SafeAddDiscount(discountPercent, 8, "Premium seasonal bonus (8%)", appliedDiscounts);

                    if (!user.IsCorporateAccount) return discountPercent;

                    discountPercent = SafeAddDiscount(discountPercent, 5, "Corporate account bonus (5%)", appliedDiscounts);

                    if (!user.HasActiveSubscription) return discountPercent;

                    discountPercent = SafeAddDiscount(discountPercent, 3, "Subscription service bonus (3%)", appliedDiscounts);

                    if (user.YearsAsMember < 5) return discountPercent;

                    discountPercent = SafeAddDiscount(discountPercent, 5, "Veteran premium member (5%)", appliedDiscounts);

                    if (user.LifetimeSpent <= 50000) return discountPercent;

                    discountPercent = SafeAddDiscount(discountPercent, 7, "VIP status (7%)", appliedDiscounts);

                    if (!order.HasExpressShipping) return discountPercent;

                    discountPercent = SafeAddDiscount(discountPercent, 2, "Express shipping loyalty bonus (2%)", appliedDiscounts);
                    return discountPercent;
                }

                return discountPercent;
            }

            if (baseTotal > 5000)
            {
                discountPercent = SafeAddDiscount(discountPercent, 5, "High-value bonus (5%)", appliedDiscounts);
            }

            return discountPercent;
        }

        private static decimal ApplyGoldMemberDiscounts(User user, Order order, decimal discountPercent, List<string> appliedDiscounts)
        {
            discountPercent = SafeAddDiscount(discountPercent, 12, "Gold membership (12%)", appliedDiscounts);

            if (order.ActiveEvent == SeasonalEvent.None) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 6, "Gold seasonal bonus (6%)", appliedDiscounts);

            if (order.Items.Count < 15) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 4, "Gold bulk bonus (4%)", appliedDiscounts);

            if (!order.HasMixedCategories()) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 3, "Category diversity bonus (3%)", appliedDiscounts);

            if (!user.IsEmployee) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 10, "Employee gold discount (10%)", appliedDiscounts);

            if (!order.IsPreOrder) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 5, "Pre-order employee bonus (5%)", appliedDiscounts);

            if (order.PaymentMethod != PaymentMethod.BankTransfer && order.PaymentMethod != PaymentMethod.Cryptocurrency) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 3, "Alternative payment bonus (3%)", appliedDiscounts);
            return discountPercent;
        }

        private static decimal ApplySilverMemberDiscounts(User user, Order order, decimal discountPercent, List<string> appliedDiscounts)
        {
            discountPercent = SafeAddDiscount(discountPercent, 8, "Silver membership (8%)", appliedDiscounts);

            if (!user.IsStudent) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 5, "Student silver bonus (5%)", appliedDiscounts);

            if (order.ActiveEvent != SeasonalEvent.BackToSchool) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 7, "Back-to-school bonus (7%)", appliedDiscounts);

            if (order.Items.Count < 8) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 4, "Student bulk discount (4%)", appliedDiscounts);

            if (SafeGetCategoryPercentage(order, "Electronics") <= 0.6m) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 6, "Student tech focus bonus (6%)", appliedDiscounts);

            if (!order.HasGiftWrap) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 2, "Gift presentation bonus (2%)", appliedDiscounts);

            if (!order.HasExpressShipping) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 3, "Express education bonus (3%)", appliedDiscounts);
            return discountPercent;
        }

        private static decimal ApplyFirstTimeBuyerDiscounts(User user, Order order, decimal discountPercent, List<string> appliedDiscounts)
        {
            discountPercent = SafeAddDiscount(discountPercent, 10, "First-time buyer (10%)", appliedDiscounts);

            if (order.ActiveEvent == SeasonalEvent.None) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 5, "Seasonal welcome bonus (5%)", appliedDiscounts);

            if (order.Items.Count < 5) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 4, "First-order volume bonus (4%)", appliedDiscounts);

            if (order.PaymentMethod != PaymentMethod.PayPal && order.PaymentMethod != PaymentMethod.CreditCard) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 3, "Premium payment newcomer bonus (3%)", appliedDiscounts);

            if (!order.IsHighValueOrder()) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 6, "High-value newcomer bonus (6%)", appliedDiscounts);

            if (order.ShippingRegion != RegionType.PremiumZone) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 4, "Premium zone newcomer bonus (4%)", appliedDiscounts);

            if (!order.HasExpressShipping) return discountPercent;

            discountPercent = SafeAddDiscount(discountPercent, 3, "Express shipping trial bonus (3%)", appliedDiscounts);
            return discountPercent;
        }

        private static decimal ApplyCouponDiscounts(User user, Order order, decimal discountPercent, ref decimal shippingCost, List<string> appliedDiscounts)
        {
            if (order.Coupon == null)
            {
                return discountPercent;
            }

            if (!order.Coupon.IsValid)
            {
                if (order.Coupon.IsExpired)
                {
                    appliedDiscounts.Add($"Coupon {order.Coupon.Code} expired - no discount");
                    Console.WriteLine("Coupon expired. No discount applied.");
                }

                return discountPercent;
            }

            if (order.Coupon.Type == "percent")
            {
                decimal couponValue = Math.Max(0, Math.Min(50, order.Coupon.Value));

                if (user.Membership == MembershipLevel.Premium)
                {
                    couponValue = Math.Min(50, couponValue * 1.3m);
                    appliedDiscounts.Add($"Premium-enhanced coupon {order.Coupon.Code} ({couponValue:F1}%)");

                    if (order.ActiveEvent == SeasonalEvent.BlackFriday)
                    {
                        couponValue = Math.Min(55, couponValue + 5);
                        appliedDiscounts.Add("Black Friday premium coupon boost (5%)");

                        if (user.IsCorporateAccount && order.PaymentMethod == PaymentMethod.BankTransfer)
                        {
                            couponValue = Math.Min(60, couponValue * 1.15m);
                            appliedDiscounts.Add($"Corporate payment multiplier (total: {couponValue:F1}%)");

                            if (order.IsBulkOrder)
                            {
                                couponValue = Math.Min(65, couponValue + 2);
                                appliedDiscounts.Add("Bulk corporate bonus (2%)");
                            }
                        }
                    }
                }
                else if (user.Membership == MembershipLevel.Gold)
                {
                    couponValue = Math.Min(40, couponValue * 1.2m);
                    appliedDiscounts.Add($"Gold-enhanced coupon {order.Coupon.Code} ({couponValue:F1}%)");
                }
                else
                {
                    appliedDiscounts.Add($"Coupon {order.Coupon.Code} ({couponValue}%)");
                }

                return SafeAddDiscount(discountPercent, couponValue, "", appliedDiscounts, false);
            }

            if (order.Coupon.Type == "shipping")
            {
                if (order.IsDomestic || user.Membership == MembershipLevel.Premium)
                {
                    shippingCost = 0;
                    appliedDiscounts.Add($"Free shipping coupon {order.Coupon.Code}");
                }
            }

            return discountPercent;
        }

        private static decimal ApplyBulkDiscounts(Order order, decimal discountPercent, List<string> appliedDiscounts)
        {
            if (order.Items.Count >= 20)
            {
                return SafeAddDiscount(discountPercent, 8, "Major bulk purchase (8%)", appliedDiscounts);
            }

            if (order.Items.Count >= 10)
            {
                return SafeAddDiscount(discountPercent, 5, "Bulk purchase (5%)", appliedDiscounts);
            }

            return discountPercent;
        }

        /// <summary>
        /// Security: Safe discount addition with bounds checking
        /// </summary>
        private static decimal SafeAddDiscount(decimal currentDiscount, decimal additionalDiscount, 
            string description, List<string> appliedDiscounts, bool addDescription = true)
        {
            if (additionalDiscount <= 0) return currentDiscount;
            
            decimal newTotal = currentDiscount + additionalDiscount;
            if (newTotal > MAX_DISCOUNT_PERCENT)
            {
                additionalDiscount = MAX_DISCOUNT_PERCENT - currentDiscount;
                newTotal = MAX_DISCOUNT_PERCENT;
            }

            if (addDescription && !string.IsNullOrEmpty(description))
            {
                appliedDiscounts.Add(description);
            }

            return newTotal;
        }

        /// <summary>
        /// Security: Safe category percentage calculation with division by zero protection
        /// </summary>
        private static decimal SafeGetCategoryPercentage(Order order, string category)
        {
            decimal total = order.GetSubtotal();
            if (total <= 0) return 0;
            
            return order.GetSubtotalForCategory(category) / total;
        }

        /// <summary>
        /// Security: Validates order data to prevent malicious inputs
        /// </summary>
        private static bool IsValidOrder(Order order)
        {
            if (order.Items == null || order.Items.Count == 0)
                return false;

            foreach (var item in order.Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Name) || 
                    string.IsNullOrWhiteSpace(item.Category) || item.Price < 0 || item.Price > 100000)
                    return false;
            }

            return true;
        }

        private static decimal CalculateBaseShipping(Order order)
        {
            return order.ShippingRegion switch
            {
                RegionType.Domestic => 10m,
                RegionType.International => 25m,
                RegionType.PremiumZone => 35m,
                _ => order.IsDomestic ? 10m : 25m
            };
        }

        private static (decimal finalPrice, decimal appliedDiscount) ApplyCategorySpecificDiscounts(decimal baseTotal, decimal discountPercent, Order order)
        {
            // 1. Category-specific discount application: Enhanced margin protection
            decimal electronicsSubtotal = order.GetSubtotalForCategory("Electronics");
            decimal clothingSubtotal = order.GetSubtotalForCategory("Clothing");
            decimal accessoriesSubtotal = order.GetSubtotalForCategory("Accessories");
            decimal otherSubtotal = baseTotal - electronicsSubtotal - clothingSubtotal - accessoriesSubtotal;

            // 2. Electronics discount cap: Limit electronics discount to 15% maximum
            decimal electronicsDiscount = Math.Min(discountPercent, 15);
            decimal discountedElectronics = electronicsSubtotal * (1 - electronicsDiscount / 100);

            // 2. Clothing discount cap: Seasonal fashion considerations
            decimal clothingDiscount = order.ActiveEvent == SeasonalEvent.BackToSchool ? 
                Math.Min(discountPercent, 25) : Math.Min(discountPercent, 20);
            decimal discountedClothing = clothingSubtotal * (1 - clothingDiscount / 100);

            // 2. Accessories full discount: No restrictions on accessories
            decimal discountedAccessories = accessoriesSubtotal * (1 - discountPercent / 100);

            // 2. Other categories: Apply full discount percentage
            decimal discountedOther = otherSubtotal * (1 - discountPercent / 100);

            decimal finalPrice = discountedElectronics + discountedClothing + discountedAccessories + discountedOther;
            return (finalPrice, baseTotal - finalPrice);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create test data collections
            var users = CreateTestUsers();
            var coupons = CreateTestCoupons();
            var orders = CreateTestOrders();

            // Test different pricing scenarios
            TestPricingScenarios(users, coupons, orders);

            // Run security tests to demonstrate security measures
            SecurityTest.RunSecurityTests();
        }

        static List<User> CreateTestUsers()
        {
            return new List<User>
            {
                // Basic users
                new User { Membership = MembershipLevel.Guest, IsFirstTimeBuyer = true, YearsAsMember = 0, LifetimeSpent = 0, HasActiveSubscription = false, IsStudent = false, IsEmployee = false, IsCorporateAccount = false },
                new User { Membership = MembershipLevel.Guest, IsFirstTimeBuyer = false, YearsAsMember = 0, LifetimeSpent = 200, HasActiveSubscription = false, IsStudent = true, IsEmployee = false, IsCorporateAccount = false },
                
                // Silver members
                new User { Membership = MembershipLevel.Silver, IsFirstTimeBuyer = false, YearsAsMember = 1, LifetimeSpent = 1500, HasActiveSubscription = false, IsStudent = true, IsEmployee = false, IsCorporateAccount = false },
                new User { Membership = MembershipLevel.Silver, IsFirstTimeBuyer = false, YearsAsMember = 2, LifetimeSpent = 3000, HasActiveSubscription = true, IsStudent = false, IsEmployee = false, IsCorporateAccount = false },
                
                // Gold members
                new User { Membership = MembershipLevel.Gold, IsFirstTimeBuyer = false, YearsAsMember = 3, LifetimeSpent = 8000, HasActiveSubscription = false, IsStudent = false, IsEmployee = true, IsCorporateAccount = false },
                new User { Membership = MembershipLevel.Gold, IsFirstTimeBuyer = false, YearsAsMember = 4, LifetimeSpent = 15000, HasActiveSubscription = true, IsStudent = false, IsEmployee = false, IsCorporateAccount = true },
                
                // Premium members
                new User { Membership = MembershipLevel.Premium, IsFirstTimeBuyer = false, YearsAsMember = 6, LifetimeSpent = 75000, HasActiveSubscription = true, IsStudent = false, IsEmployee = false, IsCorporateAccount = true },
                new User { Membership = MembershipLevel.Premium, IsFirstTimeBuyer = false, YearsAsMember = 8, LifetimeSpent = 120000, HasActiveSubscription = true, IsStudent = false, IsEmployee = true, IsCorporateAccount = false }
            };
        }

        static List<Coupon?> CreateTestCoupons()
        {
            return new List<Coupon?>
            {
                null, // No coupon
                new Coupon { Code = "SAVE15", IsValid = true, IsExpired = false, Type = "percent", Value = 15 },
                new Coupon { Code = "FLASHSALE25", IsValid = true, IsExpired = false, Type = "percent", Value = 25 },
                new Coupon { Code = "FREESHIP", IsValid = true, IsExpired = false, Type = "shipping", Value = 0 },
                new Coupon { Code = "EXPIRED20", IsValid = false, IsExpired = true, Type = "percent", Value = 20 }
            };
        }

        static List<Order> CreateTestOrders()
        {
            // Complex high-value order - triggers deep nesting
            var complexOrder = new Order
            {
                IsDomestic = true,
                ShippingRegion = RegionType.PremiumZone,
                ActiveEvent = SeasonalEvent.BlackFriday,
                PaymentMethod = PaymentMethod.BankTransfer,
                HasExpressShipping = true,
                IsPreOrder = true,
                IsBulkOrder = true,
                HasGiftWrap = false,
                OrderTime = DateTime.Now,
                Items = new List<Item>
                {
                    // Electronics (triggers electronics specialist logic)
                    new Item { Name = "Gaming Laptop", Category = "Electronics", Price = 3500 },
                    new Item { Name = "4K Monitor", Category = "Electronics", Price = 1200 },
                    new Item { Name = "Mechanical Keyboard", Category = "Electronics", Price = 300 },
                    new Item { Name = "Gaming Mouse", Category = "Electronics", Price = 150 },
                    new Item { Name = "VR Headset", Category = "Electronics", Price = 800 },
                    new Item { Name = "Tablet", Category = "Electronics", Price = 600 },
                    
                    // Clothing
                    new Item { Name = "Designer Jacket", Category = "Clothing", Price = 800 },
                    new Item { Name = "Premium Jeans", Category = "Clothing", Price = 200 },
                    new Item { Name = "Casual Shirt", Category = "Clothing", Price = 80 },
                    new Item { Name = "Winter Boots", Category = "Clothing", Price = 250 },
                    new Item { Name = "Formal Suit", Category = "Clothing", Price = 600 },
                    new Item { Name = "Sports Wear", Category = "Clothing", Price = 120 },
                    new Item { Name = "Evening Dress", Category = "Clothing", Price = 400 },
                    new Item { Name = "Casual Sneakers", Category = "Clothing", Price = 150 },
                    new Item { Name = "Winter Coat", Category = "Clothing", Price = 350 },
                    
                    // Accessories (3+ categories for diversity bonus)
                    new Item { Name = "Luxury Watch", Category = "Accessories", Price = 2000 },
                    new Item { Name = "Designer Bag", Category = "Accessories", Price = 500 },
                    new Item { Name = "Gold Necklace", Category = "Accessories", Price = 800 },
                    new Item { Name = "Sunglasses", Category = "Accessories", Price = 200 },
                    new Item { Name = "Leather Wallet", Category = "Accessories", Price = 100 },
                    new Item { Name = "Smart Ring", Category = "Accessories", Price = 300 }
                }
            };

            // Student order - triggers student-specific logic
            var studentOrder = new Order
            {
                IsDomestic = true,
                ShippingRegion = RegionType.Domestic,
                ActiveEvent = SeasonalEvent.BackToSchool,
                PaymentMethod = PaymentMethod.PayPal,
                HasExpressShipping = true,
                IsPreOrder = false,
                IsBulkOrder = false,
                HasGiftWrap = true,
                OrderTime = DateTime.Now,
                Items = new List<Item>
                {
                    // Electronics-focused for student tech bonus
                    new Item { Name = "Laptop", Category = "Electronics", Price = 1200 },
                    new Item { Name = "External Monitor", Category = "Electronics", Price = 300 },
                    new Item { Name = "Wireless Mouse", Category = "Electronics", Price = 50 },
                    new Item { Name = "Keyboard", Category = "Electronics", Price = 80 },
                    new Item { Name = "Webcam", Category = "Electronics", Price = 100 },
                    new Item { Name = "Headphones", Category = "Electronics", Price = 150 },
                    new Item { Name = "External Drive", Category = "Electronics", Price = 120 },
                    new Item { Name = "Tablet", Category = "Electronics", Price = 400 },
                    
                    // Some non-electronics
                    new Item { Name = "Backpack", Category = "Accessories", Price = 80 },
                    new Item { Name = "Notebook", Category = "Accessories", Price = 15 }
                }
            };

            // First-time buyer order - triggers newcomer logic
            var newcomerOrder = new Order
            {
                IsDomestic = false,
                ShippingRegion = RegionType.PremiumZone,
                ActiveEvent = SeasonalEvent.NewYear,
                PaymentMethod = PaymentMethod.CreditCard,
                HasExpressShipping = true,
                IsPreOrder = false,
                IsBulkOrder = false,
                HasGiftWrap = false,
                OrderTime = DateTime.Now,
                Items = new List<Item>
                {
                    new Item { Name = "Smartphone", Category = "Electronics", Price = 800 },
                    new Item { Name = "Case", Category = "Accessories", Price = 30 },
                    new Item { Name = "Charger", Category = "Electronics", Price = 50 },
                    new Item { Name = "Screen Protector", Category = "Accessories", Price = 20 },
                    new Item { Name = "Wireless Earbuds", Category = "Electronics", Price = 200 },
                    new Item { Name = "Power Bank", Category = "Electronics", Price = 60 }
                }
            };

            return new List<Order> { complexOrder, studentOrder, newcomerOrder };
        }

        static void TestPricingScenarios(List<User> users, List<Coupon?> coupons, List<Order> orders)
        {
            var scenarioCount = 1;
            
            // Test key complex scenarios instead of all combinations
            var keyScenarios = new[]
            {
                // Complex Premium VIP scenario - should hit nesting level 8
                (users.First(u => u.Membership == MembershipLevel.Premium && u.LifetimeSpent > 100000), 
                 coupons.First(c => c?.Code == "FLASHSALE25"), 
                 orders[0]), // Complex order
                
                // Gold employee scenario - should hit nesting level 7
                (users.First(u => u.Membership == MembershipLevel.Gold && u.IsEmployee), 
                 coupons.First(c => c?.Code == "SAVE15"), 
                 orders[0]), // Complex order with pre-order
                
                // Student Silver scenario - should hit nesting level 7
                (users.First(u => u.Membership == MembershipLevel.Silver && u.IsStudent), 
                 coupons.First(c => c?.Code == "SAVE15"), 
                 orders[1]), // Student order
                
                // First-time buyer complex scenario - should hit nesting level 7
                (users.First(u => u.IsFirstTimeBuyer), 
                 coupons.First(c => c?.Code == "FLASHSALE25"), 
                 orders[2]), // Newcomer order
                
                // Premium with expired coupon
                (users.First(u => u.Membership == MembershipLevel.Premium && u.LifetimeSpent > 100000), 
                 coupons.First(c => c?.Code == "EXPIRED20"), 
                 orders[0]),
                
                // No coupon scenarios
                (users.First(u => u.Membership == MembershipLevel.Gold && u.IsEmployee), 
                 null, 
                 orders[0])
            };

            foreach (var (user, coupon, order) in keyScenarios)
            {
                // Clone order and assign coupon to avoid modifying original
                var testOrder = new Order
                {
                    IsDomestic = order.IsDomestic,
                    ShippingRegion = order.ShippingRegion,
                    ActiveEvent = order.ActiveEvent,
                    PaymentMethod = order.PaymentMethod,
                    HasExpressShipping = order.HasExpressShipping,
                    IsPreOrder = order.IsPreOrder,
                    IsBulkOrder = order.IsBulkOrder,
                    HasGiftWrap = order.HasGiftWrap,
                    OrderTime = order.OrderTime,
                    Coupon = coupon,
                    Items = order.Items
                };

                Console.WriteLine($"\n=== COMPLEX SCENARIO {scenarioCount++} ===");
                Console.WriteLine($"User: {user.Membership} membership, {user.YearsAsMember} years, Lifetime: ${user.LifetimeSpent:F0}");
                Console.WriteLine($"      First-time: {user.IsFirstTimeBuyer}, Student: {user.IsStudent}, Employee: {user.IsEmployee}");
                Console.WriteLine($"      Corporate: {user.IsCorporateAccount}, Subscription: {user.HasActiveSubscription}");
                Console.WriteLine($"Order: {testOrder.Items.Count} items, Total: ${testOrder.GetSubtotal():F2}");
                Console.WriteLine($"       Event: {testOrder.ActiveEvent}, Payment: {testOrder.PaymentMethod}, Express: {testOrder.HasExpressShipping}");
                Console.WriteLine($"       Pre-order: {testOrder.IsPreOrder}, Bulk: {testOrder.IsBulkOrder}, Gift Wrap: {testOrder.HasGiftWrap}");
                Console.WriteLine($"       Region: {testOrder.ShippingRegion}");
                Console.WriteLine($"Electronics: ${testOrder.GetSubtotalForCategory("Electronics"):F2}, " +
                                $"Clothing: ${testOrder.GetSubtotalForCategory("Clothing"):F2}, " +
                                $"Accessories: ${testOrder.GetSubtotalForCategory("Accessories"):F2}");
                
                if (coupon != null)
                {
                    Console.WriteLine($"Coupon: {coupon.Code} ({coupon.Type}, {coupon.Value}%, Valid: {coupon.IsValid}, Expired: {coupon.IsExpired})");
                }
                else
                {
                    Console.WriteLine("Coupon: None");
                }
                
                Console.WriteLine("--- COMPLEX PRICING CALCULATION ---");
                PricingEngine.CalculateFinalPrice(user, testOrder);
                Console.WriteLine(new string('=', 60));
            }
        }
    }
}
