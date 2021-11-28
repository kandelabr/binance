USE [Binance]
GO

/****** Object:  StoredProcedure [dbo].[ProcessBatchTest2]    Script Date: 11/28/2021 3:46:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ProcessBatchTest2] @lastImport datetime
AS
BEGIN
	DECLARE @StartDate datetime;
	DECLARE @EndDate datetime;
	DECLARE @DIFF int;
	declare @coef decimal(10,5); 
	declare @buysymbols varchar(200);
	declare @sellsymbols varchar(500);

	set @StartDate = getdate();
	set @coef = 1.15000;
	
	SET NOCOUNT ON;


	--empty starting point table
	truncate table StartingPointTempTest;
	
	insert into StartingPointTempTest
	select P.ID, P.Amount, P.Symbol, P.InformationID,  0 as TotalDifference
	from PricesTest P
	where not exists (select * from [TransactionTest] t where t.Symbol = p.Symbol);

	--merge into starting point table (update old, add new)
	MERGE StartingPointTest AS TARGET
	USING StartingPointTempTest AS SOURCE 
	ON (TARGET.Symbol = SOURCE.Symbol) 
	--When records are matched, update the records if there is any change
	WHEN MATCHED 
	THEN UPDATE SET 
		TARGET.ID = CASE WHEN SOURCE.AMOUNT > TARGET.AMOUNT THEN SOURCE.ID ELSE TARGET.ID END,
		TARGET.Amount = CASE WHEN SOURCE.AMOUNT > TARGET.AMOUNT THEN SOURCE.Amount ELSE TARGET.Amount END,
		TARGET.InformationID = CASE WHEN SOURCE.AMOUNT > TARGET.AMOUNT THEN SOURCE.InformationID ELSE TARGET.InformationID END,
		TARGET.MinAmount = CASE WHEN SOURCE.AMOUNT < TARGET.AMOUNT THEN SOURCE.Amount ELSE TARGET.MinAmount END
	--When no records are matched, insert the incoming records from source table to target table
	WHEN NOT MATCHED BY TARGET 
	THEN INSERT (ID, Symbol, Amount, InformationID, TotalDifference, MinAmount) 
		VALUES (SOURCE.ID, SOURCE.Symbol, SOURCE.Amount, SOURCE.InformationID, SOURCE.TotalDifference, SOURCE.Amount);

	delete from  Sleep
	where dateadd(hour, 1, soldAt) < @lastImport;

	--we remove TempPrices once we know which coef to use 
	--remove data from temp
	truncate table TempPricesTest;

	--insert into temp
	insert into TempPricesTest
	select m.id, m.symbol, m.amount, @coef, m.amount / @coef, m.informationID, m.[TotalDifference], m.MinAmount
	from [dbo].StartingPointTest m;

	select t.ID as [ReferencePriceId], t.OriginalAmount as [ReferenceAmount], t.[ReferenceDifference],
		l.ID as [StartPriceId], l.Amount as [StartAmount], l.Amount as [LastAmount], 100 / l.amount as [MoneyIn], l.Symbol, t.Coefficient,  'Initialized' as CurrentStatus
	into #TransactionTemp
	from dbo.LatestPricesTest l 
	join TempPricesTest t on l.Symbol = t.Symbol
	where l.amount < t.CoefAmount 
	and (l.Amount - t.MinAmount)*100/t.MinAmount < 50;

	--INSERT FIRST TRANSACTIONS
	insert into 
	[dbo].[TransactionFirst]
		([ReferencePriceId], [ReferenceAmount], [ReferenceDifference],
		[StartPriceId], [StartAmount], [LastAmount], [MoneyIn], Symbol, Couficient, CurrentStatus)	
	select l.* from #TransactionTemp l
	where not exists 
	(
		select * from [TransactionFirst] tt
		where tt.Symbol = l.Symbol
	);

	--INSERT NEW TRANSACTIONS
	insert into 
	[dbo].[TransactionTest]
		([ReferencePriceId], [ReferenceAmount], [ReferenceDifference],
		[StartPriceId], [StartAmount], [LastAmount], [MoneyIn], Symbol, Couficient, CurrentStatus, StartDate)
	select l.*, @lastImport from #TransactionTemp l
	where not exists 
	(
		select * from [TransactionTest] tt
		where tt.Symbol = l.Symbol
		and tt.CurrentStatus = 'InProgress'
	)
	and not exists 
	(
		select * from Sleep s
		where s.Symbol = l.Symbol
	)
	and exists 
	(
		select * from [TransactionFirst] s
		where s.Symbol = l.Symbol
		and s.CurrentStatus = 'Sold'
	);

	--call buy svc
	select @buysymbols =  STRING_AGG(Symbol, ',') from [Transaction]
	where CurrentStatus = 'Initialized';

	EXEC [dbo].[BuyCrypto] @SYMBOLS = @buysymbols;

	truncate table #TransactionTemp;

	
	--UPDATE LAST TRANSCACTION
	update [dbo].[TransactionTest]
	set [LastAmount] = 
	(
		select l.amount 
		from dbo.LatestPricesTest l 
		where [TransactionTest].symbol = l.symbol
	);

	update [dbo].[TransactionFirst]
	set [LastAmount] = 
	(
		select l.amount 
		from dbo.LatestPricesTest l 
		where [TransactionFirst].symbol = l.symbol
		and [TransactionFirst].CurrentStatus = 'InProgress'
	);


	----END TRASACTION
	update [dbo].[TransactionFirst]
	set [SellPriceId] = l.ID, [SellAmount] = l.Amount, [MoneyOut] = [MoneyIn] * l.Amount, CurrentStatus = 'Finishing'
	from 
	(
		select ID, Amount, symbol
		from dbo.LatestPricesTest l 
	)l
	where [TransactionFirst].symbol = l.symbol
	and [TransactionFirst].StartAmount * 1.02 <= l.Amount-- * 100/100.05
	and [TransactionFirst].CurrentStatus = 'InProgress';

	update [dbo].[TransactionTest]
	set [SellPriceId] = l.ID, [SellAmount] = l.Amount, [MoneyOut] = [MoneyIn] * l.Amount, CurrentStatus = 'Sold', EndDate = @lastImport
	from 
	(
		select ID, Amount, symbol
		from dbo.LatestPricesTest l 
	)l
	where [TransactionTest].symbol = l.symbol
	and [TransactionTest].StartAmount * 1.02 <= l.Amount-- * 100/100.05
	and [TransactionTest].CurrentStatus = 'InProgress';

	select @sellsymbols = STRING_AGG(code, ',')
	from
	(
	select Concat(Symbol,'|',MoneyIn) as code, * from [Transaction]
	WHERE CurrentStatus = 'Sold'
	)a;

	EXEC [dbo].[SellCrypto] @SYMBOLS = @sellsymbols;

	insert into Sleep
	select symbol, @lastImport from [TransactionFirst]
	where CurrentStatus = 'Finishing';

	--ARCHIVE SOLD TRANSACTION
	insert into [TransactionArchiveTest]
	select * from [TransactionTest]
	where CurrentStatus = 'Sold';

	insert into Sleep
	select symbol, @lastImport from [TransactionTest]
	where CurrentStatus = 'Sold';

	--delete from prices and starting point sold symbol
	delete from StartingPointTest
	where symbol in 
	(
		select symbol 
		from [TransactionTest]
		where CurrentStatus = 'Sold'
    );

	delete from StartingPointTest
	where symbol in 
	(
		select symbol 
		from [TransactionFirst]
		where CurrentStatus = 'Finishing'
    );

	--REMOVE SOLD TRANSACTION
	delete from [TransactionTest]
	where CurrentStatus = 'Sold';

	--IN PROGRESS NEW TRANSACTIONS
	update [TransactionTest]
	set CurrentStatus = 'InProgress'
	where CurrentStatus = 'Initialized';

	update [TransactionFirst]
	set CurrentStatus = 'InProgress'
	where CurrentStatus = 'Initialized';

	update [TransactionFirst]
	set CurrentStatus = 'Sold'
	where CurrentStatus = 'Finishing';

	delete PricesTest
	
	set @EndDate = getdate();

	--log execution time
	insert into PricesLogTest
	select @StartDate, @EndDate;
	
END
GO


