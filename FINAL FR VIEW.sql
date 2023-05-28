


ALTER view [dbo].[Finance_Report] as


SELECT O.CustomOrderNumber AS OrderNumber, 
CONVERT(datetime2, O.CreatedOnUtc, 103) AS OrderDate, 
CONCAT(GA.Value, ' ', LN.Value) AS Customer, 
CASE O.OrderStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Processing' WHEN 30 THEN 'Complete' WHEN 40 THEN 'Cancelled' ELSE 'Unknown' END AS OrderStatus, 
CASE O.PaymentStatusId WHEN 10 THEN 'Pending' WHEN 20 THEN 'Authorized' WHEN 30 THEN 'Paid' WHEN 35 THEN 'Partially Refunded' WHEN 40 THEN 'Refunded' WHEN 50 THEN 'Voided' ELSE 'Unknown' END AS PaymentStatus, 
--O.OrderTotal,
FORMAT(ROUND(O.OrderTotal, 2), 'N2') as OrderTotal,
--O.OrderDiscount,
FORMAT(ROUND(O.OrderDiscount, 2), 'N2') as OrderDiscount,
O.ShippingAddressId,
--O.OrderShippingExclTax,
FORMAT(ROUND(O.OrderShippingExclTax, 2), 'N2') as OrderShippingExclTax,
--O.OrderTax,
FORMAT(ROUND(O.OrderTax, 2), 'N2') as OrderTax,
--O.OrderSubtotalExclTax,
FORMAT(ROUND(O.OrderSubtotalExclTax, 2), 'N2') as OrderSubtotalExclTax,
--O.OrderSubtotalInclTax,
--FORMAT(ROUND(O.OrderSubtotalInclTax, 2), 'N2') as OrderSubtotalInclTax,
-- the below field is named as Amt after disc excl. GST
FORMAT(ROUND((OrderSubtotalExclTax - OrderDiscount), 2), 'N2') as OrderSubtotalInclTax, 
ETL.PaymentMode AS PaymentMode, 
C.[Name] AS CountryName, 
ABS(COALESCE(RPH.Points, 0)) AS RedeemedRewardPoints,
RPH.UsedAmount as RedeemedRewardPointsAmount
FROM [Order] O 
JOIN genericattribute GA ON O.CustomerId = GA.EntityId 
JOIN genericattribute LN ON O.CustomerId = LN.EntityId 
JOIN Address A ON O.ShippingAddressId = A.Id 
JOIN Country C ON A.countryId = C.Id 
LEFT JOIN [ENETSTransactionLog] ETL ON O.Id = ETL.OrderId 
LEFT JOIN RewardPointsHistory RPH ON O.RedeemedRewardPointsEntryId = RPH.Id WHERE GA.[Key] = 'FirstName' AND LN.[Key] = 'LastName';

 
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


