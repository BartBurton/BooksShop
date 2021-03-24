use booksshop

create table Books
(
	id bigint primary key identity(1, 1),
	title nvarchar(128) not null,
	description nvarchar(2048) default 'No description!',
	edition nvarchar(256),
	published_at nvarchar(256)
);

create table Autors
(
	id bigint primary key identity(1, 1),
	name nvarchar(256) not null,
	dob date,
);

create table Books_Autors
(
	id_book bigint,
	id_autor bigint,

	primary key(id_book, id_autor),

	constraint fk_books_to_autors
		foreign key(id_book) references Books(id) on delete no action,
		foreign key(id_autor) references Autors(id) on delete no action
);

go