using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace Excel
{
    /// <summary>
    /// Задает настройки стиля генерируемой таблицы.
    /// </summary>
    public class StyleSheet
    {
        /// <summary>
        /// Цвет фона заголовка таблицы.
        /// </summary>
        public (byte r, byte g, byte b) HeaderBackgroundColor = (255, 255, 255);
        /// <summary>
        /// Цвет шрифта заголовка таблицы.
        /// </summary>
        public (byte r, byte g, byte b) HeaderFontColor = (0, 0, 0);
        /// <summary>
        /// Размер шрифта заголовка таблицы.
        /// </summary>
        public double FontSizeHeader = 20;
        /// <summary>
        /// Определяет шрифт заголовка таблицы жирным.
        /// </summary>
        public bool HeaderIsBold = true;
        /// <summary>
        /// Определяет нижнюю границу заголовка таблицы жирной.
        /// </summary>
        public bool HeaderBottomBorderIsBold = true;

        /// <summary>
        /// Цвет фона тела таблицы.
        /// </summary>
        public (byte r, byte g, byte b) BodyBackgroundColor = (255, 255, 255);
        /// <summary>
        /// Цвет шрифта тела таблицы.
        /// </summary>
        public (byte r, byte g, byte b) BodyFontColor = (0, 0, 0);
        /// <summary>
        /// Размер шрифта тела таблицы.
        /// </summary>
        public double FontSizeBody = 15;

        /// <summary>
        /// Определяет внутренние границы границы таблицы.
        /// </summary>
        public bool InsideBorder = true;
        /// <summary>
        /// Определяет внешние границы границы таблицы.
        /// </summary>
        public bool OutsideBorder = true;

        /// <summary>
        /// Устанавливает стиль указанной области.
        /// </summary>
        /// <param name="style">Стиль</param>
        /// <param name="sheet">Рабочий лист</param>
        /// <param name="firsRow">Первая строка стилизируемой области</param>
        /// <param name="firstColumn">Первый столбец стилизируемой области</param>
        /// <param name="lastRow">Последняя строка стилизируемой области</param>
        /// <param name="lastColumn">Последний столбец стилизируемой области</param>
        internal static void SetStyleSheet(StyleSheet style, ref IXLWorksheet sheet, int firsRow, int firstColumn, int lastRow, int lastColumn)
        {
            //Стили для всей таблицы
            sheet.Range(firsRow, firstColumn, lastRow, lastColumn).Style.Border.InsideBorder =
                (style.InsideBorder) ? XLBorderStyleValues.Thin : XLBorderStyleValues.None;

            sheet.Range(firsRow, firstColumn, lastRow, lastColumn).Style.Border.OutsideBorder =
                (style.OutsideBorder) ? XLBorderStyleValues.Thin : XLBorderStyleValues.None;

            sheet.Range(firsRow, firstColumn, firsRow, lastColumn).Style.Border.BottomBorder =
                (style.HeaderBottomBorderIsBold) ? XLBorderStyleValues.Medium : XLBorderStyleValues.None;

            //Стили для заголовков таблицы
            sheet.Range(firsRow, firstColumn, firsRow, lastColumn).Style.Fill.BackgroundColor
                = XLColor.FromArgb(style.HeaderBackgroundColor.r, style.HeaderBackgroundColor.g, style.HeaderBackgroundColor.b);

            sheet.Range(firsRow, firstColumn, firsRow, lastColumn).Style.Font.FontColor
                = XLColor.FromArgb(style.HeaderFontColor.r, style.HeaderFontColor.g, style.HeaderFontColor.b);

            sheet.Row(firsRow).Style.Font.SetFontSize(style.FontSizeHeader);

            sheet.Row(firsRow).Style.Font.SetBold(style.HeaderIsBold);


            //Стили для тела таблицы
            sheet.Range(firsRow + 1, firstColumn, lastRow, lastColumn).Style.Fill.BackgroundColor
                = XLColor.FromArgb(style.BodyBackgroundColor.r, style.BodyBackgroundColor.g, style.BodyBackgroundColor.b);

            sheet.Range(firsRow + 1, firstColumn, lastRow, lastColumn).Style.Font.FontColor
                = XLColor.FromArgb(style.BodyFontColor.r, style.BodyFontColor.g, style.BodyFontColor.b);

            sheet.Range(firsRow + 1, firstColumn, lastRow, lastColumn).Style.Font.SetFontSize(style.FontSizeBody);
        }
    }


    /// <summary>
    /// Исключение генерируемое при не совпадении кол-ва заголовков и столюцов тела таблицы.
    /// </summary>
    public class HeaderNotEqualBodyException : Exception
    {
        public HeaderNotEqualBodyException() { }
        public override string Message => "Количество заголовкав не равно количеству столбцов тела таблицы!";
    }

    /// <summary>
    /// Исключение генерируемое при не соблюдении соглашения о формате таблицы файла Excel.
    /// </summary>
    public class InvalidSizeTableException : Exception
    {
        public InvalidSizeTableException() { }
        public override string Message => "Ячейка А1 не содержит количество строк данных таблицы или ячейка B1 " +
            "не содержит количество столбцов таблицы";
    }

    /// <summary>
    /// Реализует взаимодействие с файлом Excel.
    /// </summary>
    public static class ExcelFileInteraction
    {
        /// <summary>
        /// Номер строки, с которой начинается запись данных.
        /// </summary>
        private const int BODY_START = 3;

        /// <summary>
        /// Преобразует данные в поток для Excel файла.
        /// </summary>
        /// <param name="nameSheet">Имя листа</param>
        /// <param name="headers">Массив заголовков таблицы</param>
        /// <param name="body">Двумерный список данных для таблицых</param>
        /// <param name="style">Стиль генерируемой таблицы</param>
        /// <returns>Поток для Excel файла</returns>
        /// <exception cref="HeaderNotEqualBodyException">Если количество заголовков не совпадает с количеством столбцов данных таблицы.</exception>
        public static Stream ToExcel(string nameSheet, string[] headers, List<object[]> body, StyleSheet style)
        {
            if (headers.Length != body.First().Length) { throw new HeaderNotEqualBodyException(); }

            //Создаем рабочую область
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(nameSheet);

            //Настройка стилей
            StyleSheet.SetStyleSheet(style, ref sheet, BODY_START - 1, 1, body.Count + (BODY_START - 1), headers.Length);

            //Размеры телы таблицы
            sheet.Cell("A1").Value = $"Строк данных - {body.Count}";
            sheet.Cell("B1").Value = $"Столбцов данных - {headers.Length}";

            //Устанавливаем загаловки таблицы
            for (int i = 0; i < headers.Length; i++) 
            { 
                sheet.Cell(BODY_START - 1, i + 1).Value = headers[i]; 
            }

            //Начинаем записывать данные с 3 строки, т.к. в первой
            //кол-во строк с данными, во второй заголовки
            int countRow = BODY_START;
            foreach (var row in body)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    sheet.Cell(countRow, i + 1).Value = row[i];
                }
                countRow++;
            }
            sheet.Columns().AdjustToContents();


            //Возврат потока файла
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream;
            }
        }


        /// <summary>
        /// Преобразует данные из потока Excel файла в таблицу данных.
        /// </summary>
        /// <param name="file">Поток Excel файла. Ожидается, во второй строке расположены заголовки таблицы поэтому она пропускается.</param>
        /// <returns>Двумерный список данных из полученной таблицы файла</returns>
        /// <exception cref="InvalidSizeTableException">Если в ячейке А1 не указано количество строк данных или если в ячейке B1 не указано количество столбцов данных</exception>
        public static List<object[]> FromExcel(Stream file)
        {
            //Считываем поток файла, открываем первую страницу
            var workbook = new XLWorkbook(file);
            var sheet = workbook.Worksheet(1);

            uint countRow = 0, countCol = 0;
            try
            {
                countRow = (uint)Convert.ToInt32(sheet.Cell("A1").Value);
                countCol = (uint)Convert.ToInt32(sheet.Cell("B1").Value);
            }
            catch { throw new InvalidSizeTableException(); }

            //Считывание с 3 строки, т.к. во 2 заголовки таблицы, в 1 данные о размере
            var rows = sheet.RangeUsed().Rows(BODY_START, (int)countRow + (BODY_START - 1));
            List<object[]> data = new List<object[]>();
            foreach (var row in rows)
            {
                data.Add(new object[countCol]);
                for (int i = 0; i < data.Last().Length; i++)
                {
                    data.Last()[i] = row.Cell(i + 1).Value;
                }
            }
            return data;
        }
    }
}
