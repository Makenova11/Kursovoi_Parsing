//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kursovoi_Parsing
{
    using System;
    using System.Collections.Generic;
    
    public partial class Смартфоны
    {
        public int Код_товара { get; set; }
        public string Наименование { get; set; }
        public Nullable<int> Цена { get; set; }
        public string Бренд { get; set; }
        public Nullable<int> Код_Бренда { get; set; }
    
        public virtual Бренды Бренды { get; set; }
    }
}
