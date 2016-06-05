using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Optima
{
   public class Contractor
   {
      public Core.ContractorRole          m_role;

      public string                       m_code;
      public string                       m_NIP;
      public string                       m_name;
      public string                       m_bankAccountNo;

      // address
      public string                       m_postalCode;
      public string                       m_city;
      public string                       m_street;
      public string                       m_houseNo;
      public string                       m_apartmentNo;
      public string                       m_country;

      public bool hasNIPAssigned
      {
         get { return m_NIP.Length == 10; }
      }

      /**
       * Constructor.
       */
      public Contractor( SymfoniaOptimaConverter.Symfonia.Contractor symfoniaContractor )
      {
         m_role = symfoniaContractor.m_role;

         m_code = symfoniaContractor.m_code;
         m_NIP = symfoniaContractor.m_NIP;
         m_name = symfoniaContractor.m_name;
         m_bankAccountNo = symfoniaContractor.m_bankAccountNo;

         m_postalCode = symfoniaContractor.m_postalCode;
         m_city = symfoniaContractor.m_city;
         m_street = symfoniaContractor.m_street;
         m_houseNo = symfoniaContractor.m_houseNo;
         m_apartmentNo = symfoniaContractor.m_apartmentNo;
         m_country = symfoniaContractor.m_country;

         m_NIP = m_NIP.Replace( "-", "" );
         m_bankAccountNo = m_bankAccountNo.Replace( "-", "" );

         if ( m_code == "3008" )
         {
            int a = 0;
            a -= 1;
         }
      }

      /**
       * Stores the contents in a dedicated record in an XML file.
       * 
       * @param xmlDoc
       */
      internal void SaveToXml( XContainer xmlDoc )
      {
         switch( m_role )
         {
            case Core.ContractorRole.Buyer:
               {
                  SaveWithTag( xmlDoc, "PLATNIK" );
                  SaveWithTag( xmlDoc, "ODBIORCA" );
                  break;
               }

            case Core.ContractorRole.Issuer:
               {
                  SaveWithTag( xmlDoc, "SPRZEDAWCA" );
                  break;
               }
         }
      }

      private void SaveWithTag( XContainer xmlDoc, string tag )
      {
         string fullStreetNo = m_street;
         if ( m_street.Length > 0 )
         {
            if ( m_houseNo.Length > 0 )
            {
               fullStreetNo += " " + m_houseNo;
            }
         
            if ( m_apartmentNo.Length > 0 )
            {
               fullStreetNo += " lok." + m_apartmentNo;
            }
         }

         xmlDoc.Add( new XElement( tag,
            new XElement( "KOD", m_code ),
            new XElement( "NIP", m_NIP ),
            new XElement( "GLN", "0000000000000" ),
            new XElement( "NAZWA", m_name ),
            new XElement( "ADRES", 
               new XElement( "KOD_POCZTOWY", m_postalCode ),
               new XElement( "MIASTO", m_city ),
               new XElement( "ULICA", fullStreetNo ),
               new XElement( "KRAJ", m_country )
               ),
            new XElement( "NUMER_KONTA_BANKOWEGO", m_bankAccountNo ),
            new XElement( "NAZWA_BANKU", "" )
            )
         );
      }

      public void SaveToCSV( StreamWriter output )
      {
         output.WriteLine( m_code + ";" +          // Kod
            m_name + ";" +                         // Nazwa
            ";" +                                  // Nazwa2
            ";" +                                  // Nazwa3
            ";" +                                  // Telefon
            ";" +                                  // Telefon2
            ";" +                                  // TelefonSms
            ";" +                                  // Fax
            m_street + ";" +                       // Ulica
            m_houseNo + ";" +                      // NrDomu
            m_apartmentNo + ";" +                  // NrLokalu
            m_postalCode + ";" +                   // KodPocztowy
            m_city + ";" +                         // Poczta
            m_city + ";" +                         // Miasto
            m_country + ";" +                      // Kraj
            ";" +                                  // Wojewodztwo
            ";" +                                  // Powiat
            ";" +                                  // Gmina
            ";" +                                  // URL
            ";" +                                  // Grupa
            "0;" +                                 // OsobaFizyczna
            m_NIP + ";" +                          // NIP
            ";" +                                  // NIPKraj
            ";" +                                  // Zezwolenie
            ";" +                                  // Regon
            ";" +                                  // Pesel
            ";" +                                  // Email
            m_bankAccountNo + ";" +                // BankRachunekNr
            ";" +                                  // BankNazwa
            ";" +                                  // Osoba
            ";" +                                  // Opis
            "0;" +                                 // Rodzaj
            "1;" +                                 // PlatnikVAT
            "0;" +                                 // Eksport
            "0;" +                                 // LimitKredytu
            "14;" +                                // Termin
            "przelew;" +                           // FormaPlatnosci
            "0;" +                                 // Ceny
            "1;" +                                 // PodatnikVatCzynny
            "domyślna;" +                          // CenyNazwa
            "0;" +                                 // Upust
            "0;" +                                 // NieNaliczajOdsetek
            "0;" +                                 // MetodaKasowa
            ";" +                                  // WindykacjaEMail
            ";"                                    // WindykacjaTelefonSms
            );
      }
   }
}
