create database QuanLyCafe

use QuanLyCafe

create table TableFood
(
	id int identity primary key,
	name nvarchar(100) default N'Chưa đặt tên',
	status nvarchar(100) default N'Trống'  -- có người / bàn trống
)

select * from TableFood

insert into TableFood (name, status) values(N'Bàn 1', N'Trống')

declare @i int = 1

while @i <= 10
begin
	insert into TableFood (name, status) values(N'Bàn ' + cast(@i as nvarchar(100)), N'Trống')
	set @i = @i + 1
end

select * from TableFood

create proc USP_GetTableList as select * from TableFood
exec USP_GetTableList

update TableFood set status = N'Có người' where name = N'Bàn 4'

declare @i int = 11

while @i <= 20
begin
	insert into TableFood (name, status) values(N'Bàn ' + cast(@i as nvarchar(100)), N'Trống')
	set @i = @i + 1
end

create table Account
(
	username varchar(100) primary key,
	display_name nvarchar(100) not null default N'Người dùng',
	password varchar(1000) not null default '00000000',
	type int not null default 0      --1: admin / 0: nhân viên
)

insert into Account (username, display_name, password, type) values('admin1', N'Hoàng Phan Khánh Huyền', 'admin123', 1)
insert into Account (username, display_name, password, type) values('staff1', N'Trần Ngọc Hương Giang', 'staff123', 0)

select * from Account

update Account set type = 1 where username = 'admin1'

create proc USP_GetAccountByUsername
@username varchar(100) as
begin
	select * from Account where username = @username
end

exec USP_GetAccountByUsername @username = 'admin1'

create proc USP_Login
@username varchar(100), @password varchar(100) as
begin
	select * from Account where username = @username and password = @password
end

exec USP_Login 'staff1', 'staff123'

create proc USP_UpdateAccount
@userName varchar(100), @displayName nvarchar(100), @password varchar(100), @newPass varchar(100) as
begin
	declare @isRightPass int = 0

	select @isRightPass = count(*) from Account where username = @userName and password = @password

	if (@isRightPass = 1)
	begin
		if (@newPass = null or @newPass = '') update Account set display_name = @displayName where username = @userName
		else update Account set display_name = @displayName, password = @newPass where username = @userName
	end
end

create table FoodCategory
(
	id int identity primary key,
	name nvarchar(100) default N'Chưa đặt tên'
)

insert into FoodCategory (name) values
(N'Cà phê'),
(N'Trà'),
(N'Sinh tố'),
(N'Nước ép'),
(N'Đồ ăn')

select * from FoodCategory

create table Food
(
	id int identity primary key,
	name nvarchar(100) not null default N'Chưa đặt tên',
	id_cate int not null references FoodCategory(id),
	price int not null default 0
)

insert into Food (name, id_cate, price) values
(N'Cà phê sữa đá', 1, 20000),
(N'Trà đào', 2, 25000),
(N'Hướng dương', 5, 25000)

insert into Food (name, id_cate, price) values
(N'Cà phê trứng', 1, 40000),
(N'Trà vải', 2, 25000),
(N'Nước ép dưa hấu', 4, 30000),
(N'Bạc xỉu', 1, 35000),
(N'Trà nhài', 2, 30000),
(N'Sinh tố bơ', 3, 45000),
(N'Nước ép cam', 4, 35000),
(N'Sinh tố xoài', 3, 38000),
(N'Chanh dây đá xay', 3, 35000);

select * from Food

create table Bill
(
	id int identity primary key,
	date_checkin date not null default getdate(),
	date_checkout date,
	id_table int not null references TableFood(id),
	status int not null default 0      --1: đã thanh toán, 0: chưa thanh toán
)
insert into Bill (date_checkin, date_checkout, id_table, status) values
(getdate(), null, 21, 0),
(getdate(), null, 15, 0),
(getdate(), getdate(), 21, 1),
(getdate(), null, 30, 0),
(getdate(), getdate(), 24, 1),
(getdate(), null, 12, 0)

select id from Bill where id_table = 15 and status = 0
select * from Bill

update Bill set status = 1 where id = 8

alter proc USP_InsertBill
@idTable int as
begin
	insert into Bill (date_checkin, date_checkout, id_table, status, discount) values
	(getdate(), null, @idTable, 0, 0)
end

alter table Bill add discount int
alter table Bill add total int
update Bill set discount = 0

create table BillInfo
(
	id int identity primary key,
	id_bill int not null references Bill(id),
	id_food int not null references Food(id),
	count int not null default 0
)

INSERT INTO BillInfo(id_bill, id_food, count) VALUES
(37, 3, 2),
(37, 7, 1),
(9, 1, 3),
(9, 5, 2),
(9, 9, 4),
(10, 2, 2),
(10, 6, 3),
(11, 4, 1),
(11, 8, 2),
(11, 12, 3),
(12, 3, 5),
(12, 10, 2),
(12, 1, 1),
(13, 2, 4),
(13, 7, 3),
(13, 5, 2),
(13, 11, 1)

select * from BillInfo

select * from TableFood

alter proc USP_InsertBillInfo
@idBill int, @idFood int, @count int as
begin
	declare @isExistBillInfo int
	declare @food_count int = 1

	select @isExistBillInfo = id, @food_count = bi.count
	from BillInfo as bi
	where id_bill = @idBill and id_food = @idFood

	if (@isExistBillInfo > 0)
	begin
		declare @new_count int = @food_count + @count
		if (@new_count > 0) update BillInfo set count = @food_count + @count where id_food = @idFood
		else delete BillInfo where id_bill = @idBill and id_food = @idFood
	end
	else INSERT INTO BillInfo(id_bill, id_food, count) VALUES(@idBill, @idFood,@count)
end


/*alter trigger UTG_UpdateBillInfo on BillInfo for insert, update as
begin
	declare @idBill int
	select @idBill = id_bill from inserted

	declare @idTable int
	select @idTable = id_table from Bill where id = @idBill and status = 0
	
	update TableFood set status = N'Có người' where id = @idTable
end*/

ALTER TRIGGER UTG_UpdateBillInfo 
ON BillInfo 
FOR INSERT, UPDATE 
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TableFood 
    SET status = N'Có người' 
    WHERE id IN (
        SELECT b.id_table 
        FROM Bill b
        JOIN inserted i ON b.id = i.id_bill
        WHERE b.status = 0
    );
END;


/*alter trigger UTG_UpdateTable on TableFood for update as
begin
	declare @idTable int
	declare @status nvarchar(100)

	select @idTable = id, @status = inserted.status from inserted

	declare @idBill int
	select @idBill = id from Bill where id_table = @idTable and status = 0

	declare @countBillInfo int
	select @countBillInfo = count(*) from BillInfo where id_bill = @idBill

	if (@countBillInfo > 0 and @status <> N'Có người')
		update TableFood set status = N'Có người' where id = @idTable
	else if (@countBillInfo <= 0 and @status <> N'Trống')
		update TableFood set status = N'Trống' where id = @idTable
end*/

ALTER TRIGGER UTG_UpdateTable 
ON TableFood 
FOR INSERT, UPDATE 
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TableFood 
    SET status = 
        CASE 
            WHEN (SELECT COUNT(*) 
                  FROM BillInfo bi
                  JOIN Bill b ON bi.id_bill = b.id
                  WHERE b.id_table = TableFood.id AND b.status = 0) > 0
            THEN N'Có người'
            ELSE N'Trống'
        END
    WHERE id IN (SELECT id FROM inserted);
END;

/*alter trigger UTG_UpdateBill on Bill for update as
begin
	declare @idBill int
	select @idBill = id from inserted

	declare @idTable int
	select @idTable = id_table from Bill where id = @idBill

	declare @count int = 0
	select count(*) from Bill where id_table = @idTable and status = 0

	if (@count = 0) update TableFood set status = N'Trống' where id = @idTable
end*/

ALTER TRIGGER UTG_UpdateBill 
ON Bill 
FOR UPDATE 
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TableFood 
    SET status = 
        CASE 
            WHEN (SELECT COUNT(*) FROM Bill WHERE id_table = TableFood.id AND status = 0) = 0 
            THEN N'Trống'
            ELSE status
        END
    WHERE id IN (SELECT id_table FROM inserted);
END;

/*alter proc USP_SwitchTable
@idTable1 int, @idTable2 int as
begin
	declare @id1Bill int
	declare @id2Bill int

	SELECT @id2Bill = id FROM Bill WHERE id_table = @idTable2 AND status = 0
	SELECT @id1Bill = id FROM Bill WHERE id_table = @idTable1 AND status = 0

	if (@id1Bill is null)
	begin
		insert into Bill(date_checkin, date_checkout, id_table, status) values
		(getdate(), null, @idTable1, 0)

		select @id1Bill = max(id) from Bill WHERE id_table = @idTable1 AND status = 0
	end
	
	if (@id2Bill is null)
	begin
		insert into Bill(date_checkin, date_checkout, id_table, status) values
		(getdate(), null, @idTable2, 0)

		select @id2Bill = max(id) from Bill WHERE id_table = @idTable2 AND status = 0
	end

	select id into IDBillInfoTable from BillInfo where id_bill = @id2Bill

	update BillInfo set id_bill = @id2Bill where id_bill = @id1Bill
	update BillInfo set id_bill = @id1Bill where id in (select * from IDBillInfoTable)

	drop table IDBillInfoTable
end*/

alter proc USP_SwitchTable
@idTable1 int, @idTable2 int as
begin
	declare @id1Bill int
	declare @id2Bill int

	SELECT @id2Bill = id FROM Bill WHERE id_table = @idTable2 AND status = 0
	SELECT @id1Bill = id FROM Bill WHERE id_table = @idTable1 AND status = 0

	if (@id1Bill is null)
	begin
		insert into Bill(date_checkin, date_checkout, id_table, status) values
		(getdate(), null, @idTable1, 0)

		select @id1Bill = max(id) from Bill WHERE id_table = @idTable1 AND status = 0
	end
	
	if (@id2Bill is null)
	begin
		insert into Bill(date_checkin, date_checkout, id_table, status) values
		(getdate(), null, @idTable2, 0)

		select @id2Bill = max(id) from Bill WHERE id_table = @idTable2 AND status = 0
	end

	-- Lưu danh sách BillInfo của bàn @id2Bill trước khi cập nhật
	select id into IDBillInfoTable from BillInfo where id_bill = @id2Bill

	-- Chuyển tất cả các BillInfo từ @id1Bill sang @id2Bill
	update BillInfo set id_bill = @id2Bill where id_bill = @id1Bill
	update BillInfo set id_bill = @id1Bill where id in (select * from IDBillInfoTable)

	-- Kiểm tra xem @id1Bill có còn món nào không
	if not exists (select 1 from BillInfo where id_bill = @id1Bill)
	begin
		-- Nếu không còn món nào, cập nhật trạng thái bàn @idTable1 thành 'Trống'
		update TableFood set status = N'Trống' where id = @idTable1
	end

	-- Xóa bảng tạm
	drop table IDBillInfoTable
end

select * from Bill
alter proc USP_GetListBillByDate
@checkinDate date, @checkoutDate date as
begin
	select t.name as [Tên bàn], b.total as [Tổng tiền], date_checkin as [Ngày vào], date_checkout as [Ngày ra], discount as [Giảm giá]
	from Bill as b, TableFood as t
	where date_checkin >= @checkinDate and date_checkout <= @checkoutDate and b.status = 1 and t.id = b.id_table
end

select f.id, f.name, c.name, f.price from Food as f, FoodCategory as c where f.id_cate = c.id

ALTER TRIGGER UTG_DeleteBillInfo 
ON BillInfo 
FOR DELETE 
AS
BEGIN
    DECLARE @idBill INT
    SELECT @idBill = id_bill FROM deleted

    DECLARE @idTable INT
    SELECT @idTable = id_table FROM Bill WHERE id = @idBill

    -- Đếm số lượng món còn lại trong hóa đơn sau khi xóa
    DECLARE @count INT
    SELECT @count = COUNT(*) FROM BillInfo WHERE id_bill = @idBill

    -- Nếu không còn món nào trong hóa đơn, cập nhật trạng thái bàn thành 'Trống'
    IF (@count = 0)
    BEGIN
        UPDATE TableFood SET status = N'Trống' WHERE id = @idTable
    END
END

SELECT * FROM Food WHERE name LIKE N'%Bạc%';