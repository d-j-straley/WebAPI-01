USE [RobotShop]
GO

/****** Object:  StoredProcedure [dbo].[CartItemDeleteByUserID-ProductID]    Script Date: 8/16/2024 11:43:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
Copyright 2024 David James Straley

declare @UserID int
declare @ProductID int
select @UserID = 1
select @ProductID = 1
exec dbo.[CartItemDeleteByUserID-ProductID] @UserID, @ProductID
*/

/* 
This will decrement the number of ProductID items that there are in the
cart for this User, until none are left.
*/

CREATE OR ALTER     PROCEDURE [dbo].[CartItemDeleteByUserID-ProductID]
	@UserID as int,
	@ProductID as int
AS
BEGIN
	SET NOCOUNT ON;

	declare @CartID int
	declare @nCount int
	declare @CartItemID int

	select @CartID = CartID from Cart where UserID = @UserID
	select @nCount = count(*) from CartItem 
		where CartID = @CartID
		and ProductID = @ProductID
	if(@nCount > 0)
	Begin
		/* decrement the number of this ProductID items that are in the cart
		*/
		select Top 1 @CartItemID = CartItemID 
			from CartItem
			where CartID = @CartID 
			and ProductID = @ProductID 
			order by CartItemID ASC
		delete from CartItem where CartItemID = @CartItemID
	End
END
GO


