using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Symfonia
{
   public class Contractor
   {      
      public Core.ContractorRole       m_role;

      public string                    m_code;
      public string                    m_NIP;
      public string                    m_name;
      public string                    m_bankAccountNo;
      
      // address
      public string                    m_postalCode;
      public string                    m_city;
      public string                    m_street;
      public string                    m_houseNo;
      public string                    m_apartmentNo;
      public string                    m_country;

      /**
       * Constructor.
       * 
       * @param invoiceElem
       * @param role
       */
      internal Contractor( XElement invoiceElem, Core.ContractorRole role )
      {
         m_role = role;

         Core.XmlParser parser = new Core.XmlParser( invoiceElem );

         string nip = "";

         string[] roles = { "Wystawca", "Odbiorca" };
         string roleStr = roles[(int)role];

         parser.BeginMandatory( roleStr )
            .GetOptionalStr( "Kod", ref m_code )
            .GetMandatoryStr( "NIP", ref nip )
            .GetMandatoryStr( "Nazwa", ref m_name )
            .GetOptionalStr( "NumerRachunkuBankowego", ref m_bankAccountNo )
            .GetOptionalStr( "KodPocztowy", ref m_postalCode )
            .GetOptionalStr( "Miejscowosc", ref m_city )
            .GetOptionalStr( "NumerDomu", ref m_houseNo )
            .GetOptionalStr( "NumerLokalu", ref m_apartmentNo )
            .GetOptionalStr( "Ulica", ref m_street )
            .GetOptionalStr( "Kraj", ref m_country );

         m_NIP = nip.Replace( "-", "" );
      }
   }
}
