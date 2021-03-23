use master;

create database booksshop

on primary
(
	name = bs_dat,
	filename = 'D:\bookshop\bs.mdf',
	size = 64,
	filegrowth = 8
)

log on
(
	name = bs_log,
	filename = 'D:\bookshop\bs.ldf',
	size = 32,
	filegrowth = 8
)

go