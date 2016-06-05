using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Symfonia
{
   public class Item
   {
      public string        m_lp;

      public string        m_quantity;
      public float         m_price;
      public float         m_vatAmount;
      public float         m_netValue;
      public float         m_vatValue;
      public bool          m_isTaxFree;

      public string        m_measureItem;

      public string        m_code;
      public string        m_name;
      
      /**
       * Constructor.
       * 
       * @param header
       * @param invoiceElem
       */
      internal Item( Header header, XElement itemElem )
      {
         Core.XmlParser parser = new Core.XmlParser( itemElem );

         string vatAmount = "";

         parser.GetMandatoryStr( "Lp", ref m_lp )
         .GetMandatoryStr( "Ilosc", ref m_quantity )
         .GetMandatoryFloat( "Cena", ref m_price )
         .GetMandatoryStr( "StawkaVAT", ref vatAmount )
         .GetMandatoryStr( "JednostkaMiary", ref m_measureItem )
         .GetMandatoryFloat( "Wartosc", ref m_netValue )
         .GetMandatoryFloat( "WartoscVAT", ref m_vatValue )
         .BeginMandatory( "Towar" )
            .GetMandatoryStr( "Kod", ref m_code )
            .BeginMandatory( "Nazwa" )
               .GetMandatoryStr( "Opis", ref m_name );

         if ( String.Equals( vatAmount.ToLower(), "zw" ) )
         {
            m_vatAmount = 0.0f;
            m_isTaxFree = true;
         }
         else
         {
            m_isTaxFree = false;

            try
            {
               string formatedVatAmount = vatAmount.Replace( "%", "" ) + ".00";
               CultureInfo ci = Core.XmlParser.GetCultureInfo();
               m_vatAmount = float.Parse( formatedVatAmount, NumberStyles.Any, ci );
            }
            catch( Exception )
            {
               throw new ArgumentNullException( "Invalid VAT amount - " + m_vatAmount );
            }
         }

         if ( header.m_convertValuesToNet )
         {
            float grossValue = m_netValue;
            m_netValue = grossValue / ( ( 100.0f + m_vatAmount ) / 100.0f );
            m_vatValue = grossValue - m_netValue;

            float grossToNet = ( 100.0f - m_vatAmount ) / 100.0f;
            m_price = m_price * grossToNet;
            m_price =  (float)( (int)(m_price * 100.0f) ) / 100.0f;
         }
      }

   }
}
