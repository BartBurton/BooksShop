use booksshop

insert into Autors
values
('Джеффри Рихтер', '2021-03-23'),
('Джесси Шелл', '2020-02-21'),
('Эндрю Тоненбаум', '2021-12-21'),
('Филип Джепикс', '2011-12-12'),
('Эндрю Троелсен', '2001-11-22')

insert into Books
values
('CLR via C#', default, '4th editioin', 'Питер'),
('Game design', 'How to make game that everyone will play', null, 'Альпина'),
('Архитерктура компьютера', default, '5', 'Питер'),
('Операционные системы', default, '4', 'Питер'),
('С# and .NET framework', 'From professionals for professionals', '8', 'apress')

insert into Books_Autors
values
(1, 1),
(2, 2),
(3, 3),
(4, 3),
(5, 4),
(5, 5)

go