using Microsoft.Office.Interop.Excel;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;

namespace Kursovoi_Parsing
{
    class COMFormatter
    {
        Word.Application wordApp = new Word.Application();
        Word.Documents wordDocuments;
        Word.Document wordDocument;
        Word.Tables wordTables;
        Word.Table wordTable;
        public COMFormatter(string template)
        {
            wordApp.Visible = true;

            Object newTemplate = false;
            Object documentType = Word.WdNewDocumentType.wdNewBlankDocument;
            Object visible = true;

            wordApp.Documents.Add(template, newTemplate, ref documentType, ref visible);

            wordDocuments = wordApp.Documents;
            wordDocument = wordDocuments.get_Item(1);

            wordTables = wordDocument.Tables;
            wordTable = wordTables[1];

            wordDocument.Activate();
        }


        
        public void Replace(string wordr, string replacement)
        {
            Word.Range range = wordDocument.StoryRanges[Word.WdStoryType.wdMainTextStory];
            range.Find.ClearFormatting();

            range.Find.Execute(FindText: wordr, ReplaceWith: replacement);

            TrySave();
        }

        public void TrySave()
        {
            Random random = new Random();
            int rand = random.Next(0,10);
            //string stroka = rand + ".doc";
            try
            {
                wordDocument.SaveAs2($@"C:\Users\5525m\source\repos\Kursovoi_Parsing\Отчёт.doc", Word.WdSaveFormat.wdFormatDocument);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Close()
        {
            Object saveChanges = Word.WdSaveOptions.wdPromptToSaveChanges;
            Object originalFormat = Word.WdOriginalFormat.wdWordDocument;
            Object routeDocument = Type.Missing;
            wordApp.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
        }


        public void WordTableFill(int num,int bold)
        {
            Word.Range range = wordDocument.StoryRanges[Word.WdStoryType.wdMainTextStory];
            range.Find.ClearFormatting();
           
            using (ShopParserEntities db = new ShopParserEntities())
            {
                if (num == 2) 
                {
                    var mass = db.Смартфоны.Select(b => new { brand = b.Наименование, price = b.Цена }).OrderByDescending(b => b.price).Take(10).ToList();
                    int i = 2;
                    foreach (var tcount in mass)
                    {
                        wordTable.Rows.Add();
                        if (bold == 1) { wordTable.Range.Bold = 1; }
                        if (bold == 2) { wordTable.Range.Bold = 0; }
                        if (bold == 3) { wordTable.Range.Italic = 1; }
                        wordTable.Cell(i, 1).Range.Text = tcount.brand;
                        wordTable.Cell(i, 2).Range.Text = tcount.price.ToString();
                        i++;
                    }

                }
                if (num == 3)
                {
                    var mass = db.Планшеты.Select(b => new { brand = b.Наименование, price = b.Цена }).OrderByDescending(b => b.price).Take(10).ToList();
                    int i = 2;
                    foreach (var tcount in mass)
                    {
                        wordTable.Rows.Add();
                        if (bold == 1) { wordTable.Range.Bold = 1; }
                        if(bold == 2) { wordTable.Range.Bold = 0; }
                        if(bold == 3) { wordTable.Range.Italic = 1; }  
                        wordTable.Cell(i, 1).Range.Text = tcount.brand;
                        wordTable.Cell(i, 2).Range.Text = tcount.price.ToString();
                        i++;
                    }
                }
                
            }
            //вставка изображения
            Word.Range docRange = wordDocument.Paragraphs[38].Range;
            wordDocument.Paragraphs[38].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            string imageName = @"C:\Users\5525m\source\repos\Kursovoi_Parsing\Graf.bmp";
            InlineShape pictureShape = docRange.InlineShapes.AddPicture(imageName);


            TrySave();
        }

    }

    public partial class MainWindow : System.Windows.Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //Preparing();

        }
        //private void Preparing()
        //{
        //    dt = new DataTable();

        //    #region Init

        //    var connectionStringBuilder = new SqlConnectionStringBuilder
        //    {
        //        DataSource = @"MOKKO-ADMIN",
        //        InitialCatalog = "ShopParser"
        //    };

        //    con = new SqlConnection(connectionStringBuilder.ConnectionString);
        //    dt = new DataTable();
        //    da = new SqlDataAdapter();

        //    #endregion


        //    #region select


        //    var sql = @"SELECT * FROM Смартфоны";
        //    da.SelectCommand = new SqlCommand(sql, con);

        //    #endregion

        //    #region delete

        //    sql = "DELETE FROM Смартфоны WHERE Код_товара = @Код_товара";

        //    da.DeleteCommand = new SqlCommand(sql, con);
        //    da.DeleteCommand.Parameters.Add("@Код_товара", SqlDbType.Int, 4, "Код_товара");

        //    #endregion



        //    //da.Fill(dt);
        //    myGrid.DataContext = dt.DefaultView;
        //}

        //public ObservableCollection<Смартфоны> Смартфон = new ObservableCollection<Смартфоны>();
        private ShopParserEntities db = new ShopParserEntities();
                                                                                                  //_____________________________________________________________смартфоны
        private void myGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)//начало редактирования
        {
            SmartButtonSave.IsEnabled = true;
        }

        private void SmartSave(object sender, RoutedEventArgs e)// сохранение отредактированного
        {
            Смартфоны смартфоны = myGrid.SelectedItem as Смартфоны;
            int strokaKod = смартфоны.Код_товара;
            string strokaName = смартфоны.Наименование;
            string strokaBrend = смартфоны.Бренд;
            int strokaPrice = (int)смартфоны.Цена;
            //db.Смартфоны.Remove(смартфоны); // тогда айдишник меняется
            //using (ShopParserEntities db = new ShopParserEntities())// второй рабочий вариант
            //{
            //    Смартфоны смартфоны1 = new Смартфоны() { Код_товара = strokaKod, Наименование = strokaName, Бренд = strokaBrend, Цена = strokaPrice };
            //    db.Смартфоны.Add(смартфоны1);
            //    db.SaveChanges();
            //}
            if (myGrid.SelectedItem != null) // первый вариант
            {
                Смартфоны item = (Смартфоны)myGrid.SelectedItem;
                item.Код_товара = strokaKod;
                item.Наименование = strokaName;
                item.Бренд = strokaBrend;
                item.Цена = strokaPrice;
                myGrid.Items.Refresh();
            }
            db.SaveChanges();

        }

        public void MenuItemDeleteClick(object sender, RoutedEventArgs e)//удаление  
        {
            Смартфоны смартфоны = myGrid.SelectedItem as Смартфоны;
            
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить данную строку?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Смартфоны.Remove(смартфоны);
                    db.SaveChanges();
                    myGrid.ItemsSource = db.Смартфоны.ToList();
                    myGrid.Columns[4].Visibility = Visibility.Hidden;
                    myGrid.Columns[5].Visibility = Visibility.Hidden;
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }

        }
        private void SmartDelete(object sender, RoutedEventArgs e) // Полное удаление данных в таблице
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить все данные из таблицы ?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Смартфоны.RemoveRange(db.Смартфоны);
                    db.SaveChanges();
                    myGrid.ItemsSource = db.Смартфоны.ToList();
                    myGrid.Columns[4].Visibility = Visibility.Hidden;
                    myGrid.Columns[5].Visibility = Visibility.Hidden;
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }

        }
        private void SmartParse(object sender, RoutedEventArgs e)//парсинг телефонов
        {
             
            Смартфоны Смартфоны = new Смартфоны();
            HtmlWeb ws = new HtmlWeb();
            ws.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = ws.Load("http://ti-v-trende.ru/catalog/smartfony/");
            ArrayList list = new ArrayList();// spisok dlya sochraneniya ssilok
            int count = 0;

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'item-title')]//a[@href]"))
            {
                count++; //test
                list.Add("http://ti-v-trende.ru/" + node.GetAttributeValue("href", null));  // sohranyaem
            }
            textbox.Text = "Ссылки загружены. Количество: " + count;

            ArrayList truelist = new ArrayList();
            for(int i = 36; i < 95; i++) { truelist.Add(list[i]); }
            textbox.Text = "Начинаем загружать товары";
            foreach (string o in truelist) //pereborr ssilok
            {
               
                Thread.Sleep(500);
                doc = ws.Load(o);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='container']//h1"))//name
                {
                    if (link.InnerText == " ") { Смартфоны.Наименование = "Samsung Galaxy A8 2018 (32 Гб)"; }
                    else
                    {
                        Смартфоны.Наименование = link.InnerText;
                    }
                    db.Смартфоны.Add(Смартфоны);
                    db.SaveChanges();
                }
                try
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='offers_price']//span[@class='price_value']"))//price
                    {
                        if(link.InnerText == null) { Смартфоны.Цена = 0; } else { Смартфоны.Цена = Int32.Parse(link.InnerText.Trim().Replace(" ", string.Empty)); }
                        db.Смартфоны.Add(Смартфоны);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    Смартфоны.Цена = 0;
                    db.Смартфоны.Add(Смартфоны);
                    db.SaveChanges();

                }
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='container']//h1"))//brend foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id='navigation']/div[5]/a/span"))
                {
                    string[] splitBrend = link.InnerText.Split(' ');
                    Смартфоны.Бренд = splitBrend[1];
                    //Console.WriteLine($"Бренд: {smartphone.Brend}");
                    db.Смартфоны.Add(Смартфоны);
                    db.SaveChanges();
                }
            }
            
            textbox.Text = "Загрузка завершена";
        }

        //public void ParserView()
        //{

        //    using (ShopParserEntities db = new ShopParserEntities())
        //    {

        //        var smartpnone = db.Смартфоны.ToList();
        //        string message = "";
        //        foreach (Смартфоны смартфоны in smartpnone)
        //        {
        //            message += смартфоны.Наименование + "," + смартфоны.Цена + "/";
        //        }
        //        string[] result = message.Split('/');
        //        for (int i = 0; i < result.Length - 1; i++)
        //        {
        //            string[] k = result[i].Split(',');
        //            var smart = new Смартфоны();
        //            smart.Код_товара = Convert.ToInt32(k[0]);
        //            smart.Наименование = k[1];
        //            smart.Цена = k[2];
        //            smart.Бренд = k[3];
        //            Смартфон.Add(smart);
        //        }
        //    }

        //}

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SmartGet(object sender, RoutedEventArgs e) // загрузка данных с БД
        {
            
            myGrid.ItemsSource = db.Смартфоны.ToList();
            myGrid.Columns[4].Visibility = Visibility.Hidden;
            myGrid.Columns[5].Visibility = Visibility.Hidden;
        }
                                                                                   //______________________________________________________________________________планшеты
        private void PlanParse(object sender, RoutedEventArgs e) // парсинг планшетов
        {
            Планшеты планшеты = new Планшеты();
            Планшеты планшеты1 = new Планшеты();
            
            HtmlWeb ws = new HtmlWeb();
            ws.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = ws.Load("http://ti-v-trende.ru/catalog/planshety");
            ArrayList list = new ArrayList();// spisok dlya sochraneniya ssilok
            int count = 0;

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'item-title')]//a[@href]"))
            {
                count++; //test
                list.Add("http://ti-v-trende.ru/" + node.GetAttributeValue("href", null));  // sohranyaem
            }
            
            ArrayList truelist = new ArrayList();
            for (int i = 7; i < 30; i++) { truelist.Add(list[i]); }
            textblock1.Text = "Начинаем загружать товары";
            //MessageBox.Show(textblock1.Text);
            foreach (string o in truelist) //pereborr ssilok
            {

                Thread.Sleep(500);
                doc = ws.Load(o);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='container']//h1"))//name
                {

                    планшеты.Наименование = link.InnerText;
                    db.Планшеты.Add(планшеты);
                    db.SaveChanges();
                    
                }
                try
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='offers_price']//span[@class='price_value']"))//price
                    {
                        if (link.InnerText == null) { планшеты.Цена = 0; } else { планшеты.Цена = Int32.Parse(link.InnerText.Trim().Replace(" ", string.Empty)); }
                        db.Планшеты.Add(планшеты);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    планшеты.Цена = 0;
                    db.Планшеты.Add(планшеты);
                    db.SaveChanges();

                }

            }

            textblock1.Text = "Загрузка завершена";
        }

        private void PlanDeleteClick(object sender, RoutedEventArgs e) // удаление планшетов
        {
            Планшеты планшеты = myGrid2.SelectedItem as Планшеты;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить данную строку?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Планшеты.Remove(планшеты);
                    db.SaveChanges();
                    myGrid2.ItemsSource = db.Планшеты.ToList();
                    myGrid2.Columns[4].Visibility = Visibility.Hidden;
                   
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }
        }

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{

        //}

        private void PlanGet(object sender, RoutedEventArgs e)//загрузка планшетов с БД
        {
            myGrid2.ItemsSource = db.Планшеты.ToList();
            myGrid2.Columns[1].Visibility = Visibility.Hidden;
            myGrid2.Columns[4].Visibility = Visibility.Hidden;
        }

        private void myGrid2_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)// начало редактирования планшетов
        {
            PlanButtonSave.IsEnabled = true;
        }

        private void PlanSave(object sender, RoutedEventArgs e)// сохранение отредактированного у планшетов
        {
            Планшеты планшеты = myGrid2.SelectedItem as Планшеты;
            int strokaKod = планшеты.Код_товара;
            string strokaName = планшеты.Наименование;
            //string strokaBrend = смартфоны.Бренд;
            int strokaPrice = (int)планшеты.Цена;
            if (myGrid2.SelectedItem != null) // первый вариант
            {
                Планшеты item = (Планшеты)myGrid2.SelectedItem;
                item.Код_товара = strokaKod;
                item.Наименование = strokaName;
                //item.Бренд = strokaBrend;
                item.Цена = strokaPrice;
                myGrid2.Items.Refresh();
            }
            db.SaveChanges();
        }

        private void PlanDelete(object sender, RoutedEventArgs e)// полное удаление планшетов
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить все данные из таблицы ?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Планшеты.RemoveRange(db.Планшеты);
                    db.SaveChanges();
                    myGrid2.ItemsSource = db.Планшеты.ToList();
                    myGrid2.Columns[4].Visibility = Visibility.Hidden;
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }
        }
                                                                                        //______________________________________________________________________________Бренды

        private void BrendDeleteClick(object sender, RoutedEventArgs e) // удаление брендов
        {
            Бренды бренды = myGrid3.SelectedItem as Бренды;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить данную строку?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Бренды.Remove(бренды);
                    db.SaveChanges();
                    myGrid3.ItemsSource = db.Бренды.ToList();
                    myGrid3.Columns[2].Visibility = Visibility.Hidden;
                    myGrid3.Columns[3].Visibility = Visibility.Hidden;
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }
        }

        private void BrendParse(object sender, RoutedEventArgs e) // парсинг брендов
        {
            Бренды бренды = new Бренды();
            HtmlWeb ws = new HtmlWeb();
            ws.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = ws.Load("http://ti-v-trende.ru/brands");
            ArrayList list = new ArrayList();// spisok dlya sochraneniya ssilok
            int count = 0;

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'right_block')]//a[@href]"))
            {
                count++; //test
                list.Add("http://ti-v-trende.ru/" + node.GetAttributeValue("href", null));  // sohranyaem
            }

            ArrayList truelist = new ArrayList();
            for (int i = 8; i < list.Count; i++) { truelist.Add(list[i]); }
            textblock2.Text = "Начинаем загружать товары";
            //MessageBox.Show(textblock1.Text);
            foreach (string o in list) //pereborr ssilok
            {

                Thread.Sleep(500);
                doc = ws.Load(o);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id='pagetitle']"))//name
                {

                    бренды.Наименование = link.InnerText;
                    db.Бренды.Add(бренды);
                    db.SaveChanges();
                }
                //try
                //{
                //    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='offers_price']//span[@class='price_value']"))//price
                //    {
                //        if (link.InnerText == null) { планшеты.Цена = "1000"; } else { планшеты.Цена = link.InnerText; }
                //        db.Планшеты.Add(планшеты);
                //        db.SaveChanges();
                //    }
                //}
                //catch
                //{
                //    планшеты.Цена = "Нет в наличии";
                //    db.Планшеты.Add(планшеты);
                //    db.SaveChanges();

                //}
                //foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='container']//h1"))//brend foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id='navigation']/div[5]/a/span"))
                //{
                //    string[] splitBrend = link.InnerText.Split(' ');
                //    Смартфоны.Бренд = splitBrend[1];
                //    //Console.WriteLine($"Бренд: {smartphone.Brend}");
                //    db.Планшеты.Add(планшеты);
                //    db.SaveChanges();
                //}
            }

            textblock2.Text = "Загрузка завершена";

        }

        private void BrendGet(object sender, RoutedEventArgs e) // загрузка брендов с БД
        {
            myGrid3.ItemsSource = db.Бренды.ToList();
            myGrid3.Columns[2].Visibility = Visibility.Hidden;
            myGrid3.Columns[3].Visibility = Visibility.Hidden;
        }

        private void myGrid3_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) // начало редактирования брендов
        {
            BrendButtonSave.IsEnabled = true;
        }

        private void BrendSave(object sender, RoutedEventArgs e) // сохранение отредактированного брендов
        {
            Бренды бренды = myGrid3.SelectedItem as Бренды;
            int strokaKod = бренды.Код_Бренда;
            string strokaName = бренды.Наименование;
            //string strokaBrend = смартфоны.Бренд;
            //string strokaPrice = бренды.Цена;
            if (myGrid3.SelectedItem != null) // первый вариант
            {
                Бренды item = (Бренды)myGrid3.SelectedItem;
                item.Код_Бренда = strokaKod;
                item.Наименование = strokaName;
                //item.Бренд = strokaBrend;
                //item.Цена = strokaPrice;
                myGrid3.Items.Refresh();
            }
            db.SaveChanges();
        }

        private void BrendDelete(object sender, RoutedEventArgs e) // полное удаление данных в таблице брендов
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить все данные из таблицы ?", "Удаление", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Бренды.RemoveRange(db.Бренды);
                    db.SaveChanges();
                    myGrid3.ItemsSource = db.Планшеты.ToList();
                    myGrid3.Columns[2].Visibility = Visibility.Hidden;
                    myGrid3.Columns[3].Visibility = Visibility.Hidden;
                    MessageBox.Show("Удаление прошло успешно!", "Удаление");
                    break;
                case MessageBoxResult.No:
                    db.SaveChanges();
                    MessageBox.Show("Удаление отменено", "Удаление");
                    break;
            }
        }

      

        private void Open_shabl(object sender, RoutedEventArgs e)
        {
            COMFormatter comFormatter = new COMFormatter(@"C:\Users\5525m\source\repos\Kursovoi_Parsing\shablon.doc");
            int num = 0; // default category
            int bold = 0;
            string replacementSmartCount;
            string replacementPlanCount;
            string replacementBrandCount;
            string replacementCategory = "Смартфоны"; // default 
            string Planshet = "Планшеты";

            if (butsmart.IsChecked == true) { num = 2; comFormatter.Replace("{категория}", replacementCategory); } // category
            if (butplan.IsChecked == true) { num = 3; comFormatter.Replace("{категория}", Planshet); }
            else { comFormatter.Replace("{категория}", "Неизвестно"); }

            if (zhir.IsChecked == true) { bold = 1;  } // shrift
            if (_class.IsChecked == true) { bold = 2;  }
            if (curs.IsChecked == true) { bold = 3; }


            using (ShopParserEntities db = new ShopParserEntities())
            {
              replacementSmartCount = db.Смартфоны.Count().ToString();
              replacementPlanCount = db.Планшеты.Count().ToString();
              replacementBrandCount = db.Бренды.Count().ToString();
            }
           
            comFormatter.Replace("{смартфоны}", replacementSmartCount);
            comFormatter.Replace("{планшеты}", replacementPlanCount);
            comFormatter.Replace("{бренды}", replacementBrandCount);


            //comFormatter.Replace("{график}",AddPicture(path);

            comFormatter.WordTableFill(num,bold);
            comFormatter.Close();

            //Word.Application wordApp = new Word.Application();
            //Word.Document wordDoc = wordApp.Documents.Add();
            

            //wordDoc.SaveAs2(@"C:\Users\5525m\source\repos\Kursovoi_Parsing\Отчёт.doc");
            //wordApp.Quit();



        } // работа с отчётом

        private void Button_Click(object sender, RoutedEventArgs e) //создание графика
        {
            if (create_graf_button.IsEnabled) { create_otchet_button.IsEnabled = true; view_graf.IsEnabled = true; }
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            Excel.Application excelApp = new Excel.Application();
            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);
            object misValue = System.Reflection.Missing.Value;
            string smartDI;
            string PlanDI;
            string brenDI;

            using (ShopParserEntities db = new ShopParserEntities())
            {
                smartDI = db.Смартфоны.Count().ToString();
                PlanDI = db.Планшеты.Count().ToString();
                brenDI = db.Бренды.Count().ToString();
            }

            // create data
            workSheet.Cells[1, 1] = "Смартфоны";
            workSheet.Cells[1, 2] = "Планшеты";
            workSheet.Cells[1, 3] = "Бренды";


            workSheet.Cells[2, 1] = smartDI;
            workSheet.Cells[2, 2] = PlanDI;
            workSheet.Cells[2, 3] = brenDI;

            // create chart

            Excel.Range crange;
            Excel.ChartObjects cb = (Excel.ChartObjects)workSheet.ChartObjects(Type.Missing);
            Excel.ChartObject cbc = (Excel.ChartObject)cb.Add(10, 30, 300, 300);
            Excel.Chart cp = cbc.Chart;

            crange = workSheet.get_Range("a1", "c2");
            cp.SetSourceData(crange, misValue);
            cp.HasTitle = true;
            cp.ChartTitle.Text = "Количество товара на складе";

            // условия на тип графика
            if (chart1.IsChecked==true) { cp.ChartType = Excel.XlChartType.xl3DPie; }
            if (diagr.IsChecked == true) { cp.ChartType = Excel.XlChartType.xlColumnClustered; } //вид графика
            if (grafik.IsChecked == true) { cp.ChartType = Excel.XlChartType.xlLine; }


            if(chart1.IsChecked == false)
            {
                //Даем названия осей
                Excel.Axis xAxis = (Excel.Axis)cp.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                //xAxis.AxisTitle.Text = "Количество, шт.";

                Excel.Axis yAxis = (Excel.Axis)cp.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);

                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = "Категория";
            }
           

            cp.Export("C:\\Users\\5525m\\source\\repos\\Kursovoi_Parsing\\Graf.bmp", "BMP", misValue);


            workBook.SaveAs("C:\\Users\\5525m\\source\\repos\\Kursovoi_Parsing\\Graf.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue,misValue, misValue, misValue);
            workBook.Close(true, misValue, misValue);
            excelApp.Quit();
            textbox2.Text = "График создан";

        }

        private void view_graf_Click(object sender, RoutedEventArgs e) // Открытие графика
        {
            //image2.Source = new BitmapImage(new Uri(@"C:\Users\5525m\source\repos\Kursovoi_Parsing\Graf.bmp", UriKind.Relative));
            //image2.Visibility = Visibility.Visible;
            //image2.
        }
    }
}
