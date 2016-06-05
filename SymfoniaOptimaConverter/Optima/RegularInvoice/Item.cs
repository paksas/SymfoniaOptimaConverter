using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima.RegularInvoice
{
   public class Item
   {
      public string      m_lp;

      public string      m_quantity;
      public float       m_price;
      public float       m_vatAmount;
      public float       m_netValue;
      public float       m_vatValue;

      public string      m_measureItem;

      public string      m_code;
      public string      m_name;

      /**
       * Constructor.
       */
      public Item( SymfoniaOptimaConverter.Symfonia.Item symfoniaItem )
      {
         m_lp = symfoniaItem.m_lp;

         m_quantity = symfoniaItem.m_quantity;
         m_price = symfoniaItem.m_price;
         m_netValue = symfoniaItem.m_netValue;
         m_vatValue = symfoniaItem.m_vatValue;

         m_vatAmount = symfoniaItem.m_vatAmount;
         m_measureItem = symfoniaItem.m_measureItem;

         m_code = symfoniaItem.m_code;
         m_name = symfoniaItem.m_name;
      }

      internal void SaveToXml( XContainer xmlDoc )
      {
         CultureInfo ci = Core.XmlParser.GetCultureInfo();

         // We're going to use predefined merchandise numbers.
         // One type is defined for merchandise with 23% VAT tax, the other - with 8% VAT tax, so we need to
         // determine which one we're dealing with
         int merchandiseCode = m_vatAmount >= 23.0f ? 1 : 2; // 1 - 23% VAT, 2 - 8% VAT

         xmlDoc.Add( new XElement( "POZYCJA",
            new XElement( "LP", m_lp ),
            new XElement( "TOWAR", 
               new XElement( "KOD", merchandiseCode.ToString( ci ) ),
               new XElement( "NAZWA", "" ),
               new XElement( "OPIS", "" ),
               new XElement( "EAN", "" ),
               new XElement( "NUMER_KATALOGOWY", "" )
               ),
            new XElement( "STAWKA_VAT",
               new XElement( "STAWKA", m_vatAmount ),
               new XElement( "FLAGA", "2" ),
               new XElement( "ZRODLOWA", "0.00" )
               ),
            new XElement( "CENY",
               new XElement( "POCZATKOWA_WAL_CENNIKA", m_price.ToString( ci ) ),
               new XElement( "POCZATKOWA_WAL_DOKUMENTU", m_price.ToString( ci ) ),
               new XElement( "PO_RABACIE_WAL_CENNIKA", m_price.ToString( ci ) ),
               new XElement( "PO_RABACIE_PLN", m_price.ToString( ci ) ),
               new XElement( "PO_RABACIE_WAL_DOKUMENTU", m_price.ToString( ci ) )
               ),
            new XElement( "WALUTA",
               new XElement( "SYMBOL", "PLN" ),
               new XElement( "KURS_L", "1.00" ),
               new XElement( "KURS_M", "1" )
               ),
            new XElement( "RABAT", "0.00" ),
            new XElement( "WARTOSC_NETTO", m_netValue.ToString( ci ) ),
            new XElement( "WARTOSC_BRUTTO", ( m_netValue + m_vatValue ).ToString( ci ) ),
            new XElement( "WARTOSC_NETTO_WAL", m_netValue.ToString( ci ) ),
            new XElement( "WARTOSC_BRUTTO_WAL", ( m_netValue + m_vatValue ).ToString( ci ) ),
            new XElement( "ILOSC", m_quantity ),
            new XElement( "JM", m_measureItem ),
            new XElement( "JM_CALKOWITE", "0.00" ),
            new XElement( "JM_ZLOZONA",
               new XElement( "JMZ", m_measureItem ),
               new XElement( "JM_PRZELICZNIK_L", "1.00" ),
               new XElement( "JM_PRZELICZNIK_M", "1" )
               )
            )
         );
      }
   }
}
