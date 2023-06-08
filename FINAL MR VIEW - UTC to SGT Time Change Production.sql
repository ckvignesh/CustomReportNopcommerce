--RUN ONLY IN SGH PROD. DO NOT RUN IN UAT OR ANY OTHER ENV WITHOUT VERIFYING
---------------------------------------------- Products WITHOUT ANY DISCOUNT -----------------------------------------------------
ALTER view [dbo].[Merchandise_Report] as

select 
	DISTINCT P.Id AS 'ProductID',
    P.[Name] AS 'ProductName',
    P.Sku AS 'SKU',
    P.StockQuantity AS 'StockQuantity',
    P.Price AS 'RetailPrice',
    P.OldPrice AS 'OldPrice',
-- line below is commented out to convert time form UTC to SGT as a work around for a Production bug. 
-- Change the same line in next 2 sections along with this. NOT needed  in UAT
    --CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
	CONVERT(varchar(23), DATEADD(hour, 8, P.AvailableStartDateTimeUtc), 121) AS 'ProductLaunchDate',
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
-- line below is commented out to convert time form UTC to SGT as a work around for a Production bug. 
-- Change the same line in next 2 sections along with this. NOT needed  in UAT
    --CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
	CONVERT(varchar(23), DATEADD(hour, 8, P.AvailableStartDateTimeUtc), 121) AS 'ProductLaunchDate',
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
-- line below is commented out to convert time form UTC to SGT as a work around for a Production bug. 
-- Change the same line in next 2 sections along with this. NOT needed  in UAT
    --CONVERT(varchar(23), P.AvailableStartDateTimeUtc, 121) AS 'ProductLaunchDate',
	CONVERT(varchar(23), DATEADD(hour, 8, P.AvailableStartDateTimeUtc), 121) AS 'ProductLaunchDate',
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
