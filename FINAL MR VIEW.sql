



ALTER view [dbo].[Merchandise_Report] as


---------------------------------------------- Products WITHOUT ANY DISCOUNT -----------------------------------------------------

select 
	DISTINCT P.Id AS 'ProductID',
    P.[Name] AS 'ProductName',
    P.Sku AS 'SKU',
    P.StockQuantity AS 'StockQuantity',
    P.Price AS 'RetailPrice',
    P.OldPrice AS 'OldPrice',
    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
    P.ProductCost AS 'ProductCost',
    CASE P.ManageInventoryMethodId
        WHEN 0 THEN 'Dont Manage Stock'
        WHEN 1 THEN 'Manage Stock'
        WHEN 2 THEN 'Manage Stock By Attributes'
        ELSE 'Unknown'
    END AS 'InventoryMethod',
    STUFF((
        SELECT ', ' + C2.[Name]
        FROM Product_Category_Mapping PCM2
        JOIN Category C2 ON PCM2.CategoryId = C2.Id
        WHERE PCM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
    STUFF((
        SELECT ', ' + M2.[Name]
        FROM Product_Manufacturer_Mapping PMM2
        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
        WHERE PMM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
    STUFF((
        SELECT ', ' + V2.[Name]
        FROM Vendor V2
        WHERE P.VendorId = V2.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
    CASE P.Published
        WHEN 1 THEN 'Published'
        WHEN 0 THEN 'Unpublished'
        ELSE 'Unknown'
    END AS 'ProductStatus',
'NO ACTIVE DISCOUNT' AS 'DiscountName',
NULL AS 'DiscountAmount',
NULL AS 'PromotionStartDate',
NULL AS 'PromotionEndDate'
FROM Product P
where deleted = 0 and hasdiscountsApplied = 0
---- 579 rows

-------------------------------------------------------------------------------------------------------------------------------------------------
UNION
---------------------------------- Products WITH ACTIVE DISCOUNTS ----------------------------------------------------

select 
	DISTINCT P.Id AS 'ProductID',
    P.[Name] AS 'ProductName',
    P.Sku AS 'SKU',
    P.StockQuantity AS 'StockQuantity',
    P.Price AS 'RetailPrice',
    P.OldPrice AS 'OldPrice',
    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
    P.ProductCost AS 'ProductCost',
    CASE P.ManageInventoryMethodId
        WHEN 0 THEN 'Dont Manage Stock'
        WHEN 1 THEN 'Manage Stock'
        WHEN 2 THEN 'Manage Stock By Attributes'
        ELSE 'Unknown'
    END AS 'InventoryMethod',
    STUFF((
        SELECT ', ' + C2.[Name]
        FROM Product_Category_Mapping PCM2
        JOIN Category C2 ON PCM2.CategoryId = C2.Id
        WHERE PCM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
    STUFF((
        SELECT ', ' + M2.[Name]
        FROM Product_Manufacturer_Mapping PMM2
        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
        WHERE PMM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
    STUFF((
        SELECT ', ' + V2.[Name]
        FROM Vendor V2
        WHERE P.VendorId = V2.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
    CASE P.Published
        WHEN 1 THEN 'Published'
        WHEN 0 THEN 'Unpublished'
        ELSE 'Unknown'
    END AS 'ProductStatus',
CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN D.[Name]
        ELSE 'NO ACTIVE DISCOUNT'
    END AS 'DiscountName',
	CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
			--THEN 0
			-- checked date condition and moving down
		THEN
		-- check if percent is not used
			CASE
				WHEN D.UsePercentage = 0 THEN FORMAT(ROUND((P.Price - D.DiscountAmount), 2) , 'N2')
				ELSE
					CASE
					-- check whether percentage of disc is within max discount amount
						WHEN (P.Price * D.DiscountPercentage / 100) >= D.MaximumDiscountAmount
						--FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,ETL.PaymentMode AS PaymentMode
						THEN FORMAT(ROUND((P.Price - D.MaximumDiscountAmount), 2), 'N2')
						ELSE FORMAT(ROUND((P.Price - (P.Price * D.DiscountPercentage / 100)), 2), 'N2')
					END
			END
        ELSE NULL
    END AS 'DiscountAmount',
    CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN CONVERT(varchar(23), D.StartDateUtc, 121)
        ELSE NULL
    END AS 'PromotionStartDate',
    CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN CONVERT(varchar(23), D.EndDateUtc, 121)
        ELSE NULL
    END AS 'PromotionEndDate'
FROM Product P
LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
where deleted = 0 and hasdiscountsApplied = 1
AND ((GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
     AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL))
------9 Rows

----Select * from Product where deleted = 0 and hasdiscountsApplied = 1
------ 16 rows
----Select * from Product where deleted = 0 and hasdiscountsApplied = 0
------ 579 rows

---------------------------------- Products WITH NON ACTIVE DISCOUNTS ----------------------------------------------------

UNION 

select 
	DISTINCT P.Id AS 'ProductID',
    P.[Name] AS 'ProductName',
    P.Sku AS 'SKU',
    P.StockQuantity AS 'StockQuantity',
    P.Price AS 'RetailPrice',
    P.OldPrice AS 'OldPrice',
    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
    P.ProductCost AS 'ProductCost',
    CASE P.ManageInventoryMethodId
        WHEN 0 THEN 'Dont Manage Stock'
        WHEN 1 THEN 'Manage Stock'
        WHEN 2 THEN 'Manage Stock By Attributes'
        ELSE 'Unknown'
    END AS 'InventoryMethod',
    STUFF((
        SELECT ', ' + C2.[Name]
        FROM Product_Category_Mapping PCM2
        JOIN Category C2 ON PCM2.CategoryId = C2.Id
        WHERE PCM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
    STUFF((
        SELECT ', ' + M2.[Name]
        FROM Product_Manufacturer_Mapping PMM2
        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
        WHERE PMM2.ProductId = P.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
    STUFF((
        SELECT ', ' + V2.[Name]
        FROM Vendor V2
        WHERE P.VendorId = V2.Id
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
    CASE P.Published
        WHEN 1 THEN 'Published'
        WHEN 0 THEN 'Unpublished'
        ELSE 'Unknown'
    END AS 'ProductStatus',
CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN D.[Name]
        ELSE 'NO ACTIVE DISCOUNT'
    END AS 'DiscountName',
	CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
			--THEN 0
			-- checked date condition and moving down
		THEN
		-- check if percent is not used
			CASE
				WHEN D.UsePercentage = 0 THEN FORMAT(ROUND((P.Price - D.DiscountAmount), 2) , 'N2')
				ELSE
					CASE
					-- check whether percentage of disc is within max discount amount
						WHEN (P.Price * D.DiscountPercentage / 100) >= D.MaximumDiscountAmount
						--FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,ETL.PaymentMode AS PaymentMode
						THEN FORMAT(ROUND((P.Price - D.MaximumDiscountAmount), 2), 'N2')
						ELSE FORMAT(ROUND((P.Price - (P.Price * D.DiscountPercentage / 100)), 2), 'N2')
					END
			END
        ELSE NULL
    END AS 'DiscountAmount',
    CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN CONVERT(varchar(23), D.StartDateUtc, 121)
        ELSE NULL
    END AS 'PromotionStartDate',
    CASE
        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
            AND DAP.Discount_Id IS NOT NULL
            THEN CONVERT(varchar(23), D.EndDateUtc, 121)
        ELSE NULL
    END AS 'PromotionEndDate'
FROM Product P
LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
where deleted = 0 and hasdiscountsApplied = 1 And P.Id 
Not in (
	select 
	DISTINCT P.Id AS 'ProductID'
	From Product P
	LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
	LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
	where deleted = 0 and hasdiscountsApplied = 1
	AND ((GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
     AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL))
)


---------------------------------------------------- Products WITHOUT ANY DISCOUNT -----------------------------------------------------

------select 
------	DISTINCT P.Id AS 'ProductID',
------    P.[Name] AS 'ProductName',
------    P.Sku AS 'SKU',
------    P.StockQuantity AS 'StockQuantity',
------    P.Price AS 'RetailPrice',
------    P.OldPrice AS 'OldPrice',
------    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
------    P.ProductCost AS 'ProductCost',
------    CASE P.ManageInventoryMethodId
------        WHEN 0 THEN 'Dont Manage Stock'
------        WHEN 1 THEN 'Manage Stock'
------        WHEN 2 THEN 'Manage Stock By Attributes'
------        ELSE 'Unknown'
------    END AS 'InventoryMethod',
------    STUFF((
------        SELECT ', ' + C2.[Name]
------        FROM Product_Category_Mapping PCM2
------        JOIN Category C2 ON PCM2.CategoryId = C2.Id
------        WHERE PCM2.ProductId = P.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
------    STUFF((
------        SELECT ', ' + M2.[Name]
------        FROM Product_Manufacturer_Mapping PMM2
------        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
------        WHERE PMM2.ProductId = P.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
------    STUFF((
------        SELECT ', ' + V2.[Name]
------        FROM Vendor V2
------        WHERE P.VendorId = V2.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
------    CASE P.Published
------        WHEN 1 THEN 'Published'
------        WHEN 0 THEN 'Unpublished'
------        ELSE 'Unknown'
------    END AS 'ProductStatus',
------NULL AS 'DiscountName',
------NULL AS 'DiscountAmount',
------NULL AS 'PromotionStartDate',
------NULL AS 'PromotionEndDate'
------FROM Product P
------where deleted = 0 and hasdiscountsApplied = 0
-------- 579 rows

-----------------------------------------------------------------------------------------------------------------------------------------------------
------UNION
---------------------------------------- Products WITH DISCOUNTS ----------------------------------------------------

------select 
------	DISTINCT P.Id AS 'ProductID',
------    P.[Name] AS 'ProductName',
------    P.Sku AS 'SKU',
------    P.StockQuantity AS 'StockQuantity',
------    P.Price AS 'RetailPrice',
------    P.OldPrice AS 'OldPrice',
------    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
------    P.ProductCost AS 'ProductCost',
------    CASE P.ManageInventoryMethodId
------        WHEN 0 THEN 'Dont Manage Stock'
------        WHEN 1 THEN 'Manage Stock'
------        WHEN 2 THEN 'Manage Stock By Attributes'
------        ELSE 'Unknown'
------    END AS 'InventoryMethod',
------    STUFF((
------        SELECT ', ' + C2.[Name]
------        FROM Product_Category_Mapping PCM2
------        JOIN Category C2 ON PCM2.CategoryId = C2.Id
------        WHERE PCM2.ProductId = P.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
------    STUFF((
------        SELECT ', ' + M2.[Name]
------        FROM Product_Manufacturer_Mapping PMM2
------        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
------        WHERE PMM2.ProductId = P.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
------    STUFF((
------        SELECT ', ' + V2.[Name]
------        FROM Vendor V2
------        WHERE P.VendorId = V2.Id
------        FOR XML PATH(''), TYPE
------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
------    CASE P.Published
------        WHEN 1 THEN 'Published'
------        WHEN 0 THEN 'Unpublished'
------        ELSE 'Unknown'
------    END AS 'ProductStatus',
------CASE
------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------            AND DAP.Discount_Id IS NOT NULL
------            THEN D.[Name]
------        ELSE 'EXPIRED DISCOUNT'
------    END AS 'DiscountName',
------	CASE
------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------            AND DAP.Discount_Id IS NOT NULL
------			--THEN 0
------			-- checked date condition and moving down
------		THEN
------		-- check if percent is not used
------			CASE
------				WHEN D.UsePercentage = 0 THEN FORMAT(ROUND((P.Price - D.DiscountAmount), 2) , 'N2')
------				ELSE
------					CASE
------					-- check whether percentage of disc is within max discount amount
------						WHEN (P.Price * D.DiscountPercentage / 100) >= D.MaximumDiscountAmount
------						--FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,ETL.PaymentMode AS PaymentMode
------						THEN FORMAT(ROUND((P.Price - D.MaximumDiscountAmount), 2), 'N2')
------						ELSE FORMAT(ROUND((P.Price - (P.Price * D.DiscountPercentage / 100)), 2), 'N2')
------					END
------			END
------        ELSE NULL
------    END AS 'DiscountAmount',
------    CASE
------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------            AND DAP.Discount_Id IS NOT NULL
------            THEN CONVERT(varchar(23), D.StartDateUtc, 121)
------        ELSE NULL
------    END AS 'PromotionStartDate',
------    CASE
------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------            AND DAP.Discount_Id IS NOT NULL
------            THEN CONVERT(varchar(23), D.EndDateUtc, 121)
------        ELSE NULL
------    END AS 'PromotionEndDate'
------FROM Product P
------LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
------LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
------where deleted = 0 and hasdiscountsApplied = 1

-------- 17 Rows


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------SELECT
------------    DISTINCT P.Id AS 'ProductID',
------------    P.[Name] AS 'ProductName',
------------    P.Sku AS 'SKU',
------------    P.StockQuantity AS 'StockQuantity',
------------    P.Price AS 'RetailPrice',
------------    P.OldPrice AS 'OldPrice',
------------    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
------------    P.ProductCost AS 'ProductCost',
------------    CASE P.ManageInventoryMethodId
------------        WHEN 0 THEN 'Dont Manage Stock'
------------        WHEN 1 THEN 'Manage Stock'
------------        WHEN 2 THEN 'Manage Stock By Attributes'
------------        ELSE 'Unknown'
------------    END AS 'InventoryMethod',
------------    STUFF((
------------        SELECT ', ' + C2.[Name]
------------        FROM Product_Category_Mapping PCM2
------------        JOIN Category C2 ON PCM2.CategoryId = C2.Id
------------        WHERE PCM2.ProductId = P.Id
------------        FOR XML PATH(''), TYPE
------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
------------    STUFF((
------------        SELECT ', ' + M2.[Name]
------------        FROM Product_Manufacturer_Mapping PMM2
------------        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
------------        WHERE PMM2.ProductId = P.Id
------------        FOR XML PATH(''), TYPE
------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
------------    STUFF((
------------        SELECT ', ' + V2.[Name]
------------        FROM Vendor V2
------------        WHERE P.VendorId = V2.Id
------------        FOR XML PATH(''), TYPE
------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
------------    --CASE
------------    --    WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------------    --        AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------------    --        AND DAP.Discount_Id IS NOT NULL
------------    --        THEN D.[Name]
------------    --    ELSE 'No Discount'
------------    --END AS 'DiscountName',
------------CASE
------------        WHEN D.DiscountAmount > 0 AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN 
------------            CASE
------------                WHEN P.Price - D.DiscountAmount < 0 THEN 0
------------                ELSE P.Price - D.DiscountAmount
------------            END
------------        WHEN D.DiscountAmount <= 0 AND D.DiscountPercentage IS NOT NULL AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN
------------            CASE
------------                WHEN P.Price - (P.Price * D.DiscountPercentage / 100) < 0 THEN 0
------------                ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
------------            END
------------        WHEN D.DiscountAmount <= 0 AND D.MaximumDiscountAmount IS NOT NULL AND D.DiscountPercentage IS NOT NULL AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN
------------            CASE
------------                WHEN D.MaximumDiscountAmount < (P.Price * D.DiscountPercentage / 100) THEN
------------                    CASE
------------                        WHEN P.Price - D.MaximumDiscountAmount < 0 THEN 0
------------                        ELSE P.Price - D.MaximumDiscountAmount
------------                    END
------------                ELSE
------------                    CASE
------------                        WHEN P.Price - (P.Price * D.DiscountPercentage / 100) < 0 THEN 0
------------                        ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
------------                    END
------------            END
------------        ELSE NULL
------------    END AS 'DiscountAmount',
------------    CASE
------------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------------            AND DAP.Discount_Id IS NOT NULL
------------            THEN CONVERT(varchar(23), D.StartDateUtc, 121)
------------        ELSE NULL
------------    END AS 'PromotionStartDate',
------------    CASE
------------        WHEN (GETDATE() >= D.StartDateUtc OR D.StartDateUtc IS NULL)
------------            AND (GETDATE() <= D.EndDateUtc OR D.EndDateUtc IS NULL)
------------            AND DAP.Discount_Id IS NOT NULL
------------            THEN CONVERT(varchar(23), D.EndDateUtc, 121)
------------        ELSE NULL
------------    END AS 'PromotionEndDate',
------------    CASE P.Published
------------        WHEN 1 THEN 'Published'
------------        WHEN 0 THEN 'Unpublished'
------------        ELSE 'Unknown'
------------    END AS 'ProductStatus'
------------FROM Product P
------------LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
------------LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
------------WHERE P.Deleted = 0

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

----------------SELECT
----------------    DISTINCT P.Id AS 'ProductID',
----------------    P.[Name] AS 'ProductName',
----------------    P.Sku AS 'SKU',
----------------    P.StockQuantity AS 'StockQuantity',
----------------    P.Price AS 'RetailPrice',
----------------    P.OldPrice AS 'OldPrice',
----------------    CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
----------------    P.ProductCost AS 'ProductCost',
----------------    CASE P.ManageInventoryMethodId
----------------        WHEN 0 THEN 'Dont Manage Stock'
----------------        WHEN 1 THEN 'Manage Stock'
----------------        WHEN 2 THEN 'Manage Stock By Attributes'
----------------        ELSE 'Unknown'
----------------    END AS 'InventoryMethod',
----------------    STUFF((
----------------        SELECT ', ' + C2.[Name]
----------------        FROM Product_Category_Mapping PCM2
----------------        JOIN Category C2 ON PCM2.CategoryId = C2.Id
----------------        WHERE PCM2.ProductId = P.Id
----------------        FOR XML PATH(''), TYPE
----------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
----------------    STUFF((
----------------        SELECT ', ' + M2.[Name]
----------------        FROM Product_Manufacturer_Mapping PMM2
----------------        JOIN Manufacturer M2 ON PMM2.ManufacturerId = M2.Id
----------------        WHERE PMM2.ProductId = P.Id
----------------        FOR XML PATH(''), TYPE
----------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Brands',
----------------    STUFF((
----------------        SELECT ', ' + V2.[Name]
----------------        FROM Vendor V2
----------------        WHERE P.VendorId = V2.Id
----------------        FOR XML PATH(''), TYPE
----------------    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'VendorName',
----------------    CASE P.Published
----------------        WHEN 1 THEN 'Published'
----------------        WHEN 0 THEN 'Unpublished'
----------------        ELSE 'Unknown'
----------------    END AS 'ProductStatus',
----------------    CASE
----------------        WHEN D.Name IS NOT NULL THEN D.Name
----------------        ELSE NULL
----------------    END AS 'DiscountName',
----------------    CASE
----------------        WHEN D.DiscountAmount > 0 AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN 
----------------            CASE
----------------                WHEN P.Price - D.DiscountAmount < 0 THEN 0
----------------                ELSE P.Price - D.DiscountAmount
----------------            END
----------------        WHEN D.DiscountAmount <= 0 AND D.DiscountPercentage IS NOT NULL AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN
----------------            CASE
----------------                WHEN P.Price - (P.Price * D.DiscountPercentage / 100) < 0 THEN 0
----------------                ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
----------------            END
----------------        WHEN D.DiscountAmount <= 0 AND D.MaximumDiscountAmount IS NOT NULL AND D.DiscountPercentage IS NOT NULL AND GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN
----------------            CASE
----------------                WHEN D.MaximumDiscountAmount < (P.Price * D.DiscountPercentage / 100) THEN
----------------                    CASE
----------------                        WHEN P.Price - D.MaximumDiscountAmount < 0 THEN 0
----------------                        ELSE P.Price - D.MaximumDiscountAmount
----------------                    END
----------------                ELSE
----------------                    CASE
----------------                        WHEN P.Price - (P.Price * D.DiscountPercentage / 100) < 0 THEN 0
----------------                        ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
----------------                    END
----------------            END
----------------        ELSE NULL
----------------    END AS 'DiscountAmount',
----------------    CASE
----------------        --WHEN GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN CONVERT(varchar(23), D.StartDateUtc, 121)
----------------	WHEN GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN CONVERT(varchar(10), D.StartDateUtc, 121) + ' ' + CONVERT(varchar(8), D.StartDateUtc, 108)
----------------        ELSE NULL
----------------    END AS 'PromotionStartDate',
----------------    CASE
----------------        --WHEN GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN CONVERT(varchar(23), D.EndDateUtc, 121)
----------------WHEN GETDATE() BETWEEN D.StartDateUtc AND D.EndDateUtc THEN CONVERT(varchar(10), D.EndDateUtc, 121) + ' ' + CONVERT(varchar(8), D.EndDateUtc, 108)
----------------        ELSE NULL
----------------    END AS 'PromotionEndDate'
----------------FROM Product P
----------------LEFT JOIN Discount_AppliedToProducts DAP ON P.Id = DAP.Product_Id
----------------LEFT JOIN Discount D ON DAP.Discount_Id = D.Id
----------------WHERE P.Deleted = 0



------------ categories in single row with comma
----------SELECT P.Id AS 'ProductID',
----------       P.[Name] AS 'ProductName',
----------       P.Sku AS SKU,
----------       P.StockQuantity AS 'StockQuantity',
----------       P.Price AS 'RetailPrice',
----------       P.OldPrice AS 'OldPrice',
----------       CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
----------       P.ProductCost AS 'ProductCost',
----------       CASE P.ManageInventoryMethodId
----------           WHEN 0 THEN 'DontManageStock'
----------           WHEN 1 THEN 'ManageStock'
----------           WHEN 2 THEN 'ManageStockByAttributes'
----------           ELSE 'Unknown'
----------       END AS 'InventoryMethod',
----------       COALESCE(D.[Name], 'No Discount') AS 'DiscountName',
----------       CASE
----------           WHEN D.DiscountAmount > 0 THEN P.Price - D.DiscountAmount
----------           WHEN D.DiscountAmount <= 0 AND D.DiscountPercentage IS NOT NULL THEN P.Price - (P.Price * D.DiscountPercentage / 100)
----------           WHEN D.DiscountAmount <= 0 AND D.MaximumDiscountAmount IS NOT NULL AND D.DiscountPercentage IS NOT NULL THEN
----------               CASE
----------                   WHEN D.MaximumDiscountAmount < (P.Price * D.DiscountPercentage / 100) THEN P.Price - D.MaximumDiscountAmount
----------                   ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
----------               END
----------           ELSE NULL -- Return blank when none of the discount conditions are met
----------       END AS 'DiscountAmount',
----------       CASE D.DiscountTypeId
----------           WHEN 1 THEN 'Assigned To Order Total'
----------           WHEN 2 THEN 'Assigned To Products'
----------           WHEN 5 THEN 'Assigned To Categories'
----------           WHEN 6 THEN 'Assigned To Manufacturers'
----------           WHEN 10 THEN 'Assigned To Shipping'
----------           WHEN 20 THEN 'Assigned To Order Sub Total'
----------           ELSE NULL
----------       END AS 'DiscountType',
----------       D.StartDateUtc AS 'PromotionStartDate',
----------       D.EndDateUtc AS 'PromotionEndDate',
----------       STUFF((
----------           SELECT ', ' + C2.[Name]
----------           FROM Product_Category_Mapping PCM2
----------           JOIN Category C2 ON PCM2.CategoryId = C2.Id
----------           WHERE PCM2.ProductId = P.Id
----------           FOR XML PATH(''), TYPE
----------       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 'Categories',
----------       M.[Name] AS 'Brands',
----------       V.[Name] AS 'VendorName',
----------       CASE P.Published
----------           WHEN 1 THEN 'Published'
----------           WHEN 0 THEN 'Unpublished'
----------           ELSE 'Unknown'
----------       END AS 'ProductStatus'
----------FROM Product P
----------LEFT JOIN Discount_AppliedToProducts DM ON P.Id = DM.Product_Id
----------LEFT JOIN Discount D ON DM.Discount_Id = D.Id
----------LEFT JOIN Product_Manufacturer_Mapping PMM ON P.Id = PMM.ProductId
----------LEFT JOIN Manufacturer M ON PMM.ManufacturerId = M.Id
----------LEFT JOIN Vendor V ON P.VendorId = V.Id
----------WHERE (D.StartDateUtc <= GETUTCDATE() OR D.StartDateUtc IS NULL)
----------  AND (D.EndDateUtc >= GETUTCDATE() OR D.EndDateUtc IS NULL)
----------  OR (D.StartDateUtc <= GETUTCDATE() AND D.EndDateUtc IS NULL)
----------  OR (D.EndDateUtc IS NULL AND D.StartDateUtc IS NULL);


---------- Catgories in separate line
--------SELECT P.Id AS 'ProductID',
--------       P.[Name] AS 'ProductName',
--------       P.Sku AS SKU,
--------       P.StockQuantity AS 'StockQuantity',
--------       P.Price AS 'RetailPrice',
--------       P.OldPrice AS 'OldPrice',
--------       CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
--------       P.ProductCost AS 'ProductCost',
--------       CASE P.ManageInventoryMethodId
--------           WHEN 0 THEN 'DontManageStock'
--------           WHEN 1 THEN 'ManageStock'
--------           WHEN 2 THEN 'ManageStockByAttributes'
--------           ELSE 'Unknown'
--------       END AS 'InventoryMethod',
--------       COALESCE(D.[Name], 'No Discount') AS 'DiscountName',
--------       CASE
--------           WHEN D.DiscountAmount > 0 THEN P.Price - D.DiscountAmount
--------           WHEN D.DiscountAmount <= 0 AND D.DiscountPercentage IS NOT NULL THEN P.Price - (P.Price * D.DiscountPercentage / 100)
--------           WHEN D.DiscountAmount <= 0 AND D.MaximumDiscountAmount IS NOT NULL AND D.DiscountPercentage IS NOT NULL THEN
--------               CASE
--------                   WHEN D.MaximumDiscountAmount < (P.Price * D.DiscountPercentage / 100) THEN P.Price - D.MaximumDiscountAmount
--------                   ELSE P.Price - (P.Price * D.DiscountPercentage / 100)
--------               END
--------           ELSE NULL -- Return blank when none of the discount conditions are met
--------       END AS 'DiscountAmount',
--------       CASE D.DiscountTypeId
--------           WHEN 1 THEN 'Assigned To Order Total'
--------           WHEN 2 THEN 'Assigned To Products'
--------           WHEN 5 THEN 'Assigned To Categories'
--------           WHEN 6 THEN 'Assigned To Manufacturers'
--------           WHEN 10 THEN 'Assigned To Shipping'
--------           WHEN 20 THEN 'Assigned To Order Sub Total'
--------           ELSE NULL
--------       END AS 'DiscountType',
--------       D.StartDateUtc AS 'PromotionStartDate',
--------       D.EndDateUtc AS 'PromotionEndDate',
--------       C.[Name] AS 'Categories',
--------       M.[Name] AS 'Brands',
--------       V.[Name] AS 'VendorName',
--------       CASE P.Published
--------           WHEN 1 THEN 'Published'
--------           WHEN 0 THEN 'Unpublished'
--------           ELSE 'Unknown'
--------       END AS 'ProductStatus'
--------FROM Product P
--------LEFT JOIN Discount_AppliedToProducts DM ON P.Id = DM.Product_Id
--------LEFT JOIN Discount D ON DM.Discount_Id = D.Id
--------LEFT JOIN Product_Category_Mapping PCM ON P.Id = PCM.ProductId
--------LEFT JOIN Category C ON PCM.CategoryId = C.Id
--------LEFT JOIN Product_Manufacturer_Mapping PMM ON P.Id = PMM.ProductId
--------LEFT JOIN Manufacturer M ON PMM.ManufacturerId = M.Id
--------LEFT JOIN Vendor V ON P.VendorId = V.Id
--------WHERE (D.StartDateUtc <= GETUTCDATE() OR D.StartDateUtc IS NULL)
--------  AND (D.EndDateUtc >= GETUTCDATE() OR D.EndDateUtc IS NULL)
--------  OR (D.StartDateUtc <= GETUTCDATE() AND D.EndDateUtc IS NULL)
--------OR (D.EndDateUtc IS NULL AND D.StartDateUtc IS NULL);

--------SELECT P.Id AS 'ProductID',
--------       P.[Name] AS 'ProductName',
--------       P.Sku AS SKU,
--------       P.StockQuantity AS 'StockQuantity',
--------       P.Price AS 'RetailPrice',
--------       P.OldPrice AS 'OldPrice',
--------       --CONVERT(varchar(23), P.MarkAsNewStartDateTimeUtc, 121) AS 'ProductLaunchDate',
--------		CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
--------       P.ProductCost AS 'ProductCost',
--------       CASE P.ManageInventoryMethodId
--------           WHEN 0 THEN 'DontManageStock'
--------           WHEN 1 THEN 'ManageStock'
--------           WHEN 2 THEN 'ManageStockByAttributes'
--------           ELSE 'Unknown'
--------       END AS 'InventoryMethod',
--------       COALESCE(D.[Name], 'No Discount') AS 'DiscountName',
--------       D.DiscountPercentage,
--------       D.DiscountAmount,
--------       CASE D.DiscountTypeId
--------           WHEN 1 THEN 'Assigned To Order Total'
--------           WHEN 2 THEN 'Assigned To Products'
--------           WHEN 5 THEN 'Assigned To Categories'
--------           WHEN 6 THEN 'Assigned To Manufacturers'
--------           WHEN 10 THEN 'Assigned To Shipping'
--------           WHEN 20 THEN 'Assigned To Order Sub Total'
--------           ELSE NULL
--------       END AS 'DiscountType',
--------       D.StartDateUtc AS 'PromotionStartDate',
--------       D.EndDateUtc AS 'PromotionEndDate',
--------       C.[Name] AS 'Categories',
--------       M.[Name] AS 'Brands',
--------       V.[Name] AS 'VendorName',
--------       CASE P.Published
--------           WHEN 1 THEN 'Published'
--------           WHEN 0 THEN 'Unpublished'
--------           ELSE 'Unknown'
--------       END AS 'ProductStatus'
--------FROM Product P
--------LEFT JOIN Discount_AppliedToProducts DM ON P.Id = DM.Product_Id
--------LEFT JOIN Discount D ON DM.Discount_Id = D.Id
--------LEFT JOIN Product_Category_Mapping PCM ON P.Id = PCM.ProductId
--------LEFT JOIN Category C ON PCM.CategoryId = C.Id
--------LEFT JOIN Product_Manufacturer_Mapping PMM ON P.Id = PMM.ProductId
--------LEFT JOIN Manufacturer M ON PMM.ManufacturerId = M.Id
--------LEFT JOIN Vendor V ON P.VendorId = V.Id
--------WHERE (D.StartDateUtc <= GETUTCDATE() OR D.StartDateUtc IS NULL)
--------  AND (D.EndDateUtc >= GETUTCDATE() OR D.EndDateUtc IS NULL)
--------  OR (D.StartDateUtc <= GETUTCDATE() AND D.EndDateUtc IS NULL);

--------SELECT P.Id AS 'ProductID',
--------       P.[Name] AS 'ProductName',
--------       P.Sku AS SKU,
--------       P.StockQuantity AS 'StockQuantity',
--------       P.Price AS 'RetailPrice',
--------       P.OldPrice AS 'OldPrice',
--------       CONVERT(varchar(23), P.MarkAsNewStartDateTimeUtc, 121) AS 'ProductLaunchDate',
--------       P.ProductCost AS 'ProductCost',
--------       CASE P.ManageInventoryMethodId
--------           WHEN 0 THEN 'DontManageStock'
--------           WHEN 1 THEN 'ManageStock'
--------           WHEN 2 THEN 'ManageStockByAttributes'
--------           ELSE 'Unknown'
--------       END AS 'InventoryMethod',
--------       COALESCE(D.[Name], 'No Discount') AS 'DiscountName',
--------       D.DiscountPercentage,
--------       D.DiscountAmount,
--------       CASE D.DiscountTypeId
--------           WHEN 1 THEN 'Assigned To Order Total'
--------           WHEN 2 THEN 'Assigned To Products'
--------           WHEN 5 THEN 'Assigned To Categories'
--------           WHEN 6 THEN 'Assigned To Manufacturers'
--------           WHEN 10 THEN 'Assigned To Shipping'
--------           WHEN 20 THEN 'Assigned To Order Sub Total'
--------           ELSE NULL
--------       END AS 'DiscountType',
--------       D.StartDateUtc AS 'PromotionStartDate',
--------       D.EndDateUtc AS 'PromotionEndDate',
--------       C.[Name] AS 'Categories',
--------       M.[Name] AS 'Brands',
--------       V.[Name] AS 'VendorName',
--------       CASE P.Published
--------           WHEN 1 THEN 'Published'
--------           WHEN 0 THEN 'Unpublished'
--------           ELSE 'Unknown'
--------       END AS 'ProductStatus'
--------FROM Product P
--------LEFT JOIN Discount_AppliedToProducts DM ON P.Id = DM.Product_Id
--------LEFT JOIN Discount D ON DM.Discount_Id = D.Id
--------LEFT JOIN Product_Category_Mapping PCM ON P.Id = PCM.ProductId
--------LEFT JOIN Category C ON PCM.CategoryId = C.Id
--------LEFT JOIN Product_Manufacturer_Mapping PMM ON P.Id = PMM.ProductId
--------LEFT JOIN Manufacturer M ON PMM.ManufacturerId = M.Id
--------LEFT JOIN Vendor V ON P.VendorId = V.Id;
------GO


GO

