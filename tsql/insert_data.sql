use booksshop

insert into Autors
values
('������� ������', '2021-03-23'),
('������ ����', '2020-02-21'),
('����� ���������', '2021-12-21'),
('����� �������', '2011-12-12'),
('����� ��������', '2001-11-22')

insert into Books
values
('CLR via C#', default, '4th editioin', '�����'),
('Game design', 'How to make game that everyone will play', null, '�������'),
('������������ ����������', default, '5', '�����'),
('������������ �������', default, '4', '�����'),
('�# and .NET framework', 'From professionals for professionals', '8', 'apress')

insert into Books_Autors
values
(1, 1),
(2, 2),
(3, 3),
(4, 3),
(5, 4),
(5, 5)

go