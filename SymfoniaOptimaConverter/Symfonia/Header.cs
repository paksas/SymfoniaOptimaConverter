using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Symfonia
{
   public class Header
   {
      public string      m_invoiceNo;
      public string      m_issueDate;
      public string      m_operationDate;

      public string      m_description;

      public string      m_paymentDate;
      public string      m_paymentMode;

      public float       m_netTotal;
      public float       m_vatTotal;

      public bool        m_convertValuesToNet;

      /**
       * Constructor.
       * 
       * @param invoiceElem
       */
      internal Header( XElement invoiceElem )
      {
         Core.XmlParser parser = new Core.XmlParser( invoiceElem );

         string priceType = "NETTO";
         parser.GetMandatoryStr( "NumerDokumentu", ref m_invoiceNo )
         .GetMandatoryStr( "DataWystawienia", ref m_issueDate )
         .GetMandatoryStr( "DataSprzedazy", ref m_operationDate )
         .GetMandatoryStr( "Nazwa", ref m_description )
         .GetMandatoryStr( "DataPlatnosci", ref m_paymentDate )
         .GetMandatoryStr( "FormaPlatnosci", ref m_paymentMode )
         .GetMandatoryStr( "RodzajCeny", ref priceType )
         .BeginMandatory( "Stopka" )
            .BeginMandatory( "KwotaVAT" )
               .GetMandatoryFloat( "Netto", ref m_netTotal )
               .GetMandatoryFloat( "VAT", ref m_vatTotal );

         if ( priceType.ToUpper() == "NETTO" )
         {
            // all prices listed on the invoice are net prices
            m_convertValuesToNet = false;
         }
         else
         {
            // all prices listed on the invoice are gross prices
            m_convertValuesToNet = true;
         }
      }
   }
}
