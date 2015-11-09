drop Table TreeView

Create Table TreeView(
ID bigint primary key identity,
ParentID bigint foreign key references TreeView(ID),
BranchID nvarchar(50),
FactoryID int,
EmployeeID int,
CustomerID int,
Description nvarchar(100)
)
