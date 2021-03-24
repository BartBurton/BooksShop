# BooksShop
## ТЗ от "БПО" 

### 1)MSSQL
>Решение в виде 3 скриптов T-SQL в папке tsql/. Скрип создания таблиц содержит ошибку, в связующей таблице BooksAutors для many to many связи Books и Autors, указано NO ACTION при удалениии поля связной таблицы. Ошибка была исправлена изменением контекста БД в приложении используещим EF Core и добавлением миграции - CascadeDeleteForBooksAutors. 
 
### 2)Asp MVC 5 
>Решение находится в папке BooksShop/. Решение прокомментированно и использует для стилизации страниц библиотеку bootstrap. 
 
### 3)Working with Excel 
>Решение находится в BooksShop/BooksShop/Controllers/ExсelToFromDbController.cs и в BooksShop/BooksShop/Views/ExcelToFromDb/Excel.cshtml. В папке excel/ лежит файл для загрузки корректных данных в базу.
