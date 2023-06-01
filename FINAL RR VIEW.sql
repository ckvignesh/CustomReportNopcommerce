USE [singhealth_DB]
GO

/****** Object:  View [dbo].[Refund_Report]    Script Date: 6/1/2023 8:29:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[Refund_Report] as

select OrderNumber, 
OrderDate, 
Customer, 
PaymentStatus, 
PaymentMode,
FORMAT(ROUND(RefundedAmount, 2), 'N2') as 'RefundedAmount',
FORMAT(ROUND(RefundedAmount / (1.00000+(CONVERT (decimal(10,2), TaxRates)/100)), 2), 'N2') AS 'RefundedAmountExclTax',
CAST(ROUND(RefundedAmount, 2) AS decimal(10,2)) - FORMAT(ROUND(RefundedAmount / (1.00000+(CONVERT (decimal(10,2), TaxRates)/100)), 2), 'N2') as 'GST',
--CAST((CONVERT(decimal(10,2), TaxRates)/100) AS decimal(10,2)) * FORMAT(ROUND(RefundedAmount/(1.00000+(CONVERT (decimal(10,2), TaxRates)/100)), 2), 'N2') as 'GST',
TaxRates
from Finance_Report
where PaymentStatus like '%refund%' 
--and OrderNumber like 'SPC-0523-2175'
GO


