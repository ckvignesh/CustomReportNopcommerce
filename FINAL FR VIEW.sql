


ALTER view [dbo].[Finance_Report] as


SELECT
  O.CustomOrderNumber AS OrderNumber,
  CONVERT(datetime2, O.CreatedOnUtc, 103) AS OrderDate,
  CONCAT(GA.Value, ' ', LN.Value) AS Customer,
  CASE O.OrderStatusId
    WHEN 10 THEN 'Pending'
    WHEN 20 THEN 'Processing'
    WHEN 30 THEN 'Complete'
    WHEN 40 THEN 'Cancelled'
    ELSE 'Unknown'
  END AS OrderStatus,
  CASE O.PaymentStatusId
    WHEN 10 THEN 'Pending'
    WHEN 20 THEN 'Authorized'
    WHEN 30 THEN 'Paid'
    WHEN 35 THEN 'Partially Refunded'
    WHEN 40 THEN 'Refunded'
    WHEN 50 THEN 'Voided'
    ELSE 'Unknown'
  END AS PaymentStatus,
  FORMAT(ROUND(O.OrderTotal, 2), 'N2') AS OrderTotal,
  FORMAT(ROUND(O.OrderSubTotalDiscountExclTax, 2), 'N2') AS OrderDiscount,
  O.ShippingAddressId,
  FORMAT(ROUND(O.OrderShippingExclTax, 2), 'N2') AS OrderShippingExclTax,
  FORMAT(ROUND(O.OrderTax, 2), 'N2') AS OrderTax,
  FORMAT(ROUND(O.OrderSubtotalExclTax, 2), 'N2') AS OrderSubtotalExclTax,
  SUBSTRING(O.TaxRates, 1, CHARINDEX(':', O.TaxRates) - 1) AS TaxRates,
  FORMAT(
    ROUND(O.OrderSubtotalExclTax - O.OrderSubTotalDiscountExclTax, 2),
    'N2'
  ) AS OrderSubtotalInclTax,
  O.RefundedAmount AS RefundedAmount,
  ETL.PaymentMode AS PaymentMode,
  C.[Name] AS CountryName,
  ABS(COALESCE(RPH.Points, 0)) AS RedeemedRewardPoints,
  RPH.UsedAmount AS RedeemedRewardPointsAmount,
  ONote.CreatedonUtc AS RefundedDate
FROM [Order] O
JOIN genericattribute GA ON O.CustomerId = GA.EntityId
JOIN genericattribute LN ON O.CustomerId = LN.EntityId
JOIN Address A ON O.ShippingAddressId = A.Id
JOIN Country C ON A.countryId = C.Id
LEFT JOIN (
  SELECT *
  FROM [ENETSTransactionLog] t1
  WHERE Id = (
    SELECT MAX(Id)
    FROM [ENETSTransactionLog] t2
    WHERE t1.OrderId = t2.OrderId
  )
) ETL ON O.Id = ETL.OrderId
LEFT JOIN RewardPointsHistory RPH ON O.RedeemedRewardPointsEntryId = RPH.Id
LEFT JOIN (
  SELECT OrderId, MAX(CreatedonUtc) AS CreatedonUtc
  FROM OrderNote
  WHERE [Note] LIKE '%Order has been marked as partially refunded%'
    OR [Note] LIKE '%Order has been marked as refunded%'
  GROUP BY OrderId
) ONote ON O.Id = ONote.OrderId
WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName'
ORDER BY O.CreatedOnUtc DESC
OFFSET 0 ROWS;


----SELECT O.CustomOrderNumber AS OrderNumber, 
----CONVERT(datetime2, O.CreatedOnUtc, 103) AS OrderDate, 
----CONCAT(GA.Value, ' ', LN.Value) AS Customer, 
----CASE O.OrderStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Processing' WHEN 30 THEN 'Complete' WHEN 40 THEN 'Cancelled' ELSE 'Unknown' END AS OrderStatus, 
----CASE O.PaymentStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Authorized' WHEN 30 THEN 'Paid' WHEN 35 THEN 'Partially Refunded' WHEN 40 THEN 'Refunded' WHEN 50 THEN 'Voided' ELSE 'Unknown' END AS PaymentStatus, 
------O.OrderTotal,
----FORMAT(ROUND(O.OrderTotal, 2), 'N2') as OrderTotal,
------O.OrderDiscount,
----FORMAT(ROUND(O.OrderSubTotalDiscountExclTax, 2), 'N2') as OrderDiscount,
----O.ShippingAddressId,
------O.OrderShippingExclTax,
----FORMAT(ROUND(O.OrderShippingExclTax, 2), 'N2') as OrderShippingExclTax,
------O.OrderTax,
----FORMAT(ROUND(O.OrderTax, 2), 'N2') as OrderTax,
------O.OrderSubtotalExclTax,
----FORMAT(ROUND(O.OrderSubtotalExclTax, 2), 'N2') as OrderSubtotalExclTax,
----SUBSTRING(O.TaxRates, 1, CHARINDEX(':', O.TaxRates) - 1) AS TaxRates,
------O.OrderSubtotalInclTax,
------FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,
------ the below field is named as Amt after disc excl. GST
----FORMAT(ROUND((O.OrderSubtotalExclTax - O.OrderSubTotalDiscountExclTax), 2), 'N2') as OrderSubtotalInclTax,
----O.RefundedAmount as RefundedAmount,
----ETL.PaymentMode AS PaymentMode, 
----C.[Name] AS CountryName, 
----ABS(COALESCE(RPH.Points, 0)) AS RedeemedRewardPoints,
----RPH.UsedAmount as RedeemedRewardPointsAmount
----FROM [Order] O 
----JOIN genericattribute GA ON O.CustomerId = GA.EntityId 
----JOIN genericattribute LN ON O.CustomerId = LN.EntityId 
----JOIN Address A ON O.ShippingAddressId = A.Id 
----JOIN Country C ON A.countryId = C.Id 
------LEFT JOIN [ENETSTransactionLog] ETL ON O.Id = ETL.OrderId
----LEFT JOIN ( SELECT *
----FROM [ENETSTransactionLog] t1
----WHERE Id = (
----    SELECT MAX(Id)
----    FROM [ENETSTransactionLog] t2
----    WHERE t1.OrderId = t2.OrderId
----) ) ETL ON O.Id = ETL.OrderId
----LEFT JOIN RewardPointsHistory RPH ON O.RedeemedRewardPointsEntryId = RPH.Id 
----WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName'
----Order by O.CreatedOnUtc desc offset 0 rows

--select * from [ENETSTransactionLog]


-----------
----SELECT O.CustomOrderNumber AS OrderNumber, 
----CONVERT(datetime2, O.CreatedOnUtc, 103) AS OrderDate, 
----CONCAT(GA.Value, ' ', LN.Value) AS Customer, 
----CASE O.OrderStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Processing' WHEN 30 THEN 'Complete' WHEN 40 THEN 'Cancelled' ELSE 'Unknown' END AS OrderStatus, 
----CASE O.PaymentStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Authorized' WHEN 30 THEN 'Paid' WHEN 35 THEN 'Partially Refunded' WHEN 40 THEN 'Refunded' WHEN 50 THEN 'Voided' ELSE 'Unknown' END AS PaymentStatus, 
------O.OrderTotal,
----FORMAT(ROUND(O.OrderTotal, 2), 'N2') as OrderTotal,
------O.OrderDiscount,
----FORMAT(ROUND(O.OrderDiscount, 2), 'N2') as OrderDiscount,
----O.ShippingAddressId,
------O.OrderShippingExclTax,
----FORMAT(ROUND(O.OrderShippingExclTax, 2), 'N2') as OrderShippingExclTax,
------O.OrderTax,
----FORMAT(ROUND(O.OrderTax, 2), 'N2') as OrderTax,
------O.OrderSubtotalExclTax,
----FORMAT(ROUND(O.OrderSubtotalExclTax, 2), 'N2') as OrderSubtotalExclTax,
------O.OrderSubtotalInclTax,
------FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,
------ the below field is named as Amt after disc excl. GST
----FORMAT(ROUND((OrderSubtotalExclTax - OrderDiscount), 2), 'N2') as OrderSubtotalInclTax, 
----ETL.PaymentMode AS PaymentMode, 
----C.[Name] AS CountryName, 
----ABS(COALESCE(RPH.Points, 0)) AS RedeemedRewardPoints,
----RPH.UsedAmount as RedeemedRewardPointsAmount
----FROM [Order] O 
----JOIN genericattribute GA ON O.CustomerId = GA.EntityId 
----JOIN genericattribute LN ON O.CustomerId = LN.EntityId 
----JOIN Address A ON O.ShippingAddressId = A.Id 
----JOIN Country C ON A.countryId = C.Id 
----LEFT JOIN [ENETSTransactionLog] ETL ON O.Id = ETL.OrderId 
----LEFT JOIN RewardPointsHistory RPH ON O.RedeemedRewardPointsEntryId = RPH.Id WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName'
----Order by O.CreatedOnUtc desc offset 0 rows
--------
 



----SELECT O.CustomOrderNumber AS OrderNumber, 
----CONVERT(datetime2, O.CreatedOnUtc, 103) AS OrderDate, 
----CONCAT(GA.Value, ' ', LN.Value) AS Customer, 
----CASE O.OrderStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Processing' WHEN 30 THEN 'Complete' WHEN 40 THEN 'Cancelled' ELSE 'Unknown' END AS OrderStatus, 
----CASE O.PaymentStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Authorized' WHEN 30 THEN 'Paid' WHEN 35 THEN 'Partially Refunded' WHEN 40 THEN 'Refunded' WHEN 50 THEN 'Voided' ELSE 'Unknown' END AS PaymentStatus, 
------O.OrderTotal,
----FORMAT(ROUND(O.OrderTotal, 2), 'N2') as OrderTotal,
------O.OrderDiscount,
----FORMAT(ROUND(O.OrderDiscount, 2), 'N2') as OrderDiscount,
----O.ShippingAddressId,
------O.OrderShippingExclTax,
----FORMAT(ROUND(O.OrderShippingExclTax, 2), 'N2') as OrderShippingExclTax,
------O.OrderTax,
----FORMAT(ROUND(O.OrderTax, 2), 'N2') as OrderTax,
------O.OrderSubtotalExclTax,
----FORMAT(ROUND(O.OrderSubtotalExclTax, 2), 'N2') as OrderSubtotalExclTax,
------O.OrderSubtotalInclTax,
------FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,
----FORMAT(ROUND((OrderSubtotalExclTax - OrderDiscount), 2), 'N2') as OrderSubtotalInclTax,
----ETL.PaymentMode AS PaymentMode, 
----C.[Name] AS CountryName, 
----ABS(COALESCE(RPH.Points, 0)) AS RedeemedRewardPoints 
----FROM [Order] O 
----JOIN genericattribute GA ON O.CustomerId = GA.EntityId 
----JOIN genericattribute LN ON O.CustomerId = LN.EntityId 
----JOIN Address A ON O.ShippingAddressId = A.Id 
----JOIN Country C ON A.countryId = C.Id 
----LEFT JOIN [ENETSTransactionLog] ETL ON O.Id = ETL.OrderId 
----LEFT JOIN RewardPointsHistory RPH ON O.RedeemedRewardPointsEntryId = RPH.Id WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName';



-------------------------------------------------------------------------------------------------------------
------SELECT O.CustomOrderNumber AS OrderNumber,
------       CONVERT(varchar, O.CreatedOnUtc, 120) AS OrderDate,
------       CONCAT(GA.Value, ' ', LN.Value) AS Customer,
------       CASE O.OrderStatusId
------           WHEN 10 THEN 'Pending'
------           WHEN 20 THEN 'Processing'
------           WHEN 30 THEN 'Complete'
------           WHEN 40 THEN 'Cancelled'
------           ELSE 'Unknown'
------       END AS OrderStatus,
------       CASE O.PaymentStatusId
------           WHEN 10 THEN 'Pending'
------           WHEN 20 THEN 'Authorized'
------           WHEN 30 THEN 'Paid'
------           WHEN 35 THEN 'Partially Refunded'
------           WHEN 40 THEN 'Refunded'
------           WHEN 50 THEN 'Voided'
------           ELSE 'Unknown'
------       END AS PaymentStatus,
------       O.OrderTotal,
------       O.OrderDiscount,
------       O.ShippingAddressId,
------       O.OrderShippingExclTax,
------       O.OrderTax,
------       O.OrderSubtotalExclTax,
------       O.OrderSubtotalInclTax,
------       ETL.PaymentMode AS PaymentMode,
------       C.[Name] AS CountryName
------FROM [Order] O
------JOIN genericattribute GA ON O.CustomerId = GA.EntityId
------JOIN genericattribute LN ON O.CustomerId = LN.EntityId
------JOIN Address A ON O.ShippingAddressId = A.Id
------JOIN Country C ON A.countryId = C.Id
------LEFT JOIN [ENETSTransactionLog] ETL ON O.Id = ETL.OrderId
------WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName';
GO

