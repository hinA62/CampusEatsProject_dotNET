-- ============================================
-- CampusEats Database Seed Script
-- Populates MenuItems, Menus, and Orders tables
-- ============================================

-- Clear existing data (in correct order due to dependencies)
DELETE FROM "Orders";
DELETE FROM "InventoryDayItems";
DELETE FROM "InventoryDays";
DELETE FROM "Menus";
DELETE FROM "MenuItems";

-- ============================================
-- 1. INSERT MENU ITEMS (Individual dishes)
-- ============================================

-- Breakfast Items
INSERT INTO "MenuItems" ("Id", "Name", "Price", "ImageUrl", "Allergens") VALUES
('11111111-1111-1111-1111-111111111101', 'Scrambled Eggs', 4.99, 'https://images.unsplash.com/photo-1525351484163-7529414344d8', '["eggs"]'),
('11111111-1111-1111-1111-111111111102', 'Pancakes', 5.49, 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445', '["gluten", "dairy", "eggs"]'),
('11111111-1111-1111-1111-111111111103', 'Croissant', 3.99, 'https://images.unsplash.com/photo-1555507036-ab1f4038808a', '["gluten", "dairy"]'),
('11111111-1111-1111-1111-111111111104', 'Greek Yogurt Bowl', 6.49, 'https://images.unsplash.com/photo-1488477181946-6428a0291777', '["dairy"]'),
('11111111-1111-1111-1111-111111111105', 'Avocado Toast', 7.99, 'https://images.unsplash.com/photo-1541519227354-08fa5d50c44d', '["gluten"]'),

-- Main Courses
('11111111-1111-1111-1111-111111111201', 'Grilled Chicken Breast', 12.99, 'https://images.unsplash.com/photo-1532550907401-a500c9a57435', null),
('11111111-1111-1111-1111-111111111202', 'Beef Burger', 11.49, 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd', '["gluten", "dairy"]'),
('11111111-1111-1111-1111-111111111203', 'Margherita Pizza', 10.99, 'https://images.unsplash.com/photo-1574071318508-1cdbab80d002', '["gluten", "dairy"]'),
('11111111-1111-1111-1111-111111111204', 'Caesar Salad', 8.99, 'https://images.unsplash.com/photo-1546793665-c74683f339c1', '["dairy", "fish"]'),
('11111111-1111-1111-1111-111111111205', 'Pasta Carbonara', 13.49, 'https://images.unsplash.com/photo-1612874742237-6526221588e3', '["gluten", "dairy", "eggs"]'),
('11111111-1111-1111-1111-111111111206', 'Grilled Salmon', 16.99, 'https://images.unsplash.com/photo-1467003909585-2f8a72700288', '["fish"]'),
('11111111-1111-1111-1111-111111111207', 'Vegetable Stir Fry', 9.99, 'https://images.unsplash.com/photo-1512058564366-18510be2db19', '["soy"]'),
('11111111-1111-1111-1111-111111111208', 'Falafel Wrap', 8.49, 'https://images.unsplash.com/photo-1529006557810-274b9b2fc783', '["gluten", "sesame"]'),

-- Sides
('11111111-1111-1111-1111-111111111301', 'French Fries', 3.99, 'https://images.unsplash.com/photo-1576107232684-1279f390859f', null),
('11111111-1111-1111-1111-111111111302', 'Sweet Potato Fries', 4.49, 'https://images.unsplash.com/photo-1573080496219-bb080dd4f877', null),
('11111111-1111-1111-1111-111111111303', 'Coleslaw', 3.49, 'https://images.unsplash.com/photo-1580013759032-c96505e24c1f', '["dairy"]'),
('11111111-1111-1111-1111-111111111304', 'Garlic Bread', 4.99, 'https://images.unsplash.com/photo-1573140401552-388ed0f91b3c', '["gluten", "dairy"]'),
('11111111-1111-1111-1111-111111111305', 'Mixed Greens Salad', 5.99, 'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe', null),

-- Desserts
('11111111-1111-1111-1111-111111111401', 'Chocolate Brownie', 4.99, 'https://images.unsplash.com/photo-1564355808853-1c8f77b11f97', '["gluten", "dairy", "eggs"]'),
('11111111-1111-1111-1111-111111111402', 'Cheesecake', 5.99, 'https://images.unsplash.com/photo-1533134486753-c833f0ed4866', '["gluten", "dairy", "eggs"]'),
('11111111-1111-1111-1111-111111111403', 'Apple Pie', 4.49, 'https://images.unsplash.com/photo-1535920527002-b35e96722eb9', '["gluten", "dairy"]'),
('11111111-1111-1111-1111-111111111404', 'Ice Cream Sundae', 5.49, 'https://images.unsplash.com/photo-1563805042-7684c019e1cb', '["dairy", "nuts"]'),
('11111111-1111-1111-1111-111111111405', 'Fruit Salad', 4.99, 'https://images.unsplash.com/photo-1564093497595-593b96d80180', null),

-- Drinks
('11111111-1111-1111-1111-111111111501', 'Orange Juice', 2.99, 'https://images.unsplash.com/photo-1600271886742-f049cd451bba', null),
('11111111-1111-1111-1111-111111111502', 'Coffee', 2.49, 'https://images.unsplash.com/photo-1509042239860-f550ce710b93', null),
('11111111-1111-1111-1111-111111111503', 'Cappuccino', 3.99, 'https://images.unsplash.com/photo-1572442388796-11668a67e53d', '["dairy"]'),
('11111111-1111-1111-1111-111111111504', 'Iced Tea', 2.49, 'https://images.unsplash.com/photo-1556679343-c7306c1976bc', null),
('11111111-1111-1111-1111-111111111505', 'Smoothie', 4.99, 'https://images.unsplash.com/photo-1505252585461-04db1eb84625', '["dairy"]');

-- ============================================
-- 2. INSERT MENUS (Combo meals)
-- ============================================

-- Breakfast Menus
INSERT INTO "Menus" ("Id", "Name", "Price", "ItemId", "Category", "Restrictions") VALUES
('22222222-2222-2222-2222-222222222201', 'Classic Breakfast Combo', 12.99, 
 '["11111111-1111-1111-1111-111111111101", "11111111-1111-1111-1111-111111111103", "11111111-1111-1111-1111-111111111501"]'::jsonb,
 'Traditional', 'None'),

('22222222-2222-2222-2222-222222222202', 'Healthy Morning Menu', 14.99,
 '["11111111-1111-1111-1111-111111111104", "11111111-1111-1111-1111-111111111105", "11111111-1111-1111-1111-111111111502"]'::jsonb,
 'Vegetarian', 'GlutenFree'),

('22222222-2222-2222-2222-222222222203', 'Pancake Breakfast', 11.99,
 '["11111111-1111-1111-1111-111111111102", "11111111-1111-1111-1111-111111111501", "11111111-1111-1111-1111-111111111502"]'::jsonb,
 'Dessert', 'None'),

-- Lunch Menus
('22222222-2222-2222-2222-222222222301', 'Burger Meal Deal', 16.99,
 '["11111111-1111-1111-1111-111111111202", "11111111-1111-1111-1111-111111111301", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Meat', 'None'),

('22222222-2222-2222-2222-222222222302', 'Pizza Combo', 15.99,
 '["11111111-1111-1111-1111-111111111203", "11111111-1111-1111-1111-111111111204", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Mediterranean', 'None'),

('22222222-2222-2222-2222-222222222303', 'Healthy Lunch Box', 18.99,
 '["11111111-1111-1111-1111-111111111201", "11111111-1111-1111-1111-111111111305", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Meat', 'GlutenFree'),

('22222222-2222-2222-2222-222222222304', 'Mediterranean Delight', 22.99,
 '["11111111-1111-1111-1111-111111111206", "11111111-1111-1111-1111-111111111305", "11111111-1111-1111-1111-111111111304", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Seafood', 'None'),

('22222222-2222-2222-2222-222222222305', 'Vegetarian Feast', 14.99,
 '["11111111-1111-1111-1111-111111111207", "11111111-1111-1111-1111-111111111305", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Vegetarian', 'GlutenFree'),

('22222222-2222-2222-2222-222222222306', 'Mediterranean Wrap Meal', 13.99,
 '["11111111-1111-1111-1111-111111111208", "11111111-1111-1111-1111-111111111302", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 'Vegan', 'None'),

-- Dinner Menus
('22222222-2222-2222-2222-222222222401', 'Italian Night', 19.99,
 '["11111111-1111-1111-1111-111111111205", "11111111-1111-1111-1111-111111111304", "11111111-1111-1111-1111-111111111204", "11111111-1111-1111-1111-111111111402"]'::jsonb,
 'Mediterranean', 'None'),

('22222222-2222-2222-2222-222222222402', 'Steak House Special', 24.99,
 '["11111111-1111-1111-1111-111111111201", "11111111-1111-1111-1111-111111111302", "11111111-1111-1111-1111-111111111305", "11111111-1111-1111-1111-111111111401"]'::jsonb,
 'Meat', 'GlutenFree'),

-- Dessert Menus
('22222222-2222-2222-2222-222222222501', 'Sweet Tooth Menu', 13.99,
 '["11111111-1111-1111-1111-111111111401", "11111111-1111-1111-1111-111111111404", "11111111-1111-1111-1111-111111111503"]'::jsonb,
 'Dessert', 'None'),

('22222222-2222-2222-2222-222222222502', 'Healthy Dessert Option', 11.99,
 '["11111111-1111-1111-1111-111111111405", "11111111-1111-1111-1111-111111111104", "11111111-1111-1111-1111-111111111505"]'::jsonb,
 'Vegetarian', 'GlutenFree, LactoseFree');

-- ============================================
-- 3. INSERT ORDERS
-- ============================================

-- Generate some Client IDs for testing
-- Client 1: Regular customer
-- Client 2: Frequent customer
-- Client 3: Vegetarian customer

-- Orders from yesterday (Completed)
INSERT INTO "Orders" ("Id", "ClientId", "Price", "MenuIDs", "ItemIDs", "CreatedAt", "Status") VALUES
('33333333-3333-3333-3333-333333333301', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 16.99, 
 '["22222222-2222-2222-2222-222222222301"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '1 day', 'Completed'),

('33333333-3333-3333-3333-333333333302', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 15.99, 
 '["22222222-2222-2222-2222-222222222302"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '1 day', 'Completed'),

('33333333-3333-3333-3333-333333333303', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 23.98,
 '[]'::jsonb, '["11111111-1111-1111-1111-111111111207", "11111111-1111-1111-1111-111111111208", "11111111-1111-1111-1111-111111111305", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 NOW() - INTERVAL '1 day', 'Completed');

-- Orders from today (Various statuses for Kitchen feature testing)
-- Pending orders
INSERT INTO "Orders" ("Id", "ClientId", "Price", "MenuIDs", "ItemIDs", "CreatedAt", "Status") VALUES
('33333333-3333-3333-3333-333333333311', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 12.99, 
 '["22222222-2222-2222-2222-222222222201"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '30 minutes', 'Pending'),

('33333333-3333-3333-3333-333333333312', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 18.99, 
 '["22222222-2222-2222-2222-222222222303"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '25 minutes', 'Pending'),

('33333333-3333-3333-3333-333333333313', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 21.97,
 '[]'::jsonb, '["11111111-1111-1111-1111-111111111202", "11111111-1111-1111-1111-111111111301", "11111111-1111-1111-1111-111111111401", "11111111-1111-1111-1111-111111111504"]'::jsonb,
 NOW() - INTERVAL '20 minutes', 'Pending'),

-- Confirmed orders (ready to prepare)
('33333333-3333-3333-3333-333333333321', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 14.99, 
 '["22222222-2222-2222-2222-222222222305"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '15 minutes', 'Confirmed'),

('33333333-3333-3333-3333-333333333322', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 22.99, 
 '["22222222-2222-2222-2222-222222222304"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '12 minutes', 'Confirmed'),

-- Preparing orders (in kitchen)
('33333333-3333-3333-3333-333333333331', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 19.99, 
 '["22222222-2222-2222-2222-222222222401"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '10 minutes', 'Preparing'),

('33333333-3333-3333-3333-333333333332', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 24.99, 
 '["22222222-2222-2222-2222-222222222402"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '8 minutes', 'Preparing'),

('33333333-3333-3333-3333-333333333333', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 31.96,
 '["22222222-2222-2222-2222-222222222201"]'::jsonb, '["11111111-1111-1111-1111-111111111206", "11111111-1111-1111-1111-111111111305"]'::jsonb,
 NOW() - INTERVAL '5 minutes', 'Preparing'),

-- Recently completed orders (from today)
('33333333-3333-3333-3333-333333333341', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 13.99, 
 '["22222222-2222-2222-2222-222222222306"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '45 minutes', 'Completed'),

('33333333-3333-3333-3333-333333333342', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 11.99, 
 '["22222222-2222-2222-2222-222222222203"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '50 minutes', 'Completed'),

-- One cancelled order
('33333333-3333-3333-3333-333333333351', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 15.99, 
 '["22222222-2222-2222-2222-222222222302"]'::jsonb, '[]'::jsonb,
 NOW() - INTERVAL '1 hour', 'Cancelled');

-- ============================================
-- VERIFICATION QUERIES
-- ============================================

-- Check MenuItems count
SELECT 'MenuItems' as table_name, COUNT(*) as count FROM "MenuItems"
UNION ALL
-- Check Menus count
SELECT 'Menus' as table_name, COUNT(*) as count FROM "Menus"
UNION ALL
-- Check Orders count
SELECT 'Orders' as table_name, COUNT(*) as count FROM "Orders";

-- Show order status distribution
SELECT "Status", COUNT(*) as count 
FROM "Orders" 
GROUP BY "Status" 
ORDER BY "Status";

-- Show today's active orders (for Kitchen view testing)
SELECT 
    "Id",
    "ClientId",
    "Price",
    "Status",
    "CreatedAt"
FROM "Orders"
WHERE "CreatedAt" >= CURRENT_DATE
  AND "Status" IN ('Pending', 'Confirmed', 'Preparing')
ORDER BY "CreatedAt" ASC;
