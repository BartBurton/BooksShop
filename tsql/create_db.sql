use master;

create database booksshop

on primary
(
	name = bs_dat,
	filename = 'D:\!!dataDB_MSSQLServer\booksshop\bs.mdf',
	size = 64,
	filegrowth = 8
)

log on
(
	name = bs_log,
	filename = 'D:\!!dataDB_MSSQLServer\booksshop\bs.ldf',
	size = 32,
	filegrowth = 8
)

go
