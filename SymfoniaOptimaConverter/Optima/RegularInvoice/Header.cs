using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima.RegularInvoice
{
   class VATSummary
   {
      public float                     m_vatAmount;
      public float                     m_vatValue;
      public float                     m_netValue;

      /**
       * Stores the contents in a dedicated record in an XML file.
       * 
       * @param xmlDoc
       */
      public void SaveToXml( XContainer xmlDoc )
      {
         CultureInfo  ci = Core.XmlParser.GetCultureInfo();

         xmlDoc.Add( new XElement( "LINIA_VAT", 
               new XElement( "STAWKA_VAT",
                  new XElement( "STAWKA", m_vatAmount.ToString( ci ) ),
                  new XElement( "FLAGA", "2" ),
                  new XElement( "ZRODLOWA", "0.00" )
                  ),
               new XElement( "NETTO", m_netValue.ToString(ci) ),
               new XElement( "VAT", m_vatValue.ToString(ci) ),
               new XElement( "BRUTTO", ( m_netValue + m_vatValue ).ToString(ci) ),
               new XElement( "NETTO_WAL", m_netValue.ToString(ci) ),
               new XElement( "VAT_WAL", m_vatValue.ToString(ci) ),
               new XElement( "BRUTTO_WAL", ( m_netValue + m_vatValue ).ToString(ci) )
               )
         );
      }
   }

   public class Header
   {
      private Contractor               m_buyer;
      private Contractor               m_issuer;

      public string                    m_invoiceNo;
      public string                    m_docDate;
      public string                    m_issueDate;
      public string                    m_operationDate;
      public string                    m_discountReturnDate = "";

      public string                    m_description;

      public string                    m_paymentDate;
      public string                    m_paymentMode;

      private float                    m_netTotal = 0.0f;
      private float                    m_vatTotal = 0.0f;

      private bool                     m_isInvoiceTaxFree = true;

      private List< VATSummary >       m_vatSummaryTable = new List<VATSummary>();

      /**
       * Invoice recipient.
       */
      public Contractor buyer
      {
         get { return m_buyer; }
         set { m_buyer = value; }
      }

      /**
       * Invoice issuer.
       */
      public Contractor issuer
      {
         get { return m_issuer; }
         set { m_issuer = value; }
      }

      /**
       * Constructor.
       */
      public Header( SymfoniaOptimaConverter.Symfonia.Header symfoniaHeader )
      {
         m_invoiceNo = symfoniaHeader.m_invoiceNo;
         m_docDate = symfoniaHeader.m_issueDate;
         m_issueDate = symfoniaHeader.m_issueDate;
         m_operationDate = symfoniaHeader.m_operationDate;
         m_description = symfoniaHeader.m_description;

         m_paymentMode = symfoniaHeader.m_paymentMode;
         m_paymentDate = symfoniaHeader.m_paymentDate;
      }

      /**
       * Processes the item, extracting the pricing data that will be included
       * in the VAT summary table etc.
       * 
       * @param item
       */
      internal void Process( Item item )
      {
         VATSummary summary = new VATSummary();
         summary.m_vatAmount = item.m_vatAmount;
         summary.m_vatValue = item.m_vatValue;
         summary.m_netValue = item.m_netValue;

         m_vatSummaryTable.Add( summary );

         // add the amounts to the total summary
         m_netTotal += item.m_netValue;
         m_vatTotal += item.m_vatValue;

         // If all items are tax free, then we should change the category of the invoice
         if ( !item.m_isTaxFree )
         {
            m_isInvoiceTaxFree = false;
         }
      }

      /**
       * Stores the contents in a dedicated record in an XML file.
       * 
       * @param xmlDoc
       */
      internal void SaveToXml( XContainer xmlDoc )
      {
         CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
         ci.NumberFormat.CurrencyDecimalSeparator = ".";
         ci.NumberFormat.NumberDecimalSeparator = ".";

         XElement headerElem = new XElement( "NAGLOWEK", 
            new XElement( "GENERATOR", "Comarch Opt!ma" ),
            new XElement( "TYP_DOKUMENTU", "302" ),
            new XElement( "RODZAJ_DOKUMENTU", "302000" ),
            new XElement( "FV_MARZA", "0" ),
            new XElement( "FV_MARZA_RODZAJ", "0" ),

            new XElement( "NUMER_PELNY", m_invoiceNo ),
            new XElement( "DATA_DOKUMENTU", m_docDate ),
            new XElement( "DATA_WYSTAWIENIA", m_issueDate ),
            new XElement( "DATA_OPERACJI", m_operationDate ),
            new XElement( "TERMIN_ZWROTU_KAUCJI", m_discountReturnDate ),

            new XElement( "KOREKTA", "0" ),
            new XElement( "DETAL", "0" ),
            new XElement( "TYP_NETTO_BRUTTO", "1" ),
            new XElement( "RABAT", "0.00" ),

            new XElement( "OPIS", m_description ),

            new XElement( "PLATNOSC",
               new XElement( "FORMAL", m_paymentMode ),
               new XElement( "TERMIN", m_paymentDate )
               ),


            getInvoiceCategoryCode(),

            new XElement( "WALUTA",
               new XElement( "SYMBOL", "PLN" ),
               new XElement( "KURS_L", "1.0" ),
               new XElement( "KURS_M", "1" ),
               new XElement( "PLAT_WAL_OD_PLN", "0" ),
               new XElement( "KURS_NUMER", "3" ),
               new XElement( "KURS_DATA", m_issueDate )
               ),

            new XElement( "KWOTY",
               new XElement( "RAZEM_NETTO_WAL", m_netTotal.ToString(ci) ),
               new XElement( "RAZEM_NETTO", m_netTotal.ToString(ci) ),
               new XElement( "RAZEM_BRUTTO", ( m_netTotal + m_vatTotal ).ToString(ci) ),
               new XElement( "RAZEM_VAT", m_vatTotal.ToString(ci) )
               ),

            new XElement( "MAGAZYN_ZRODLOWY", "MAGAZYN" ),
            new XElement( "MAGAZYN_DOCELOWY", "" ),
            new XElement( "KAUCJE_PLATNOSCI", "0" ),
            new XElement( "BLOKADA_PLATNOSCI", "0" ),
            new XElement( "VAT_DLA_DOK_WAL", "0" ),
            new XElement( "TRYB_NETTO_VAT", "0" )
         );

         buyer.SaveToXml( headerElem );
         issuer.SaveToXml( headerElem );

         xmlDoc.Add( headerElem );

         xmlDoc.Add( new XElement( "PLATNOSCI",
            new XElement( "PLATNOSC",
               new XElement( "FORMAL", m_paymentMode ),
               new XElement( "TERMIN", m_paymentDate ),
               new XElement( "KWOTA", ( m_netTotal + m_vatTotal ).ToString(ci) ),
               new XElement( "KWOTA_W_WAL_SYSTEMOWEJ", ( m_netTotal + m_vatTotal ).ToString(ci) ),
               new XElement( "WALUTA",
                  new XElement( "SYMBOL" ),
                  new XElement( "KURS_L", "1.00" ),
                  new XElement( "KURS_M", "1" )
                  )
               )
            )
         );

         xmlDoc.Add( new XElement( "KAUCJE" ) );
         xmlDoc.Add( new XElement( "PLATNOSCI_KAUCJE" ) );
      
         buildVatSummaryTable( xmlDoc );
      }

      private void buildVatSummaryTable( XContainer xmlDoc )
      {
         XElement rootElem = new XElement( "TABELKA_VAT" );
         xmlDoc.Add( rootElem );

         foreach( VATSummary summary in m_vatSummaryTable )
         {
            summary.SaveToXml( rootElem );
         }
      }

      private XElement getInvoiceCategoryCode()
      {
         if ( m_isInvoiceTaxFree )
         {
            return new XElement("KATEGORIA",
               new XElement("KOD", "U"),
               new XElement("OPIS", "Usługa")
               );
         }
         else
         {
            return new XElement("KATEGORIA",
               new XElement("KOD", "T"),
               new XElement("OPIS", "Sprzedaż towaru")
               );
         }
      }

   }
}
