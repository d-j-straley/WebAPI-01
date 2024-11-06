USE [RobotShop]
GO

/****** Object:  StoredProcedure [dbo].[ProductsInCartSelectAllByUserID]    Script Date: 8/15/2024 2:40:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
Copyright 2024 David James Straley

declare @UserID int
select @UserID = 1
exec dbo.ProductsInCartSelectAllByUserID @UserID
*/

CREATE OR ALTER PROCEDURE [dbo].[ProductsInCartSelectAllByUserID]
	-- Add the parameters for the stored procedure here
	@UserID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @CartID int
	select @CartID = CartID from Cart where UserID = @UserID 

	/* create a recordset of Products (as per what the Client
	 * will want to receive_ with the related info of those Products
	 * which are stored in this User's cart.
	 */
	select p.ProductID, p.Description,
	p.ImageName, c.CategoryName, ci.Price, ci.Discount
	from CartItem ci, Product p, Category c
	where p.ProductID = ci.ProductID
	and c.CategoryID = p.CategoryID
	and ci.CartID = @CartID 
	order by ci.Price DESC

END
GO


